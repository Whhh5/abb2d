using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public sealed class ClassPoolMgr : Singleton<ClassPoolMgr>
{
    private Dictionary<Type, ClassPoolData> m_DicClassPool = new();
    private PoolNaNUserData m_NaNUserData = new();
    private ClassPoolData GetClassPoolData(Type type)
    {
        if (!m_DicClassPool.TryGetValue(type, out var poolData))
        {
            poolData = new ClassPoolData();
            m_DicClassPool.Add(type, poolData);
        }
        return poolData;
    }

    public T Pull<T>()
        where T : class, IGamePool, new()
    {
        var data = Pull<T, PoolNaNUserData>(ref m_NaNUserData);
        return data;
    }
    public T Pull<T, TUserData>(ref TUserData userData)
        where T : class, IGamePool, new()
        where TUserData : struct, IPoolUserData
    {
        var type = typeof(T);
        var poolData = GetClassPoolData(type);
        if (!poolData.TryPull(out var data))
        {
            data = new T();
            data.PoolConstructor();
        }
        data.OnPoolInit(ref userData);
        return data as T;
    }
    public void Push<T>(T classData)
        where T : class, IGamePool
    {
        var type = classData.GetType();
        classData.OnPoolDestroy();
        var poolData = GetClassPoolData(type);
        poolData.Push(classData);
    }
    
}

