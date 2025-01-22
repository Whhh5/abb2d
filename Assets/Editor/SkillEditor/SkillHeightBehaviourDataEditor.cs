using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SkillHeightBehaviourDataEditor : SkillHeightBehaviourData, ISkillBehaviourEditor
{
    public float _ValueEditor;
    public void InitEditor()
    {
        _ValueEditor = _ArrValue[0] / 100f;
    }
    public void Draw()
    {
        _ValueEditor = EditorGUILayout.FloatField(_ValueEditor, GUILayout.Width(100));
    }

    public void GetStringData(ref List<int> data)
    {
        data.Add(Mathf.RoundToInt(_ValueEditor * 100));
    }
}