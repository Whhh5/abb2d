
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine.UIElements;

/// <summary>
/// 扩展Unity的按钮栏
/// </summary>
[InitializeOnLoad]
public static class CustomUnityToolbar
{
    public static Type m_toolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
    public static ScriptableObject m_currentToolbar;

    static CustomUnityToolbar()
    {
        EditorApplication.delayCall += OnUpdate;
    }

    public static void OnUpdate()
    {
        // Relying on the fact that toolbar is ScriptableObject and gets deleted when layout changes
        if (m_currentToolbar == null)
        {
            // Find toolbar
            var toolbars = Resources.FindObjectsOfTypeAll(m_toolbarType);
            m_currentToolbar = toolbars.Length > 0 ? (ScriptableObject)toolbars[0] : null;

            if (m_currentToolbar != null)
            {
                var root = m_currentToolbar.GetType().GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
                var rawRoot = root.GetValue(m_currentToolbar);
                var mRoot = rawRoot as VisualElement;
                RegisterCallback("ToolbarZoneLeftAlign", GUILeft);
                RegisterCallback("ToolbarZoneRightAlign", GUIRight);

                void RegisterCallback(string root, Action cb)
                {
                    var toolbarZone = mRoot.Q(root);

                    var parent = new VisualElement()
                    {
                        style = {
                            flexGrow = 1,
                            flexDirection = FlexDirection.Row,
                        }
                    };
                    var container = new IMGUIContainer();
                    container.onGUIHandler += () =>
                    {
                        cb?.Invoke();
                    };
                    parent.Add(container);
                    toolbarZone.Add(parent);
                }
            }
        }

    }

    //private static BaseType baseType = new();
    /// <summary>
    /// 绘制左侧的元素
    /// </summary>
    private static void GUILeft()
    {
        GUILayout.BeginHorizontal();
        //if (GUILayout.Button("Create Blob Asset"))
        //{
        //    //CreateBlobAsset();
        //}
        if (GUILayout.Button("Update Load Target"))
        {
            LoadConfigEditor.CreateLoadConfigJson();
        }
        if (GUILayout.Button("Update Buff"))
        {
            UpdateBuff();
        }
        if (GUILayout.Button("Export Excel"))
        {
            ExportExcel();
        }
        GUILayout.EndHorizontal();
    }

