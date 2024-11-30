using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public enum EManagerFuncType
{
    None = 0,
    AwakeAsync = 1 << 0,
    OnEnableAsync = 1 << 1,
    Update = 1 << 2,
}

public class GameManager : MonoBehaviour
{
    private EManagerFuncType m_CurInitSatge = EManagerFuncType.None;
    private Dictionary<EManagerFuncType, HashSet<Singleton>> m_ManagerList = new();
    private void SubManager(Singleton f_Manager)
    {
        for (int i = (int)EManagerFuncType.None; i <= (int)EManagerFuncType.Update; i++)
        {
            var type = (EManagerFuncType)i;
            if ((f_Manager.FuncType & type) == EManagerFuncType.None)
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
            if (assembly.GetName().Name != "AbbFramework")
                continue;
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
        if (m_ManagerList.TryGetValue(EManagerFuncType.AwakeAsync, out var list))
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
        m_CurInitSatge |= EManagerFuncType.AwakeAsync;
        Debug.Log(" --------------------- ��ʼ�� Awake end -----------------------");


        // ���� start
        Debug.Log(" --------------------- start Start -----------------------");
        if (m_ManagerList.TryGetValue(EManagerFuncType.OnEnableAsync, out list))
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
        m_CurInitSatge |= EManagerFuncType.OnEnableAsync;
        Debug.Log(" ---------------------  Start end -----------------------");


        //// �������������
        //ILoadPrefabAsync.LoadAsync(GManager.MainCamera);
        //// ������Ϸui
        //ILoadPrefabAsync.LoadAsync(GManager.UGUIManager);
    }
    private void OnDestroy()
    {
        // ֹͣ start
        Debug.Log(" --------------------- ֹͣ OnDisable -----------------------");
        m_CurInitSatge &= ~EManagerFuncType.OnEnableAsync;
        if (m_ManagerList.TryGetValue(EManagerFuncType.OnEnableAsync, out var list))
        {
            foreach (var item in list)
            {
                item.OnDisable();
            }
        }
        Debug.Log(" --------------------- ֹͣ OnDisable ��� -----------------------");
        // ֹͣ awake
        Debug.Log(" --------------------- ֹͣ Destroy -----------------------");
        m_CurInitSatge &= ~EManagerFuncType.AwakeAsync;
        if (m_ManagerList.TryGetValue(EManagerFuncType.AwakeAsync, out list))
        {
            foreach (var item in list)
            {
                item.Destroy();
            }
        }
        Debug.Log(" --------------------- ֹͣ Destroy ��� -----------------------");
    }
    private void Update()
    {
        if ((m_CurInitSatge & EManagerFuncType.OnEnableAsync) == EManagerFuncType.None)
        {
            return;
        }
        // ���� update
        if (m_ManagerList.TryGetValue(EManagerFuncType.Update, out var list))
        {
            foreach (var item in list)
            {
                item.Update();
            }
        }
    }




}
