using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameClassPoolData<T> : ClassData
    where T : class, IGamePool
{
    //public static EnClassType ClassType => EnClassType.GameClassPoolData;
    private int m_UseCount = 0;
    private List<T> m_ListClass = new(GlobalConfig.Int5);

    public void Push(T classType)
    {
        m_ListClass.Add(classType);
    }
    public bool TryPull(out T result)
    {
        result = null;
        if (m_ListClass.Count == 0)
            return false;

        result = m_ListClass[^1];
        m_ListClass.RemoveAt(m_ListClass.Count - 1);
        return true;
    }
}

