#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;

public class ULoadConfigEditorWindow : EditorWindow
{
    private ULoadConfig config;
    private const string ConfigAssetPath = "Assets/ULoad/Editor/ULoadConfig.asset";
    private const string ConfigHashKey = "ULoad_LastConfigHash";
    private const string LastBuildTimeKey = "ULoad_LastBuildTime";
    private static string lastError = "";

    [MenuItem("ULoad/Config Editor")]
    public static void ShowWindow()
    {
        GetWindow<ULoadConfigEditorWindow>("ULoad Config");
    }

    private void OnEnable()
    {
        config = AssetDatabase.LoadAssetAtPath<ULoadConfig>(ConfigAssetPath);
        if (config == null)
        {
            config = CreateInstance<ULoadConfig>();
            AssetDatabase.CreateAsset(config, ConfigAssetPath);
            AssetDatabase.SaveAssets();
        }
    }

    private void OnGUI()
    {
        if (config == null)
        {
            EditorGUILayout.HelpBox("Config asset not found or failed to load.", MessageType.Error);
            return;
        }

        EditorGUILayout.LabelField("Resource Folders", EditorStyles.boldLabel);
        for (int i = 0; i < config.ResourceFolders.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            config.ResourceFolders[i] = EditorGUILayout.TextField(config.ResourceFolders[i]);
            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                config.ResourceFolders.RemoveAt(i);
                break;
            }
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("Add Folder"))
        {
            config.ResourceFolders.Add("");
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Grouping Rules", EditorStyles.boldLabel);
        for (int i = 0; i < config.GroupingRules.Count; i++)
        {
            var rule = config.GroupingRules[i];
            EditorGUILayout.BeginVertical("box");
            rule.Folder = EditorGUILayout.TextField("Folder", rule.Folder);
            rule.GroupName = EditorGUILayout.TextField("Group Name", rule.GroupName);
            rule.GroupingType = (ULoadGroupingType)EditorGUILayout.EnumPopup("Grouping Type", rule.GroupingType);
            if (rule.GroupingType == ULoadGroupingType.CustomLabel)
                rule.CustomLabel = EditorGUILayout.TextField("Custom Label", rule.CustomLabel);
            if (GUILayout.Button("Remove Rule"))
            {
                config.GroupingRules.RemoveAt(i);
                break;
            }
            EditorGUILayout.EndVertical();
        }
        if (GUILayout.Button("Add Grouping Rule"))
        {
            config.GroupingRules.Add(new ULoadGroupingRule());
        }
        if (GUILayout.Button("Save"))
        {
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Build Status", EditorStyles.boldLabel);
        string lastBuildTime = EditorPrefs.GetString(LastBuildTimeKey, "Never");
        string lastConfigHash = EditorPrefs.GetString(ConfigHashKey, "None");
        EditorGUILayout.LabelField("Last Build Time:", lastBuildTime);
        EditorGUILayout.LabelField("Last Config Hash:", lastConfigHash);
        if (!string.IsNullOrEmpty(lastError))
        {
            EditorGUILayout.HelpBox(lastError, MessageType.Error);
        }
        if (GUILayout.Button("Run Address Assignment + Build"))
        {
            try
            {
                if (!ULoadBuilder_EditorWindowBridge.AssignAddresses())
                {
                    lastError = "Address assignment failed.";
                }
                else if (!ULoadBuilder_EditorWindowBridge.BuildAddressablesContent())
                {
                    lastError = "Addressables build failed.";
                }
                else
                {
                    lastError = "";
                    EditorPrefs.SetString(LastBuildTimeKey, DateTime.Now.ToString());
                    EditorPrefs.SetString(ConfigHashKey, ULoadBuilder_EditorWindowBridge.GetConfigHash(config));
                    Debug.Log("ULoad: Manual build completed successfully.");
                }
            }
            catch (Exception ex)
            {
                lastError = ex.Message + "\n" + ex.StackTrace;
            }
        }
    }
}

// Bridge for EditorWindow to call static methods in ULoadBuilder
public static class ULoadBuilder_EditorWindowBridge
{
    public static bool AssignAddresses()
    {
        var method = typeof(ULoadBuilder).GetMethod("AssignAddresses", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        return (bool)method.Invoke(null, null);
    }
    public static bool BuildAddressablesContent()
    {
        var method = typeof(ULoadBuilder).GetMethod("BuildAddressablesContent", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        return (bool)method.Invoke(null, null);
    }
    public static string GetConfigHash(ULoadConfig config)
    {
        var method = typeof(ULoadBuilder).GetMethod("GetConfigHash", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        return (string)method.Invoke(null, new object[] { config });
    }
}
#endif