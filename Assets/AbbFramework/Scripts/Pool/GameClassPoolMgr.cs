using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public sealed class GameClassPoolMgr : Singleton<GameClassPoolMgr>
{
    private Dictionary<Type, object> m_DicClassPool = new();
    private GameClassPoolData<T> GetClassPoolData<T>(Type type)
        where T: class, IGamePool
    {
        if (!m_DicClassPool.TryGetValue(type, out var poolData))
        {
            poolData = new GameClassPoolData<T>();
            m_DicClassPool.Add(type, poolData);
        }
        return poolData as GameClassPoolData<T>;
    }

    public T Pull<T>()
        where T : class, IGamePool, new()
    {
        var type = typeof(T);
        var poolData = GetClassPoolData<T>(type);
        if (!poolData.TryPull(out var data))
        {
            data = new T();
            data.PoolConstructor();
        }
        data.OnPoolGet();
        return data;
    }
    public void Push<T>(T classData)
        where T : class, IGamePool
    {
        var type = typeof(T);
        classData.OnPoolRecycle();
        var poolData = GetClassPoolData<T>(type);
        poolData.Push(classData);
    }
    
}

