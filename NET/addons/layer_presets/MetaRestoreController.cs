using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LayerPresets;
public static class MetaRestoreController
{
    public static void RestoreLayersFromMeta(PropertyHint targetPropertyHint, string folder = "res://")
    {
        List<string> scenePaths = [];
        FindSceneFiles(folder, scenePaths);

        var editorInterface = EditorInterface.Singleton;
        var openedScenes = editorInterface.GetOpenScenes();
        foreach (string path in scenePaths)
        {
            GD.Print($"Checking scene {path}");
            var isOpen = openedScenes.Contains(path);
            bool modified = false;
            if (isOpen)
            {
                editorInterface.OpenSceneFromPath(path);
                var rootNode = editorInterface.GetEditedSceneRoot();

                ProcessNode(rootNode, ref modified, targetPropertyHint);

                if (modified)
                {
                    editorInterface.SaveScene();
                }
            }
            else
            {
                var packedScene = GD.Load<PackedScene>(path);
                if (packedScene == null) continue;

                var rootNode = packedScene.Instantiate<Node>();

                // temporarily adding because setting properties require scene being in the tree
                editorInterface.GetEditedSceneRoot().AddChild(rootNode);
                ProcessNode(rootNode, ref modified, targetPropertyHint);
                rootNode.GetParent()?.RemoveChild(rootNode);

                if (modified)
                {
                    packedScene.Pack(rootNode);
                    ResourceSaver.Save(packedScene, path);
                }

                rootNode.QueueFree();
            }

            if (modified)
            {
                GD.Print($"Scene updated: {path}");
            }
        }
    }

    private static void FindSceneFiles(string dirPath, List<string> scenePaths)
    {
        using DirAccess dir = DirAccess.Open(dirPath);
        if (dir == null) return;

        dir.ListDirBegin();
        string fileName = dir.GetNext();

        while (!string.IsNullOrEmpty(fileName))
        {
            string fullPath = dir.GetCurrentDir().PathJoin(fileName);

            if (dir.CurrentIsDir())
            {
                if (fileName != "." && fileName != "..")
                {
                    FindSceneFiles(fullPath, scenePaths);
                }
            }
            else if (fileName.GetExtension() == "tscn")
            {
                scenePaths.Add(fullPath);
            }
            fileName = dir.GetNext();
        }
    }

    private static void ProcessNode(Node node, ref bool modified, PropertyHint targetPropertyHint)
    {
        foreach (var prop in node.GetPropertyList())
        {
            string propertyName = prop["name"].AsString();

            ProcessProperty(node, prop, targetPropertyHint, node.Name, ref modified);

            var val = node.Get(propertyName);
            if (val.Obj is Resource resource)
            {
                foreach(var resourceProp in resource.GetPropertyList())
                {
                    ProcessProperty(resource, resourceProp, targetPropertyHint, $"Resource ID: {resource.ResourceSceneUniqueId}", ref modified);
                }

                // saving the file if resource is not local to scene
                if (!string.IsNullOrEmpty(resource.ResourcePath))
                {
                    ResourceSaver.Save(resource, resource.ResourcePath);
                }
            }
        }

        foreach (Node child in node.GetChildren())
        {
            ProcessNode(child, ref modified, targetPropertyHint);
        }
    }

    private static void ProcessProperty(GodotObject @object, Dictionary prop, PropertyHint targetPropertyHint, string objectName, ref bool modified)
    {
        string propertyName = prop["name"].AsString();
        var propertyHint = prop["hint"].As<PropertyHint>();
        var metaName = PresetsController.GetPresetMetaName(@object, propertyName, propertyHint);
        if (propertyHint == targetPropertyHint && @object.HasMeta(metaName))
        {
            var presetId = (string)@object.GetMeta(metaName);
            var preset = PresetsController.GetPreset(presetId, propertyHint);
            var currentValue = (uint)@object.Get(propertyName);
            if (preset != null)
            {
                if (currentValue != preset.Layer)
                {
                    @object.Set(propertyName, preset.Layer);
                    GD.Print($"Restored {SettingsConstants.GetFormattedPropertyHintName(propertyHint)} {propertyName} to preset {preset.Name} on object: '{objectName}'");
                    modified = true;
                }
            }
            else
            {
                @object.RemoveMeta(metaName);
                GD.Print($"Removed stale meta [{metaName} = {presetId}] on Node: {objectName}");
                modified = true;
            }
        }
    }
}
