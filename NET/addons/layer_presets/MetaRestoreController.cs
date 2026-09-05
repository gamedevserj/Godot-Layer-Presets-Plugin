using Godot;
using System.Collections.Generic;

namespace LayerPresets;
public static class MetaRestoreController
{
    public static void RestoreLayersFromMeta(PropertyHint targetPropertyHint, string folder = "res://")
    {
        List<string> scenePaths = [];
        FindSceneFiles(folder, scenePaths);

        int updatedScenesCount = 0;

        foreach (string path in scenePaths)
        {
            var editorInterface = EditorInterface.Singleton;
            editorInterface.OpenSceneFromPath(path);

            var rootNode = editorInterface.GetEditedSceneRoot();
            bool modified = false;

            ProcessNode(rootNode, ref modified, targetPropertyHint);

            if (modified)
            {
                editorInterface.SaveScene();
                updatedScenesCount++;
                GD.Print($"Scene updated: {path}");
            }

            editorInterface.CloseScene();
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
            var propertyHint = prop["hint"].As<PropertyHint>();
            var metaName = PresetsController.GetPresetMetaName(node, propertyName, propertyHint);
            if (propertyHint == targetPropertyHint && node.HasMeta(metaName))
            {
                var presetId = (string)node.GetMeta(metaName);
                var preset = PresetsController.GetPreset(presetId, propertyHint);
                node.Set(propertyName, preset.Layer);
                modified = true;
                GD.Print($"Restored {SettingsConstants.GetFormattedPropertyName(propertyHint)} {propertyName} to preset {preset.Name} on Node: {node.Name}");
            }

            var val = node.Get(propertyName);
            if (val.Obj is Resource resource && ProcessResource(resource))
            {
                modified = true;

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

    private static bool ProcessResource(Resource resource)
    {
        bool resourceModified = false;
        foreach (var prop in resource.GetPropertyList())
        {
            var propertyHint = prop["hint"].As<PropertyHint>();
            if (propertyHint == PropertyHint.Layers3DPhysics)
            {
                string propertyName = prop["name"].AsString();
                var metaName = PresetsController.GetPresetMetaName(resource, propertyName, propertyHint);
                if (resource.HasMeta(metaName))
                {
                    var presetId = (string)resource.GetMeta(metaName);
                    var preset = PresetsController.GetPreset(presetId, propertyHint);
                    resource.Set(propertyName, preset.Layer);
                    resourceModified = true;
                    GD.Print($"Restored {SettingsConstants.GetFormattedPropertyName(propertyHint)} {propertyName} to preset {preset.Name}");
                }
            }
        }
        return resourceModified;
    }
}
