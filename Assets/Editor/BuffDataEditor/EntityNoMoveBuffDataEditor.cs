using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[EditorFieldName("禁止移动")]
public class EntityNoMoveBuffDataEditor : EntityNoMovementBuffData, IBuffDaraEditor
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


[EditorFieldName("player buff")]
public class EntityPlayerBuffBuffDataEditor : EntityPlayerBuffBuffData, IBuffDaraEditor
{
    public EnBuff Buff => EnBuff.PlayerBuff;
    private float _Time = 0;
    public void InitParams(int[] arrParam)
    {
        arrParam ??= new int[0];
        var count = arrParam.Length > 0 ? arrParam[0] : 0;
        _Time = count > 0 ? arrParam[1] / 100f : 1f;
    }

    public void Draw()
    {
        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("持续时间", GUILayout.Width(50));
                _Time = EditorGUILayout.FloatField(_Time, GUILayout.Width(50));
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
    }

    public void GetStringData(ref List<int> data)
    {
        var index = data.Count;
        data.Add(Mathf.RoundToInt(_Time * 100));
        data.Insert(index, data.Count - index);
    }

    public void InitEditor()
    {
    }

}


public class EntityPoisonBuffDataEditor : EntityPoisonBuffData, IBuffDaraEditor
{
    public EnBuff Buff => EnBuff.Poison;
    private float _Time = 0;
    public void InitParams(int[] arrParam)
    {
        arrParam ??= new int[0];
        var count = arrParam.Length > 0 ? arrParam[0] : 0;
        _Time = count > 0 ? arrParam[1] / 100f : 1f;
    }

    public void Draw()
    {
        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("持续时间", GUILayout.Width(50));
                _Time = EditorGUILayout.FloatField(_Time, GUILayout.Width(50));
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
    }

    public void GetStringData(ref List<int> data)
    {
        var index = data.Count;
        data.Add(Mathf.RoundToInt(_Time * 100));
        data.Insert(index, data.Count - index);
    }

    public void InitEditor()
    {
    }

}
public class EntityPlayerSkill2BuffDataEditor : EntityPlayerSkill2BuffData, IBuffDaraEditor
{
    public EnBuff Buff => EnBuff.PlayerSkill2;
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
public class EntityExpiosionBuffDataEditor : EntityExpiosionBuffData, IBuffDaraEditor
{
    public EnBuff Buff => EnBuff.Expiosion;
    private float _Time = 0;
    public void InitParams(int[] arrParam)
    {
        arrParam ??= new int[0];
        var count = arrParam.Length > 0 ? arrParam[0] : 0;
        _Time = count > 0 ? arrParam[1] / 100f : 1f;
    }

    public void Draw()
    {
        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("持续时间", GUILayout.Width(50));
                _Time = EditorGUILayout.FloatField(_Time, GUILayout.Width(50));
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
    }

    public void GetStringData(ref List<int> data)
    {
        var index = data.Count;
        data.Add(Mathf.RoundToInt(_Time * 100));
        data.Insert(index, data.Count - index);
    }

    public void InitEditor()
    {
    }

}
public class EntityExpiosion2BuffDataEditor : EntityExpiosion2BuffData, IBuffDaraEditor
{
    public EnBuff Buff => EnBuff.Expiosion2;
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