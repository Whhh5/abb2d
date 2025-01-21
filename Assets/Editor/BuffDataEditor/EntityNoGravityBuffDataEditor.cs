using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[EditorFieldName("忽略质量")]
public class EntityNoGravityBuffDataEditor : IBuffDaraEditor
{
    public EnBuff Buff => EnBuff.NoGravity;
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