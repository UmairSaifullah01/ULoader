namespace THEBADDEST.Assets
{
using System.Collections.Generic;
using UnityEngine;

public enum ULoadGroupingType { None, ByType, BySubfolder, CustomLabel }

[System.Serializable]
public class ULoadGroupingRule
{
    public string Folder;
    public string GroupName;
    public ULoadGroupingType GroupingType;
    public string CustomLabel;
}

[CreateAssetMenu(fileName = "ULoaderConfig", menuName = "ULoad/Config", order = 0)]
public class ULoaderConfig : ScriptableObject
{
    public List<string> ResourceFolders = new List<string> { "Assets/MyResources" };
    public List<ULoadGroupingRule> GroupingRules = new List<ULoadGroupingRule>();
}
}