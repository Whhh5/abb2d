
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
            EditorUtil.DrawCfgField<EffectCfg>("特效ID", effectID, id => effectID = id, 300);
            schedule = EditorGUILayout.Slider("进度", schedule, 0, 1, GUILayout.Width(300));

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("偏移", GUILayout.Width(100));
                offsetX = EditorGUILayout.FloatField(offsetX, GUILayout.Width(50));
                offsetY = EditorGUILayout.FloatField(offsetY, GUILayout.Width(50));
                offsetZ = EditorGUILayout.FloatField(offsetZ, GUILayout.Width(50));
            }
            EditorGUILayout.EndHorizontal(); 

            traceType = (EnEffectTraceType)EditorGUILayout.EnumPopup(traceType, GUILayout.Width(100));
            bindingType = (EnEffectBindingType)EditorGUILayout.EnumPopup(bindingType, GUILayout.Width(100));
        }
        EditorGUILayout.EndVertical();
    }

    public void GetStringData(ref List<int> data)
    {
        var index = data.Count;
        data.Add(effectID);
        data.Add(Mathf.RoundToInt(schedule * 100));
        data.Add(Mathf.RoundToInt(offsetX * 100));
        data.Add(Mathf.RoundToInt(offsetY * 100));
        data.Add(Mathf.RoundToInt(offsetZ * 100));
        data.Add((int)traceType);
        data.Add((int)bindingType);
        data.Insert(index, data.Count - index);

        data.Add(m_TempParams.Count);
        data.AddRange(m_TempParams);
    }
}