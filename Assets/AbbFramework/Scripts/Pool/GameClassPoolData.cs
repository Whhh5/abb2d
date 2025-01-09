using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClassPoolData : ClassData
{
    private List<IClassPool> m_ListClass = new(GlobalConfig.Int5);

    public void Push(IClassPool classType)
    {
        m_ListClass.Add(classType);
    }
    public bool TryPull(out IClassPool result)
    {
        result = null;
        if (m_ListClass.Count == 0)
            return false;

        result = m_ListClass[^1];
        m_ListClass.RemoveAt(m_ListClass.Count - 1);
        return true;
    }
}

