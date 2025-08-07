#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

public class ULoadBuilder : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;
    private const string ConfigAssetPath = "Assets/ULoad/Editor/ULoadConfig.asset";
    private const string ConfigHashKey = "ULoad_LastConfigHash";

    public void OnPreprocessBuild(BuildReport report)
    {
        Debug.Log("ULoad: Running Addressables Build + Assignment");
        try
        {
            if (ShouldRebuild())
            {
                if (!AssignAddresses())
                {
                    Debug.LogError("ULoad: Address assignment failed. Build aborted.");
                    return;
                }
                if (!BuildAddressablesContent())
                {
                    Debug.LogError("ULoad: Addressables build failed.");
                    return;
                }
                SaveCurrentConfigHash();
            }
            else
            {
                Debug.Log("ULoad: No changes detected in config, skipping Addressables build.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"ULoad: Exception during build process: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private static bool ShouldRebuild()
    {
        var config = AssetDatabase.LoadAssetAtPath<ULoadConfig>(ConfigAssetPath);
        if (config == null) return true;
        string currentHash = GetConfigHash(config);
        string lastHash = EditorPrefs.GetString(ConfigHashKey, "");
        return currentHash != lastHash;
    }

    private static void SaveCurrentConfigHash()
    {
        var config = AssetDatabase.LoadAssetAtPath<ULoadConfig>(ConfigAssetPath);
        if (config == null) return;
        string currentHash = GetConfigHash(config);
        EditorPrefs.SetString(ConfigHashKey, currentHash);
    }

    private static string GetConfigHash(ULoadConfig config)
    {
        var sb = new StringBuilder();
        foreach (var folder in config.ResourceFolders)
            sb.Append(folder);
        foreach (var rule in config.GroupingRules)
        {
            sb.Append(rule.Folder).Append(rule.GroupName).Append(rule.GroupingType).Append(rule.CustomLabel);
        }
        using (var md5 = MD5.Create())
        {
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
            return System.BitConverter.ToString(hash);
        }
    }

    private static bool AssignAddresses()
    {
        try
        {
            var config = AssetDatabase.LoadAssetAtPath<ULoadConfig>(ConfigAssetPath);
            if (config == null || config.ResourceFolders == null || config.ResourceFolders.Count == 0)
            {
                Debug.LogWarning($"ULoad: No resource folders configured in {ConfigAssetPath}");
                return false;
            }
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            foreach (var folder in config.ResourceFolders)
            {
                foreach (var guid in AssetDatabase.FindAssets("", new[] { folder }))
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    if (Directory.Exists(assetPath)) continue;
                    string relativeAddress = assetPath.Replace(folder + "/", "").Replace(Path.GetExtension(assetPath), "");
                    AddressableAssetGroup group = settings.DefaultGroup;
                    string labelToAdd = null;
                    foreach (var rule in config.GroupingRules)
                    {
                        if (!assetPath.StartsWith(rule.Folder)) continue;
                        switch (rule.GroupingType)
                        {
                            case ULoadGroupingType.ByType:
                                // Grouping by type logic can be implemented here
                                break;
                            case ULoadGroupingType.BySubfolder:
                                // Grouping by subfolder logic can be implemented here
                                break;
                            case ULoadGroupingType.CustomLabel:
                                labelToAdd = rule.CustomLabel;
                                break;
                        }
                    }
                    var entry = settings.CreateOrMoveEntry(guid, group);
                    entry.address = relativeAddress;
                    if (!string.IsNullOrEmpty(labelToAdd) && !entry.labels.Contains(labelToAdd))
                        entry.SetLabel(labelToAdd, true, true);
                }
            }
            AssetDatabase.SaveAssets();
            Debug.Log("ULoad: Address assignment completed successfully.");
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"ULoad: Exception during address assignment: {ex.Message}\n{ex.StackTrace}");
            return false;
        }
    }

    private static bool BuildAddressablesContent()
    {
        try
        {
            AddressableAssetSettings.BuildPlayerContent();
            // if (result.Error != null && result.Error != "")
            // {
            //     Debug.LogError($"ULoad: Addressables build error: {result.Error}");
            //     return false;
            // }
            Debug.Log("ULoad: Addressables build completed successfully.");
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"ULoad: Exception during Addressables build: {ex.Message}\n{ex.StackTrace}");
            return false;
        }
    }
}
#endif
