using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[EditorFieldName("速度增量")]
public class EntityMoveDownBuffDataEditor : IBuffDaraEditor
{
    [EditorFieldName("增量值(0-1)")]
    public float value;

    public EnBuff Buff => EnBuff.MovingChanges;
    public void InitParams(int[] arrParam)
    {
        value = (arrParam?[0] ?? 0) / 100f;
    }

    public void Draw()
    {
        EditorGUILayout.BeginHorizontal(GUILayout.Width(100));
        {
            EditorGUILayout.LabelField("增量值", GUILayout.Width(40));
            value = EditorGUILayout.FloatField(value, GUILayout.Width(50));
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