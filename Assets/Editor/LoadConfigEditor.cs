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
        var itemList = new List<AssetCfgEditor>();
        var targetList = new List<string>();
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
                    var unityPath = ABBUtil.GetUnityPathByFullPath(fullPath);
                    var targetType = $"{item.Value}{name}";
                    var index = targetList.Count;
                    targetList.Add(targetType);

                    itemList.Add(new()
                    {
                        enName = targetType,
                        nAssetID = index,
                        strPath = unityPath,
                    });
                }
            }
        }

        WriteEnLoadTargetFile(itemList);
    }

    private static void WriteEnLoadTargetFile(List<AssetCfgEditor> data)
    {
        var catalogObj = GameSchedule.ReadCfg("CfgCatalog", typeof(ExportExcelInfo));
        var catalogList = catalogObj as ExportExcelInfo[];
        var assetCatalog = Array.Find<ExportExcelInfo>(catalogList, (item) => item.excelInfo.excelName == "AssetCfg");
        var excelInfo = assetCatalog.excelInfo;

        var excelPath = Path.Combine(ABBUtil.GetUnityRootPath(), "Misc", "Excel", "AssetCfg.xlsx");
        var assetCfg = ExcelUtil.ReadExcel(excelPath);
        var workbook = assetCfg.Workbook;
        var worksheetCount = workbook.Worksheets.Count();
        var worksheet = workbook.Worksheets[1];
        var assetList = GameSchedule.ReadCfg<AssetCfg>();
        var path2Index = new Dictionary<string, int>();
        var changeList = new Dictionary<int, int>();
        var changeList2 = new Dictionary<int, int>();
        var maxIndex = 1;
        var enumStr = new StringBuilder();
        enumStr.AppendLine($"public enum EnLoadTarget");
        enumStr.AppendLine($"{{");
        enumStr.AppendLine($"\tNone = 0,");
        for (int i = 0; i < assetList.Length; i++)
        {
            var item = assetList[i];
            maxIndex = Mathf.Max(item.nAssetID);
            path2Index.Add(item.strPath, i);
        }
        for (int i = 0; i < data.Count; i++)
        {
            var dataItem = data[i];
            if (!path2Index.TryGetValue(dataItem.strPath, out var listIndex))
                continue;
            changeList2.Add(i, listIndex);
            var cfgData = assetList[listIndex];
            if (cfgData.strPath == dataItem.strPath)
            {
                dataItem.nAssetID = cfgData.nAssetID;
                if (cfgData.enName == dataItem.enName)
                {
                    continue;
                }
            }
            changeList.Add(listIndex, i);
        }

        for (int i = 0; i < assetList.Length; i++)
        {
            var ass = assetList[i];
            object item = ass;
            if (changeList.TryGetValue(i, out var dataItem))
            {
                var dataAss = data[dataItem];
                item = dataAss;
                enumStr.AppendLine($"\t{dataAss.enName} = {dataAss.nAssetID},");
            }
            else
            {
                enumStr.AppendLine($"\t{ass.enName} = {ass.nAssetID},");
            }
            var row = excelInfo.dataStartRow + i;
            foreach (var file2Col in assetCatalog.field2ColList)
            {
                var fieldInfo = item.GetType().GetField(file2Col.Key, BindingFlags.Public | BindingFlags.Instance);
                var value = fieldInfo.GetValue(item);
                worksheet.SetValue(row, file2Col.Value, value);
            }
        }
        var startRowIndex = assetList.Length + excelInfo.dataStartRow;
        var count = 0;
        for (int i = 0; i < data.Count; i++)
        {
            if (changeList2.ContainsKey(i))
                continue;
            var dataItem = data[i];
            dataItem.nAssetID = maxIndex + i + 1;
            var row = startRowIndex + count;
            worksheet.SetValue(row, 1, "*");
            foreach (var file2Col in assetCatalog.field2ColList)
            {
                var fieldInfo = dataItem.GetType().GetField(file2Col.Key, BindingFlags.Public | BindingFlags.Instance);
                var value = fieldInfo.GetValue(dataItem);
                worksheet.SetValue(row, file2Col.Value, value);
            }
            enumStr.AppendLine($"\t{dataItem.enName} = {dataItem.nAssetID},");
            count++;
        }

        enumStr.AppendLine("}");
        File.WriteAllText(m_EnLoadTargetPath, enumStr.ToString());
        assetCfg.Save();
        assetCfg.Dispose();
        AssetDatabase.Refresh();
    }

    public class AssetCfgEditor
    {
        // id
        public System.Int32 nAssetID;
        // 枚举名称
        public System.String enName;
        // 路径
        public System.String strPath;
    }
}

