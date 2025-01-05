using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class ExportExcelInfo
{
    public ExcelInfo excelInfo;
    public string strDesc;
    public List<string> fieldList = new();
    public Dictionary<string, int> field2ColList = new();
    public List<string> fieldTypeList = new();
    public List<string> descList = new();
}
public class ExcelInfo
{
    public string excelName;
    public int dataStartRow;
    public int dataStartCol;
    public int fieldRow;
    public int fieldTypeRow;
    public int descRow;
    public HashSet<int> validCol = new();
    public List<List<int>> keysCol = new();
}
public partial class GameSchedule : Singleton<GameSchedule>
{
    public override async UniTask AwakeAsync()
    {
        await base.AwakeAsync();

        var catalog = ReadCfg("CfgCatalog", typeof(ExportExcelInfo));
        var list = catalog as ExportExcelInfo[];
        foreach (var item in list)
        {
            //var cfg = GetCfgPath(item);
            var name = item.excelInfo.excelName;
            var type = this.GetType();
            var cfgType = Type.GetType(name);
            var obj = ReadCfg(name, cfgType);
            var fieldName = $"m_{name}";
            var fileInfo = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            fileInfo.SetValue(Instance, obj);
        }

        Initialization();

    }

    public static string GetCfgPath<T>()
    {
        var path = GetCfgPath(typeof(T).ToString());
        return path;
    }
    public static string GetCfgPath(string cfgName)
    {
        var path = Path.Combine(GlobalConfig.CfgRootPath, $"{cfgName}.json");
        return path;
    }
    public static T[] ReadCfg<T>()
        where T : ICfg
    {
        var fileName = $"{typeof(T)}";
        var cfg = ReadCfg(fileName, typeof(T));
        return cfg as T[];
    }
    public static object ReadCfg(string fileName, Type type)
    {
        var path = GetCfgPath(fileName);
#if UNITY_EDITOR
        var ass = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(path);
#endif
        var arrType = Array.CreateInstance(type, 0).GetType();

        var cfg = JsonConvert.DeserializeObject(ass.text, arrType);
        return cfg;
    }
}
