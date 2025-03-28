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
        arrParam ??= new int[0];
        var count = arrParam.Length > 0 ? arrParam[0] : 0;
        value = count > 0 ? arrParam[1] / 100f : 0;
    }

    public void Draw()
    {
        EditorGUILayout.BeginVertical();
        {
            var buffCfg = ExcelUtil.GetCfg<BuffCfg>((int)Buff);
            var buffType = (EnBuffType)buffCfg.nBuffType;
            EditorGUILayout.EnumPopup(buffType, GUILayout.Width(100));

            EditorGUILayout.BeginHorizontal(GUILayout.Width(100));
            {
                EditorGUILayout.LabelField("增量值", GUILayout.Width(40));
                value = EditorGUILayout.FloatField(value, GUILayout.Width(50));
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
    }

    public void GetStringData(ref List<int> data)
    {
        var count = data.Count;
        data.Add(Mathf.RoundToInt(value * 100));
        data.Insert(count, data.Count - count);
    }

    public void InitEditor()
    {
    }
}