    /// <summary>
    /// 绘制右侧的元素
    /// </summary>
    private static void GUIRight()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button($"Clear Cache"))
        {
            ToolsMenu.CleanEditorMemory();
        }

        GUILayout.EndHorizontal();
    }

    public static void ExportExcel()
    {
        string[] arrParams = new string[]
            {
                Path.Combine(ABBUtil.GetUnityRootPath(), "Misc", "Excel"),
                Path.Combine(ABBUtil.GetDataPath(), "Abbresources", "GameCfgJson"),
                Path.Combine(ABBUtil.GetDataPath(), "Scripts", "GameCfgCS"),
            };
        var paramsStr = "";
        for (int i = 0; i < arrParams.Length; i++)
        {
            paramsStr += arrParams[i] + " ";
        }
        var pro2 = new ProcessStartInfo()
        {
            FileName = $"/Users/qiuxiaohui/Projects/ExcelTools/ExcelTools/bin/Debug/net6.0/ExcelTools",
            RedirectStandardOutput = true, // 重定向标准输出
            UseShellExecute = false, // 不使用系统外壳程序启动
            CreateNoWindow = true, // 不创建新窗口
            Arguments = paramsStr,
            ErrorDialog = true,
            //StandardErrorEncoding = System.Text.Encoding.UTF8,
            RedirectStandardError = true,
            StandardOutputEncoding = System.Text.Encoding.UTF8,
            //RedirectStandardInput = true,
        };
        pro2.UseShellExecute = false;
        using (var process = Process.Start(pro2))
        {
            process.OutputDataReceived += (item, e) =>
            {
                if (string.IsNullOrEmpty(e.Data))
                    return;
                UnityEngine.Debug.Log($"{item},{e.Data}"); // 打印结果
            };
            process.ErrorDataReceived += (item, e) =>
            {
                if (string.IsNullOrEmpty(e.Data))
                    return;
                UnityEngine.Debug.LogError($"{item},{e.Data}"); // 打印结果
            };

            process.BeginOutputReadLine();

            process.BeginErrorReadLine();

            process.WaitForExit();
            UnityEngine.Debug.Log($"Export Excel Finish"); // 打印结果
        }
        AssetDatabase.Refresh();
    }

    private static readonly string _BuffClassPath = "Assets/AbbFramework/Scripts/EntityBuff";
    private static readonly string _BuffEnumName = "EnBuff";
    private static readonly string _BuffTypeEnumName = "EnBuffType";

    private static void UpdateBuff()
    {
        var unityPath = ABBUtil.GetUnityRootPath();
        var fileRootPath = Path.Combine(unityPath, _BuffClassPath);
        if (!Directory.Exists(fileRootPath))
            Directory.CreateDirectory(fileRootPath);


        CreateBuffData(in fileRootPath);
        CreateBuffUtil(in fileRootPath);
        CreateBuffTypeEnum(in fileRootPath);
        CreateBuffEnum(in fileRootPath);
    }
    private static void CreateBuffTypeEnum(in string rootPath)
    {
        var filePath = Path.Combine(rootPath, $"{_BuffTypeEnumName}.cs");
        var content = new StringBuilder();

        content.AppendLine($"public enum {_BuffTypeEnumName}");
        content.AppendLine($"{{");

        content.AppendLine($"\tNone = 0,");
        var buffCount = ExcelUtil.GetCfgCount<BuffTypeCfg>();
        for (int i = 0; i < buffCount; i++)
        {
            var buffTypeCfg = ExcelUtil.GetCfgByIndex<BuffTypeCfg>(i);
            content.AppendLine($"\t[{typeof(EditorFieldNameAttribute)}(\"{buffTypeCfg.strDescEditor}\")]");
            content.AppendLine($"\t{buffTypeCfg.strEnumNameEditor} = {buffTypeCfg.nTypeID},");
        }

        content.AppendLine($"}}");
        File.WriteAllText(filePath, content.ToString());
    }
    private static void CreateBuffEnum(in string rootPath)
    {
        var filePath = Path.Combine(rootPath, $"{_BuffEnumName}.cs");
        var content = new StringBuilder();

        content.AppendLine($"public enum {_BuffEnumName}");
        content.AppendLine($"{{");

        content.AppendLine($"\tNone = 0,");
        var buffCount = ExcelUtil.GetCfgCount<BuffCfg>();
        for (int i = 0; i < buffCount; i++)
        {
            var buffCfg = ExcelUtil.GetCfgByIndex<BuffCfg>(i);
            content.AppendLine($"\t[{typeof(EditorFieldNameAttribute)}(\"{buffCfg.strDescEditor}\")]");
            content.AppendLine($"\t{buffCfg.strEnumNameEditor} = {buffCfg.nBuffID},");
        }

        content.AppendLine($"}}");
        File.WriteAllText(filePath, content.ToString());
    }
    private static void CreateBuffUtil(in string rootPath)
    {
        var filePath = Path.Combine(rootPath, $"BuffUtil.cs");

        var content = new StringBuilder();

        content.AppendLine("public partial class BuffUtil");
        content.AppendLine("{");


        content.AppendLine($"\tpublic static {typeof(IEntityBuffData)} CreateBuffData({_BuffEnumName} buff, {typeof(IClassPoolUserData)} data)");
        content.AppendLine("\t{");
        content.AppendLine($"\t\treturn buff switch");
        content.AppendLine($"\t\t{{");
        var buffCount = ExcelUtil.GetCfgCount<BuffCfg>();
        for (int i = 0; i < buffCount; i++)
        {
            var buffCfg = ExcelUtil.GetCfgByIndex<BuffCfg>(i);
            content.AppendLine($"\t\t\t{_BuffEnumName}.{buffCfg.strEnumNameEditor} => {typeof(ClassPoolMgr)}.Instance.Pull<{buffCfg.strClassNameEditor}>(data),");
        }
        content.AppendLine($"\t\t\t_ => default,");
        content.AppendLine($"\t\t}};");
        content.AppendLine("\t}");


        content.AppendLine("}");


        File.WriteAllText(filePath, content.ToString());
    }
    private static void CreateBuffData(in string rootPath)
    {
        var count = ExcelUtil.GetCfgCount<BuffCfg>();
        for (int i = 0; i < count; i++)
        {
            var buffCfg = ExcelUtil.GetCfgByIndex<BuffCfg>(i);
            if (string.IsNullOrWhiteSpace(buffCfg.strClassNameEditor))
                throw new Exception($"buff id= {buffCfg.nBuffID}, className == null");
            var filePath = Path.Combine(rootPath, $"{buffCfg.strClassNameEditor}.cs");
            if (File.Exists(filePath))
                continue;
            //File.Create(filePath);
            //var file = File.Create(filePath);
            var fileContent = new StringBuilder();
            fileContent.AppendLine("using UnityEngine;");
            fileContent.AppendLine($"public class {buffCfg.strClassNameEditor} : EntityBuffData");
            fileContent.AppendLine("{");
            fileContent.AppendLine("\t");
            fileContent.AppendLine("}");
            //UnityEngine.Debug.Log($"{filePath},  {fileContent.ToString()}");
            File.WriteAllText(filePath, fileContent.ToString(), Encoding.UTF8);
        }
    }
}