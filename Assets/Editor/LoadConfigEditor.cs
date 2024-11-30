using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Unity.Plastic.Newtonsoft.Json;
using System;

public class LoadConfigEditor : MonoBehaviour
{
    private static Dictionary<string, string> m_Suffix2NamePrefix = new()
    {
        {"prefab", "Pre_" },
        {"sprite", "Sp_" },
        {"animation", "Anim_" },
    };
    private static string[] m_SearchPaths = new string[]
    {
        "Assets/Abbresources",
        "Assets/Resources",
    };
    private static string m_ConfigJsonPath => ABBUtil.GetFullPathByUnityPath(GlobalConfig.LoadConfigRecordsJson);
    private static string m_EnLoadTargetPath => ABBUtil.GetFullPathByUnityPath(GlobalConfig.LoadTargetEnum);
    [MenuItem("Tools/Load/UpdateConfigJson")]
    public static void CreateLoadConfigJson()
    {
        var itemList = new List<LoadConfigItem>();
        var targetList = new List<string>();
        foreach (var item in m_Suffix2NamePrefix)
        {
            var assets = AssetDatabase.FindAssets($"t:{item.Key}", m_SearchPaths);
            Debug.Log(assets.Length);

            foreach (var guid in assets)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var name = Path.GetFileNameWithoutExtension(assetPath);
                var targetType = $"{item.Value}{name}";
                var index = targetList.Count;
                targetList.Add(targetType);

                itemList.Add(new()
                {
                    LoadTarget = (EnLoadTarget)index,
                    Path = assetPath,
                });
            }
        }

        WriteEnLoadTargetFile(targetList);

        WriteEnLoadTargetFile(itemList);
    }
    private static void WriteEnLoadTargetFile(List<string> targetList)
    {
        var result = $"public enum EnLoadTarget" +
            $"\n{{";

        for (int i = 0; i < targetList.Count; i++)
        {
            result += $"\n\t{targetList[i]},";
        }
        result += $"\n}}";
        File.WriteAllText(m_EnLoadTargetPath, result);
        AssetDatabase.Refresh();
    }

    private static void WriteEnLoadTargetFile(List<LoadConfigItem> data)
    {
        var result = JsonConvert.SerializeObject(data);
        File.WriteAllText(m_ConfigJsonPath, result);
        AssetDatabase.Refresh();
    }
}
