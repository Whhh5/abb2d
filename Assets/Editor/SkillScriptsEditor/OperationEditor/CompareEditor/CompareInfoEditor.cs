using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



public class CompareLessInfoEditor : CompareLessInfo
{

}
public class CompareEqualInfoEditor : CompareEqualInfo
{

}
public class CompareGreaterInfoEditor : CompareGreaterInfo
{

}
public abstract class CompareInfoEditor : CompareInfo, ISkillTypeEditor
{
    private float _Value = -1;
    public void InitEditor()
    {
        _Value = value / 100f;
    }

    public void GetStringData(ref List<int> data)
    {
        var headIndex = data.Count;
        data.Add(Mathf.RoundToInt(_Value * 100));
        data.Insert(headIndex, data.Count - headIndex);
    }

    public void Draw()
    {
        EditorGUILayout.BeginHorizontal();
        {
            var typeName = EditorUtil.GetEnumName(GetCompareType());

            EditorGUILayout.LabelField("input", GUILayout.Width(50));
            EditorGUILayout.TextField($"{typeName}", GUILayout.Width(30));

            _Value = EditorGUILayout.FloatField(_Value, GUILayout.Width(50));
        }
        EditorGUILayout.EndHorizontal();

    }


    public static List<(GUIContent title, Func<SkillTypeSelectItemInfoEditor> click)> GetOperationCompareMenu(string rootMenu)
    {
        var result = new List<(GUIContent, Func<SkillTypeSelectItemInfoEditor>)>();
        for (var i = EnOperationCompareType.None + 1; i < EnOperationCompareType.EnumCount; i++)
        {
            var compareType = i;
            var name = EditorUtil.GetEnumName(compareType);
            var con = new GUIContent()
            {
                text = $"{rootMenu}/{name}",
                tooltip = "adasdasda",
            };
            result.Add((con, () =>
            {
                var item = MenuOperationCompareClick(compareType);
                return item;
            }
            ));
        }
        return result;
    }
    public static SkillTypeSelectItemInfoEditor MenuOperationCompareClick(EnOperationCompareType compareType)
    {
        Debug.LogError(compareType);
        return null;
    }
}