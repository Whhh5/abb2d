using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public sealed class GameClassPoolMgr : Singleton<GameClassPoolMgr>
{
    private Dictionary<Type, GameClassPoolData> m_DicClassPool = new();
    private GameClassPoolData GetClassPoolData(Type type)
    {
        if (!m_DicClassPool.TryGetValue(type, out var poolData))
        {
            poolData = new GameClassPoolData();
            m_DicClassPool.Add(type, poolData);
        }
        return poolData;
    }

    public T Pull<T>()
        where T : class, IGamePool, new()
    {
        var type = typeof(T);
        var poolData = GetClassPoolData(type);
        if (!poolData.TryPull(out var data))
        {
            data = new T();
            data.PoolConstructor();
        }
        data.OnPoolGet();
        return data as T;
    }
    public void Push<T>(T classData)
        where T : class, IGamePool
    {
        var type = classData.GetType();
        classData.OnPoolRecycle();
        var poolData = GetClassPoolData(type);
        poolData.Push(classData);
    }
    
}

