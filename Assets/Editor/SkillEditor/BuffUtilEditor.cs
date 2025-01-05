using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class BuffUtilEditor
{
    public static IBuffDaraEditor GetBuffDataEditor(EnBuff buff)
    {
        IBuffDaraEditor buffData = buff switch
        {
            EnBuff.MoveDown => new EntityMoveDownBuffDataEditor(),
            EnBuff.NoGravity => new EntityNoGravityBuffDataEditor(),
            EnBuff.NoJump => new EntityNoJumpBuffDataEditor(),
            EnBuff.NoMove => new EntityNoMoveBuffDataEditor(),
            EnBuff.NoRotation => new EntityNoRotationBuffDataEditor(),
            _ => null,
        };
        buffData.Buff = buff;
        return buffData;
    }
}

public interface IBuffDaraEditor : IEditorItem, IEditorItemInit
{
    public EnBuff Buff { get; set; }
}
[EditorFieldName("速度增量")]
public class EntityMoveDownBuffDataEditor : IBuffDaraEditor
{
    [EditorFieldName("增量值(0-1)")]
    public float value;

    public EnBuff Buff { get; set; }
    public void InitParams(int[] arrParam)
    {
        value = (arrParam?[0] ?? 0) / 100f;
    }

    public void Draw()
    {
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("增量值", GUILayout.Width(40));
            value = EditorGUILayout.FloatField(value, GUILayout.Width(200));
        }
        EditorGUILayout.EndHorizontal();
    }

    public void GetStringData(ref List<int> data)
    {
        data.Add(Mathf.RoundToInt(value * 100));
    }

    public void InitEditor()
    {

    }
}


[EditorFieldName("忽略质量")]
public class EntityNoGravityBuffDataEditor : IBuffDaraEditor
{
    public EnBuff Buff { get; set; }
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
[EditorFieldName("禁止跳跃")]
public class EntityNoJumpBuffDataEditor : IBuffDaraEditor
{
    public EnBuff Buff { get; set; }
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
[EditorFieldName("禁止移动")]
public class EntityNoMoveBuffDataEditor : IBuffDaraEditor
{
    public EnBuff Buff { get; set; }
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
[EditorFieldName("禁止旋转")]
public class EntityNoRotationBuffDataEditor : IBuffDaraEditor
{
    public EnBuff Buff { get; set; }
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