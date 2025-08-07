#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;

[InitializeOnLoad]
public static class ForceAssetDatabaseMode
{
    static ForceAssetDatabaseMode()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings != null && settings.ActivePlayModeDataBuilder.Name != "Use Asset Database (fastest)")
        {
            foreach (var builder in settings.DataBuilders)
            {
                if (builder.name == "Use Asset Database (fastest)")
                {
                    settings.ActivePlayModeDataBuilderIndex = settings.DataBuilders.IndexOf(builder);
                    Debug.Log("[ULoader] Set Addressables to Asset Database mode for Editor.");
                    break;
                }
            }
        }
    }
}
#endif
