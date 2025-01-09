using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public sealed class ClassPoolMgr : Singleton<ClassPoolMgr>
{
    private Dictionary<Type, ClassPoolData> m_DicClassPool = new();
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
        where T : class, IClassPool, new()
    {
        var data = Pull<T>(null);
        return data;
    }
    public T Pull<T>(IClassPoolUserData userData)
        where T : class, IClassPool, new()
    {
        var type = typeof(T);
        var poolData = GetClassPoolData(type);
        if (!poolData.TryPull(out var data))
        {
            data = new T();
            data.PoolConstructor();
        }
        data.OnPoolInit(userData);
        return data as T;
    }
    public void Push<T>(T classData)
        where T : class, IClassPool
    {
        var type = classData.GetType();
        classData.OnPoolDestroy();
        var poolData = GetClassPoolData(type);
        poolData.Push(classData);
    }
    
}

