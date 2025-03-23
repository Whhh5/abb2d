using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


//public class SelectClipSkillDataEditor: SelectClipSkillData
//{

//}
public class SkillTypeSingletonDataEditor : SkillTypeSingletonData, ISkillTypeEditor
{
    private List<int> m_ClipList = new();
    //private SkillItemInfoEditor[] _ItemInfoList = new SkillItemInfoEditor[1];

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
                var index = i;
                var clipID = m_ClipList[index];
                EditorUtil.DrawClipID(clipID, value => m_ClipList[index] = value);
            }
        }
        EditorGUILayout.EndVertical();
    }

    public void GetStringData(ref List<int> data)
    {
        data.AddRange(m_ClipList);
    }
}