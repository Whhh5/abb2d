using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class SingletonClipSkillDataEditor : SingletonClipSkillData, IEditorItem
{
    private List<int> m_ClipList = new();

    public void InitEditor()
    {
        m_ClipList.AddRange(m_ArrClip ?? new int[] { });
    }

    public void Draw()
    {
        EditorGUILayout.BeginVertical();
        {
            if (GUILayout.Button("Add"))
            {
                m_ClipList.Add(0);
            }
            for (int i = 0; i < m_ClipList.Count; i++)
            {
                var clipID = m_ClipList[i];

                EditorGUILayout.BeginHorizontal();
                {
                    m_ClipList[i] = EditorGUILayout.IntField("clipID:", clipID, GUILayout.Width(200));
                    if (GUILayout.Button("❌", GUILayout.Width(50)))
                    {
                        m_ClipList.RemoveAt(i);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndVertical();
    }

    public void GetStringData(ref List<int> data)
    {
        data.AddRange(m_ClipList);
    }
}