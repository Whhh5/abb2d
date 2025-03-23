using System.IO;
using Unity.Entities.Serialization;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using Unity.Entities.Content;
using UnityEngine.Profiling;

public class ToolsMenu
{
    [MenuItem("Tools/Monster Debug Window", priority = 101)]
    public static void OpenMonsterDebugWindow()
    {
        var widnow = EditorWindow.GetWindow<MonsterDebugWindow>();
        widnow.Show();
    }
    [MenuItem("Tools/Cfg Editor Window", priority = 102)]
    public static void OpenCfgEditorWindow()
    {
        var widnow = EditorWindow.GetWindow<CfgEditorWindow>();
        widnow.Show();
    }
    [UnityEditor.MenuItem("Tools/CreateWeakObjectSO")]
    public static void ExecuteCreateWeakObjectSO()
    {
        //var dir = Path.Combine("Assets", "Abbresources", "SO");
        //var soName = $"WeakObjectSO.asset";
        //var folderPath = "Assets/Abbresources/EcsPrefab";

        //var newSO = ScriptableObject.CreateInstance<CreateWeakObjectSO>();

        //var folderGuids = AssetDatabase.FindAssets("", new[] { folderPath });
        //for (int i = 0; i < folderGuids.Length; i++)
        //{
        //    var assPath = AssetDatabase.GUIDToAssetPath(folderGuids[i]);
        //    var ass = AssetDatabase.LoadAssetAtPath<Object>(assPath);
        //    var assName = ass.name;
        //    var weakObjectID = UntypedWeakReferenceId.CreateFromObjectInstance(ass);

        //    newSO.AddEcsObject(assName, ref weakObjectID);
        //}

        //var path = Path.Combine(dir, soName);
        //AssetDatabase.CreateAsset(newSO, path);

        //var guid = AssetDatabase.GUIDFromAssetPath(path);
        //AssetDatabase.SaveAssetIfDirty(guid);
    }
    [MenuItem("Tools/Clean Editor Memory", priority = 201)]
    static void CleanEditorMemory()
    {
        // 获取总内存使用量
        long totalMemory = Profiler.GetTotalReservedMemoryLong();
        // 获取已使用的堆内存
        long usedHeapMemory = Profiler.GetTotalAllocatedMemoryLong();
        // 获取未使用的堆内存
        long unusedHeapMemory = Profiler.GetTotalUnusedReservedMemoryLong();

        ExcelUtil.ClearCache();
        AssetDatabase.Refresh();
        Caching.ClearCache();
        // 立即卸载未使用的资源
        EditorUtility.UnloadUnusedAssetsImmediate();
        // 强制进行垃圾回收
        System.GC.Collect();

        var usedHeapMemory2 = Profiler.GetTotalAllocatedMemoryLong();
        long unusedHeapMemory2 = Profiler.GetTotalUnusedReservedMemoryLong();
        var clearValue = (float)(usedHeapMemory - usedHeapMemory2) / 1024 / 1024;
        var unusedValue = (float)unusedHeapMemory2 / 1024 / 1024;
        var totalValue = (float)totalMemory / 1024 / 1024;
        Debug.LogWarning($"Editor memory has been cleaned. {clearValue:0.0}MB, {unusedValue:0}/{totalValue:0}MB");
    }
}
