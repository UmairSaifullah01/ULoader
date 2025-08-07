#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using System.Linq;
using System.IO;
using THEBADDEST.Assets;

[InitializeOnLoad]
public static class AutoMarkAddressablesOnPlay
{
    static AutoMarkAddressablesOnPlay()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            MarkAllAssetsInFoldersAsAddressable();
        }
    }

    private static void MarkAllAssetsInFoldersAsAddressable()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogWarning("AddressableAssetSettings not found.");
            return;
        }

        // Try to load ULoader config for resource folders
        string configPath = "Assets/ULoader/Editor/ULoadConfig.asset";
        ULoaderConfig config = AssetDatabase.LoadAssetAtPath<ULoaderConfig>(configPath);
        string[] resourceFolders;
        if (config != null && config.ResourceFolders != null && config.ResourceFolders.Count > 0)
            resourceFolders = config.ResourceFolders.ToArray();
        else
            resourceFolders = new string[] { "Assets/MyResources" };

        foreach (var folder in resourceFolders)
        {
            if (!Directory.Exists(folder)) continue;
            var guids = AssetDatabase.FindAssets("", new[] { folder });
            foreach (var guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (Directory.Exists(assetPath)) continue; // skip folders

                var entry = settings.FindAssetEntry(guid);
                if (entry == null)
                {
                    settings.CreateOrMoveEntry(guid, settings.DefaultGroup);
                    Debug.Log($"[ULoader] Marked as Addressable: {assetPath}");
                }
            }
        }
        settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, null, true);
        AssetDatabase.SaveAssets();
    }
}
#endif
