﻿using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SkillTypeLoopDataEditor : SkillTypeLoopData, ISkillTypeEditor
{
    private const int m_Count = 3;
    private List<SkillItemInfoEditor> m_ArrAttackDataEditor = new(new SkillItemInfoEditor[m_Count]);
    private List<IBuffDaraEditor> m_ArrBuff = new();
    private int m_CurSelectIndex = 0;

    public void InitEditor()
    {
        for (int i = 0; i < m_Count; i++)
        {
            var data = i < m_DataList.Count ? m_DataList[i] : new();
            var item = EditorUtil.Copy<SkillItemInfoEditor>(data);
            item.InitEditor();
            m_ArrAttackDataEditor[i] = item;
        }

        foreach (var item in m_BuffList)
        {
            var type = SkillFactroyEditor.GetBuffDataEditor(item.Key);
            type.InitParams(item.Value);
            type.InitEditor();
            m_ArrBuff.Add(type);
        }
    }
    public void Draw()
    {
        EditorGUILayout.BeginVertical();
        {
            DrawBuffData();

            DrawAtkData();
        }
        EditorGUILayout.EndVertical();
    }

    public void GetStringData(ref List<int> data)
    {
        data.Add(m_Count);
        for (int i = 0; i < m_Count; i++)
        {
            var index = data.Count;
            var data2 = m_ArrAttackDataEditor[i];
            data2.GetStringData(ref data);
            data.Insert(index, data.Count - index);
        }

        var buffCount = m_ArrBuff.Count;
        data.Add(buffCount);
        for (int i = 0; i < m_ArrBuff.Count; i++)
        {
            var buff = m_ArrBuff[i];
            data.Add((int)buff.Buff);
            var buffIndex = data.Count;
            buff.GetStringData(ref data);
            data.Insert(buffIndex, data.Count - buffIndex);
        }
    }

    private void DrawBuffData()
    {
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("buff", GUILayout.Width(50));
            var titleRect = GUILayoutUtility.GetLastRect();
            var addRect = new Rect()
            {
                position = titleRect.position + new Vector2(titleRect.width, 0),
                size = new Vector2(100, titleRect.height),
            };
            if (GuiStyleUtil.DrawAddButton(addRect))
            {
                var menu = new GenericMenu();
                var count = ExcelUtil.GetCfgCount<BuffCfg>();
                for (var i = 0; i < count; i++)
                {
                    var buff = (EnBuff)ExcelUtil.GetCfgByIndex<BuffCfg>(i).nBuffID;
                    if (m_ArrBuff.FindIndex((item) => item.Buff == buff) >= 0)
                        continue;
                    var key = EditorUtil.GetEnumName(buff);
                    menu.AddItem(new() { text = key }, false, () =>
                    {
                        var type = SkillFactroyEditor.GetBuffDataEditor(buff);
                        m_ArrBuff.Add(type);
                    });
                }
                menu.ShowAsContext();
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            for (int i = 0; i < m_ArrBuff.Count; i++)
            {
                var buffData = m_ArrBuff[i];
                var name = EditorUtil.GetClassName(buffData);
                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField(name, GUILayout.Width(50));
                        if (GuiStyleUtil.DrawCloseButton())
                        {
                            m_ArrBuff.RemoveAt(i);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    {
                        buffData.Draw();
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawAtkData()
    {

        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField(m_Count.ToString(), GUILayout.Width(40));

            for (int i = 0; i < m_Count; i++)
            {
                if (GUILayout.Button($"{i}", GUILayout.Width(80), GUILayout.Height(m_CurSelectIndex == i ? 30 : 20)))
                {
                    m_CurSelectIndex = i;
                }
            }
        }
        EditorGUILayout.EndHorizontal();
        if (m_CurSelectIndex < m_ArrAttackDataEditor.Count)
        {
            var data = m_ArrAttackDataEditor[m_CurSelectIndex];
            data.Draw();
        }
    }
}

