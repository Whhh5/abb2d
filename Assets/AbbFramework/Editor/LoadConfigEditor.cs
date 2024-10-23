using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Unity.Plastic.Newtonsoft.Json;
using System;

public class LoadConfigEditor : MonoBehaviour
{
    private static string m_UpdtaeTargetFloder = "";
    private static Dictionary<string, string> m_Suffix2NamePrefix = new()
    {
        {"prefab", "Pre_" },
        {"sprite", "Sp_" },
    };
    private static string[] m_SearchPaths = new string[]
    {
        "Assets/Abbresources",
        "Assets/Resources",
    };
    private static string m_ConfigJsonPath = $"{Application.dataPath}/Resources/{GlobalConfig.LoadConfigRecordsJson}";
    private static string m_EnLoadTargetPath = $"{Application.dataPath}/AbbFramework/Scripts/Load/EnLoadTarget.cs";
    [MenuItem("Tools/Load/UpdateConfigJson")]
    public static void CreateLoadConfigJson()
    {
        loop();
        loop();
        void loop()
        {
            var itemList = new List<LoadConfigItem>();
            var targetStr = "";
            foreach (var item in m_Suffix2NamePrefix)
            {
                var assets = AssetDatabase.FindAssets($"t:{item.Key}", m_SearchPaths);
                Debug.Log(assets.Length);

                foreach (var guid in assets)
                {
                    var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    var name = Path.GetFileNameWithoutExtension(assetPath);
                    var targetType = $"{item.Value}{name}";
                    targetStr += "\t" + targetType + ",";

                    if (Enum.TryParse<EnLoadTarget>(targetType, out var result))
                    {
                        itemList.Add(new()
                        {
                            LoadTarget = result,
                            Path = assetPath.Replace("Assets/", ""),
                        });
                    }
                }
            }

            WriteEnLoadTargetFile(targetStr);

            WriteEnLoadTargetFile(itemList);
        }
    }
    private static void WriteEnLoadTargetFile(string str)
    {
        var result = $"public enum EnLoadTarget\n" +
            $"{{\n" +
            $"{str}\n" +
            $"}}";
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
