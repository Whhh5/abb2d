
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AttackLinkBuffItemEditor : SkillBuffScheduleAction, ISkillScheduleActionEditor
{
    private List<int> m_BuffParamList = new();
    public void InitEditor()
    {
        m_BuffParamList.AddRange(arrBuffParams ?? new int[0]);
    }
    public void Draw()
    {
        EditorGUILayout.BeginVertical();
        {
            buffID = EditorGUILayout.IntField("buffID:", buffID, GUILayout.Width(200));
            addSchedule = EditorGUILayout.Slider("buffID:", addSchedule, 0, 1, GUILayout.Width(200));

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("buff 参数", GUILayout.Width(100));
                for (int i = 0; i < m_BuffParamList.Count; i++)
                {
                    m_BuffParamList[0] = EditorGUILayout.IntField(m_BuffParamList[i], GUILayout.Width(50));
                    if (GUILayout.Button("❌", GUILayout.Width(30)))
                    {
                        m_BuffParamList.RemoveAt(i);
                    }
                    EditorGUILayout.Space(20);
                }
                if (GUILayout.Button("➕", GUILayout.Width(50)))
                {
                    m_BuffParamList.Add(0);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
    }

    public void GetStringData(ref List<int> data)
    {
        var index = data.Count;
        data.Add(buffID);
        data.Add(Mathf.RoundToInt(addSchedule * 100));

        data.Insert(index, data.Count - index);

        data.Add(m_BuffParamList.Count);
        data.AddRange(m_BuffParamList);
    }
}