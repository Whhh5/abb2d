using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[EditorFieldName("禁止移动")]
public class EntityNoMoveBuffDataEditor : IBuffDaraEditor
{
    public EnBuff Buff => EnBuff.NoMovement;
    public void InitParams(int[] arrParam)
    {
    }

    public void Draw()
    {
    }

    public void GetStringData(ref List<int> data)
    {
        var index = data.Count;
        data.Insert(index, data.Count - index);
    }

    public void InitEditor()
    {
    }

}