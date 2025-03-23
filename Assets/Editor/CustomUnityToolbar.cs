
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System;
using UnityEngine.Playables;
using System.Diagnostics;
using System.IO;
using static PlasticPipe.PlasticProtocol.Messages.NegotiationCommand;
using Unity.Plastic.Newtonsoft.Json;
using Unity.Entities;
using Unity.Entities.Conversion;
//using Unity.Entities.Serialization;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities.UniversalDelegates;
#if UNITY_2019_1_OR_NEWER
using UnityEngine.UIElements;
#else
using UnityEngine.Experimental.UIElements;
#endif

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
        if (GUILayout.Button("Create Blob Asset"))
        {
            //CreateBlobAsset();
        }
        if (GUILayout.Button("Update Load Target"))
        {
            LoadConfigEditor.CreateLoadConfigJson();
        }
        if (GUILayout.Button("Export Excel"))
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
        GUILayout.EndHorizontal();
    }

    private static bool m_IsCreateEnemy = false;
    private class ClipCfg2
    {
        // id
        public System.Int32 nClipID;
        // 剪辑名字
        public System.String strName;
        // 长度
        public System.Single fLength;
        // 资源ID
        public System.Int32 nAssetCfgID;
    }
    /// <summary>
    /// 绘制右侧的元素
    /// </summary>
    private static void GUIRight()
    {
        GUILayout.BeginHorizontal();
        var str = m_IsCreateEnemy ? "启用" : "禁用";
        if (GUILayout.Button($"创建怪物({str})"))
        {
            m_IsCreateEnemy = !m_IsCreateEnemy;
            if (m_IsCreateEnemy)
            {
                ABBInputMgr.Instance.AddListanerDown(KeyCode.Mouse0, OnClickMouse0);
            }
            else
            {
                ABBInputMgr.Instance.RemoveListanerDown(KeyCode.Mouse0, OnClickMouse0);
            }
        }
        if (GUILayout.Button("Camera"))
        {

        }
        if (GUILayout.Button("Level1"))
        {
            var cfg22 = new ClipCfg2()
            {
                nClipID = 1,
                strName = "111",
                fLength = 2.33f,
                nAssetCfgID = 1,
            };
            var arrCfg = new ClipCfg2[] { cfg22, cfg22 };
            var json = JsonConvert.SerializeObject(arrCfg);
            var str2 = "{\"nClipID\":1,\"strName\":\"111\",\"fLength\":2.33,\"nAssetCfgID\":1}";
            //var str4 = "{\"nClipID\":1,\"strName\":\"111\",\"fLength\":2.33,\"nAssetCfgID\":1}";
            //var str5 = "[{\"nClipID\":1,\"strName\":\"111\",\"fLength\":2.33,\"nAssetCfgID\":1},{\"nClipID\":1,\"strName\":\"111\",\"fLength\":2.33,\"nAssetCfgID\":1}]";
            //var str6 = "[{\"nClipID\":1,\"strName\":\"111\",\"fLength\":2.33,\"nAssetCfgID\":1}]";

            var str3 = JsonConvert.DeserializeObject(json, typeof(ClipCfg[]));
            var cfg = JsonConvert.DeserializeObject<ClipCfg2>(str2);
        }
        GUILayout.EndHorizontal();
    }
    private static void OnClickMouse0()
    {
        var pos = CameraMgr.Instance.GetCameraWorldPos();
        var dir = CameraMgr.Instance.GetCameraForward();
        var camera = CameraMgr.Instance.GetMainCamera();

        var ray = camera.ScreenPointToRay(Input.mousePosition);
        var hit = Physics.RaycastAll(ray);
        if (hit.Length > 0)
        {
            MonsterMgr.Instance.CreateMonster(1, hit[0].point + Vector3.up * 2);
        }
    }

    //public static Entity ConvertGameObjectToEntity(GameObject prefab)
    //{
    //    //Unity.Entities.Conversion.LiveConversionSettings.
    //    var entityMgr = World.DefaultGameObjectInjectionWorld.EntityManager;
    //    var entity = entityMgr.CreateEntity(typeof(AttackComponent));
    //    entityMgr.GetComponentTypes(entity, Allocator.Temp);
    //    entityMgr.SetComponentData<AttackComponent>(entity, new AttackComponent()
    //    {
    //        faction = EnFaction.Zombie,
    //        timer = 1,
    //        timerMax = 2,
    //    });

    //    return entity;
    //}
    //private unsafe static void CreateBlobAsset()
    //{
    //    var testObj = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Scenes/Ecs Sample/UnitEntity.prefab");

    //    var blobBuilder = new BlobBuilder(Allocator.Temp);
    //    ref var blobData = ref blobBuilder.ConstructRoot<MyBlobData>();
    //    var builderArray = blobBuilder.Allocate(ref blobData.IntArray, 5);
    //    var entityArray = blobBuilder.Allocate(ref blobData.entityArray, 1);
    //    entityArray[0] = new AttackComponent()
    //    {
    //        faction = EnFaction.Zombie,
    //        timer = 1,
    //        timerMax = 2,
    //    };

    //    for (int i = 0; i < 5; i++)
    //    {
    //        builderArray[i] = i;
    //    }
    //    blobBuilder.AllocateString(ref blobData.StrValue, "sjbaduagosdoa");
    //    var blobRef = blobBuilder.CreateBlobAssetReference<MyBlobData>(Allocator.Temp);
    //    //var legnth = blobBuilder.
    //    blobBuilder.Dispose();


    //    var str = blobBuilder.ToString();
    //    //var strByte = str

    //    int dataLength = UnsafeUtility.SizeOf<MyBlobData>();
    //    var intSize = UnsafeUtility.SizeOf<int>();
    //    //var strSize = UnsafeUtility.SizeOf<BlobString>();
    //    dataLength += builderArray.Length * intSize;// + strSize;

    //    var filePath = Path.Combine(ABBUtil.GetUnityRootPath(), "Misc/AssetBlob.bin");
    //    unsafe
    //    {
    //        var dataPtr = (byte*)blobRef.GetUnsafePtr();

    //        using (var writer = new BinaryWriter(File.Open(filePath, FileMode.OpenOrCreate)))
    //        {
    //            writer.Write(dataLength);

    //            for (int i = 0; i < dataLength; i++)
    //            {
    //                writer.Write(dataPtr[i]);
    //            }
    //        }
    //    }
    //    blobRef.Dispose();
    //    //BlobAssetSerializeExtensions.Write(new Unity.Entities.Serialization.StreamBinaryWriter())


    //    using (var reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
    //    {
    //        var length = reader.ReadInt32();

    //        byte* dataPtr = (byte*)UnsafeUtility.Malloc(length, 1, Allocator.Temp);
    //        for (int i = 0; i < length; i++)
    //        {
    //            dataPtr[i] = reader.ReadByte();
    //        }

    //        var blobAsset = BlobAssetReference<MyBlobData>.Create((void*)dataPtr, length);

    //        for (int i = 0; i < blobAsset.Value.IntArray.Length; i++)
    //        {
    //            EcsUtil.DebugLog(blobAsset.Value.IntArray[i]);
    //        }
    //        EcsUtil.DebugLog(blobAsset.Value.StrValue.Length);
    //        EcsUtil.DebugLog(blobAsset.Value.entityArray[0].faction);
    //    }
    //}
}
//public struct MyBlobData
//{
//    public BlobArray<int> IntArray;
//    public BlobString StrValue;
//    public BlobArray<AttackComponent> entityArray;
//}
//public class BaseType
//{

//}
//public class ChildType : BaseType
//{

//}