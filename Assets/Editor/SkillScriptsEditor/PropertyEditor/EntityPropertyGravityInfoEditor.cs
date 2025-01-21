using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EntityPropertyGravityInfoEditor : EntityPropertyGravityInfo
{
    private List<int> m_Params = new();
    public void InitEditor()
    {
        m_Params = new(arrParams);
    }
    public void Draw()
    {

    }
    public void GetStr(ref List<int> result)
    {
        var index = result.Count;
        result.Add((int)target);

        result.Insert(index, result.Count - index);
        result.Add(m_Params.Count);
        result.AddRange(m_Params);
    }
}