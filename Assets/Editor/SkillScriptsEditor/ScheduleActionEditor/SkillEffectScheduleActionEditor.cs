
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SkillEffectScheduleActionEditor : SkillEffectScheduleAction, ISkillScheduleActionEditor
{
    private List<int> m_TempParams = new(new int[2]);
    public void InitEditor()
    {
        m_TempParams[0] = effectParams?.Length > 0 ? effectParams[0] : 0;
        m_TempParams[1] = effectParams?.Length > 1 ? effectParams[1] : 0;
    }
    public void Draw()
    {
        EditorGUILayout.BeginVertical();
        {
            effectID = EditorGUILayout.IntField("特效ID", effectID, GUILayout.Width(300));
            schedule = EditorGUILayout.Slider("进度", schedule, 0, 1, GUILayout.Width(300));
            m_TempParams[0] = Mathf.RoundToInt(EditorGUILayout.FloatField("飞行时间", m_TempParams[0] * 0.01f, GUILayout.Width(300)) * 100);
            m_TempParams[1] = Mathf.RoundToInt(EditorGUILayout.FloatField("飞行距离", m_TempParams[1] * 0.01f, GUILayout.Width(300)) * 100);
            //atkValue = EditorGUILayout.IntField("伤害值", atkValue, GUILayout.Width(300));
            //(physicsResolve as IEditorDataInit).InitEditor();
        }
        EditorGUILayout.EndVertical();
    }

    public void GetStringData(ref List<int> data)
    {
        var index = data.Count;
        data.Add(effectID);
        data.Add(Mathf.RoundToInt(schedule * 100));
        data.Insert(index, data.Count - index);

        data.Add(m_TempParams.Count);
        data.AddRange(m_TempParams);
    }
}