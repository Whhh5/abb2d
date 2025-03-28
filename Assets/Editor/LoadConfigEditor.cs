using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Unity.Plastic.Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;
using System.Text;

public class LoadConfigEditor : MonoBehaviour
{
    private static Dictionary<string, string> m_Suffix2NamePrefix = new()
    {
        {"asset", "SO_" },
        {"prefab", "Pre_" },
        {"png", "Sp_" },
        {"FBX", "Anim_" },
        {"anim", "Anim_" },
        {"mask", "AvaMask_" },
    };
    private static string[] m_SearchPaths = new string[]
    {
        "Assets/Abbresources",
        "Assets/Resources",
        "Assets/AbbFramework",
    };
    private static string m_ConfigJsonPath => ABBUtil.GetFullPathByUnityPath(GlobalConfig.LoadConfigRecordsJson);
    private static string m_EnLoadTargetPath => ABBUtil.GetFullPathByUnityPath(GlobalConfig.LoadTargetEnum);
    [MenuItem("Tools/Load/UpdateConfigJson")]
    public static void CreateLoadConfigJson()
    {
        var curAssetDic = new Dictionary<string, AssetCfg>();
        var assetCfgCount = ExcelUtil.GetCfgCount<AssetCfg>();
        for (int i = 0; i < assetCfgCount; i++)
        {
            var cfg = ExcelUtil.GetCfgByIndex<AssetCfg>(i);
            curAssetDic.Add(cfg.strDescEditor, cfg);
        }


        foreach (var item in m_Suffix2NamePrefix)
        {
            foreach (var searchPath in m_SearchPaths)
            {
                var fullDir = ABBUtil.GetFullPathByUnityPath(searchPath);
                var dirInfo = new DirectoryInfo(fullDir);
                var fileList = dirInfo.GetFiles($"*.{item.Key}", SearchOption.AllDirectories);
                foreach (var fileInfo in fileList)
                {
                    var fullPath = fileInfo.FullName;
                    var name = Path.GetFileNameWithoutExtension(fullPath);
                    var strPath = ABBUtil.GetUnityPathByFullPath(fullPath);
                    var strDescEditor = $"{item.Value}{name}";


                    if (!curAssetDic.TryGetValue(strDescEditor, out var cfg))
                    {
                        cfg = ExcelUtil.CreateTypeInstance<AssetCfg>();
                        var assetID = ExcelUtil.GetNextIndex<AssetCfg>();
                        ExcelUtil.SetCfgValue(cfg, nameof(cfg.strDescEditor), strDescEditor);
                        ExcelUtil.SetCfgValue(cfg, nameof(cfg.nAssetID), assetID);
                        curAssetDic.Add(strDescEditor, cfg);
                        ExcelUtil.AddCfg(cfg);
                    }
                    if(cfg.strPath != strPath)
                        ExcelUtil.SetCfgValue(cfg, nameof(cfg.strPath), strPath);
                }
            }
        }

        ExcelUtil.SaveExcel<AssetCfg>();
        WriteEnLoadTargetFile();
    }

    private static void WriteEnLoadTargetFile()
    {
        var enumStr = new StringBuilder();
        enumStr.AppendLine($"public enum EnLoadTarget");
        enumStr.AppendLine($"{{");
        enumStr.AppendLine($"\tNone = 0,");


        var count = ExcelUtil.GetCfgCount<AssetCfg>();
        for (int i = 0; i < count; i++)
        {
            var cfg = ExcelUtil.GetCfgByIndex<AssetCfg>(i);

            enumStr.AppendLine($"\t{cfg.strDescEditor} = {cfg.nAssetID}, // {cfg.strPath}");
        }

        enumStr.AppendLine("}");
        File.WriteAllText(m_EnLoadTargetPath, enumStr.ToString());
        AssetDatabase.Refresh();
    }
}

