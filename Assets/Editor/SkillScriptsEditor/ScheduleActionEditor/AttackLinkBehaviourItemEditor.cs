
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AttackLinkBehaviourItemEditor : SkillBehaviourScheduleAction, ISkillScheduleActionEditor
{
    private List<int> m_ParamsList = new();


    public void InitEditor()
    {
        m_ParamsList.AddRange(arrParams ?? new int[1]);
    }
    public void Draw()
    {
        EditorGUILayout.BeginVertical();
        {
            behaviourType = (EnBehaviourType)EditorGUILayout.EnumPopup(behaviourType, GUILayout.Width(100));
            schedule = EditorGUILayout.Slider(schedule, 0, 1, GUILayout.Width(200));
            m_ParamsList[0] = Mathf.RoundToInt(EditorGUILayout.FloatField(m_ParamsList[0] / 100f, GUILayout.Width(100)) * 100);
        }
        EditorGUILayout.EndVertical();
    }

    public void GetStringData(ref List<int> data)
    {
        var index = data.Count;
        data.Add(Mathf.RoundToInt(schedule * 100));
        data.Add((int)behaviourType);

        data.Insert(index, data.Count - index);

        data.Add(m_ParamsList.Count);
        data.AddRange(m_ParamsList);
    }
}