using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[EditorFieldName("禁止跳跃")]
public class EntityNoJumpBuffDataEditor : IBuffDaraEditor
{
    public EnBuff Buff => EnBuff.NoJumping;
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