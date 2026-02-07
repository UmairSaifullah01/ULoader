#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;


namespace THEBADDEST.Assets
{


	public class ULoadConfigEditorWindow : EditorWindow
	{

		private ULoaderConfig config;
		private const string ConfigHashKey = "ULoader_LastConfigHash";
		private const string LastBuildTimeKey = "ULoader_LastBuildTime";
		private static string lastError = "";
		
		private static string GetConfigAssetPath()
		{
			// Find the script file to determine the package location
			string[] guids = AssetDatabase.FindAssets("t:Script ULoadConfigEditorWindow");
			if (guids.Length > 0)
			{
				string scriptPath = AssetDatabase.GUIDToAssetPath(guids[0]);
				string packageDir = System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(scriptPath)); // Go up two levels from Editor folder
				return System.IO.Path.Combine(packageDir, "Editor/Content/ULoaderConfig.asset").Replace("\\", "/");
			}
			
			// Fallback to the new default path if script not found
			return "Assets/ULoader/Editor/Content/ULoaderConfig.asset";
		}

		[MenuItem("ULoader/Config Editor")]
		public static void ShowWindow()
		{
			GetWindow<ULoadConfigEditorWindow>("ULoader Config");
		}

		private void OnEnable()
		{
			string configAssetPath = GetConfigAssetPath();
			config = AssetDatabase.LoadAssetAtPath<ULoaderConfig>(configAssetPath);
			if (config == null)
			{
				config = CreateInstance<ULoaderConfig>();
				string dir = System.IO.Path.GetDirectoryName(configAssetPath);
				if (!System.IO.Directory.Exists(dir))
					System.IO.Directory.CreateDirectory(dir);
				AssetDatabase.CreateAsset(config, configAssetPath);
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
					if (!ULoaderBuilder_EditorWindowBridge.AssignAddresses())
					{
						lastError = "Address assignment failed.";
					}
					else if (!ULoaderBuilder_EditorWindowBridge.BuildAddressablesContent())
					{
						lastError = "Addressables build failed.";
					}
					else
					{
						lastError = "";
						EditorPrefs.SetString(LastBuildTimeKey, DateTime.Now.ToString());
						EditorPrefs.SetString(ConfigHashKey, ULoaderBuilder_EditorWindowBridge.GetConfigHash(config));
						Debug.Log("ULoader: Manual build completed successfully.");
					}
				}
				catch (Exception ex)
				{
					lastError = ex.Message + "\n" + ex.StackTrace;
				}
			}
		}

	}

	// Bridge for EditorWindow to call static methods in ULoaderBuilder
	public static class ULoaderBuilder_EditorWindowBridge
	{

		public static bool AssignAddresses()
		{
			var method = typeof(ULoaderBuilder).GetMethod("AssignAddresses", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
			return (bool)method.Invoke(null, null);
		}

		public static bool BuildAddressablesContent()
		{
			var method = typeof(ULoaderBuilder).GetMethod("BuildAddressablesContent", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
			return (bool)method.Invoke(null, null);
		}

		public static string GetConfigHash(ULoaderConfig config)
		{
			var method = typeof(ULoaderBuilder).GetMethod("GetConfigHash", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
			return (string)method.Invoke(null, new object[] { config });
		}

	}
#endif


}