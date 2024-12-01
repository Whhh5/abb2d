using UnityEngine;
using System.Collections;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System;

public class GamePoolData
{
    private Type m_Type = null;
    private int m_Count = 0;
    public int Count => m_Count;
    private List<IGamePool> m_List = new(10);
    private int m_TotalCount = 0;
    public int TotalCount => m_TotalCount;
    public void SetType(Type type)
    {
        m_Type = type;
    }
    public void Clear()
    {
        foreach (var item in m_List)
        {
            item.PoolRelease();
        }
        m_List.Clear();
        m_TotalCount = 0;
        m_Count = 0;
        m_Type = null;
    }
    public T Get<T>()
        where T : class, IGamePool, new()
    {
        m_TotalCount++;
        if (m_Count == 0)
        {
            var obj = new T();
            obj.PoolConstructor();
            return obj;
        }
        else
        {
            var index = m_Count - 1;
            var value = m_List[index];
#if UNITY_EDITOR
            if (m_Type is not T)
            {
                ABBUtil.LogError($"recycle type no consistency, {m_Type} -> {typeof(T)}");
                return null;
            }
#endif
            m_List[index] = null;
            m_Count--;
            return value as T;
        }
    }
    public void Recycle(IGamePool obj)
    {
#if UNITY_EDITOR
        var objType = obj.GetType();
        if (objType != m_Type)
        {
            ABBUtil.LogError($"recycle type no consistency, {m_Type} -> {objType}");
            return;
        }
#endif
        m_List.Add(obj);
        m_Count++;
        m_TotalCount--;
    }
}
public class GameUtil : Singleton<GameUtil>
{
    private static Dictionary<Type, GamePoolData> m_ClassPool = new();
    private static List<GamePoolData> m_GamePoolList = new(10);
    public static T GetClass<T>()
        where T : class, IGamePool, new()
    {
        var type = typeof(T);
        if (!m_ClassPool.TryGetValue(type, out var list))
        {
            list = m_GamePoolList.Count == 0
                    ? new()
                    : m_GamePoolList[^1];
            list.SetType(type);
            m_ClassPool.Add(type, list);
        }
        var result = list.Get<T>();
        result.OnPoolGet();
        return result;
    }
    public static void RecycleClass<T>(T classData)
        where T : class, IGamePool
    {
        var type = classData.GetType();
        if (!m_ClassPool.TryGetValue(type, out var list))
        {
            ABBUtil.LogError($"recycle type no exist, {type}");
            return;
        }
        classData.OnPoolRecycle();
        list.Recycle(classData);
        if (list.TotalCount == 0)
        {
            list.Clear();
            m_ClassPool.Remove(type);
            m_GamePoolList.Add(list);
        }
    }
}

