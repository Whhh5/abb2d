using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public enum EnManagerFuncType
{
    None = 0,
    AwakeAsync = 1 << 0,
    OnEnableAsync = 1 << 1,
    Update = 1 << 2,
}
public enum EnGameStatus
{
    Start,
    Playing,
    Pause,

}


public class GameManager : MonoBehaviour
{
    private int m_GameManagerGoID = -1;
    #region 生命周期管理
    private EnManagerFuncType m_CurInitSatge = EnManagerFuncType.None;
    private Dictionary<EnManagerFuncType, HashSet<Singleton>> m_ManagerList = new();
    private void SubManager(Singleton f_Manager)
    {
        if(f_Manager.GetType() == Type.GetType("ABBInputMgr"))
        {

        }
        for (int i = 0; i < 3; i++)
        {
            var type = (EnManagerFuncType)(1 << i);
            if ((f_Manager.FuncType & type) != type)
                continue;
            if (!m_ManagerList.TryGetValue(type, out var list))
            {
                list = new();
                m_ManagerList.Add(type, list);
            }
            list.Add(f_Manager);
        }
    }
    private void Register()
    {
        Assembly[] hotUpdateAsss = AppDomain.CurrentDomain.GetAssemblies();
        var parentType = typeof(Singleton<>);
        var instanceStr = "Instance";
        foreach (var assembly in hotUpdateAsss)
        {
            //if (assembly.GetName().Name != "AbbFramework")
            //    continue;
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (!typeof(Singleton).IsAssignableFrom(type))
                    continue;
                if (parentType == type)
                    continue;
                if (typeof(Singleton) == type)
                    continue;
                var specificParentType = parentType.MakeGenericType(type);
                var instanceField = specificParentType.GetField(instanceStr, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                var instance = instanceField.GetValue(null);
                var monoBehaviour = instance as Singleton;
                SubManager(monoBehaviour);
                Debug.Log($"Register ======> {type}");
            }
        }
    }

    private void Awake()
    {
        GameObject.DontDestroyOnLoad(this);
        Register();
    }
    private async void Start()
    {
        Debug.Log(" --------------------- start Awake -----------------------");
        // ���� awake
        if (m_ManagerList.TryGetValue(EnManagerFuncType.AwakeAsync, out var list))
        {
            var count = list.Count;
            UniTask[] tasks = new UniTask[count];
            var index = 0;
            foreach (var item in list)
            {
                tasks[index++] = UniTask.Create(async () =>
                {
                    await item.AwakeAsync();
                });
            }
            await UniTask.WhenAll(tasks);
        }
        m_CurInitSatge |= EnManagerFuncType.AwakeAsync;
        Debug.Log(" --------------------- ��ʼ�� Awake end -----------------------");


        // ���� start
        Debug.Log(" --------------------- start Start -----------------------");
        if (m_ManagerList.TryGetValue(EnManagerFuncType.OnEnableAsync, out list))
        {
            var count = list.Count;
            UniTask[] tasks = new UniTask[count];
            var index = 0;
            foreach (var item in list)
            {
                tasks[index++] = UniTask.Create(async () =>
                {
                    await item.OnEnableAsync();
                });
            }
            await UniTask.WhenAll(tasks);
        }
        m_CurInitSatge |= EnManagerFuncType.OnEnableAsync;
        Debug.Log(" ---------------------  Start end -----------------------");


        m_GameManagerGoID = await ABBGOMgr.Instance.CreateGOAsync(EnLoadTarget.Pre_GameManager, null);
    }
    private void OnDestroy()
    {

    }
    private void Update()
    {
        if ((m_CurInitSatge & EnManagerFuncType.OnEnableAsync) == EnManagerFuncType.None)
        {
            return;
        }
        // ���� update
        if (m_ManagerList.TryGetValue(EnManagerFuncType.Update, out var list))
        {
            foreach (var item in list)
            {
                item.Update();
            }
        }
    }

    private void OnApplicationQuit()
    {
        // ֹͣ start
        Debug.Log(" --------------------- ֹͣ OnDisable -----------------------");
        m_CurInitSatge &= ~EnManagerFuncType.OnEnableAsync;
        if (m_ManagerList.TryGetValue(EnManagerFuncType.OnEnableAsync, out var list))
        {
            foreach (var item in list)
            {
                item.OnDisable();
            }
        }
        Debug.Log(" --------------------- ֹͣ OnDisable ��� -----------------------");
        // ֹͣ awake
        Debug.Log(" --------------------- ֹͣ Destroy -----------------------");
        m_CurInitSatge &= ~EnManagerFuncType.AwakeAsync;
        if (m_ManagerList.TryGetValue(EnManagerFuncType.AwakeAsync, out list))
        {
            foreach (var item in list)
            {
                item.Destroy();
            }
        }
        Debug.Log(" --------------------- ֹͣ Destroy ��� -----------------------");
        //ABBGOMgr.Instance.DestroyGO(m_GameManagerGoID);
        m_GameManagerGoID = -1;
    }
    #endregion
}
