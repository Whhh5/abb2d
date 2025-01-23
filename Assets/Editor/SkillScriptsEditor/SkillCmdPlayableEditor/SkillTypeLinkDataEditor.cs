
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SkillTypeLinkDataEditor : SkillTypeLinkData, ISkillTypeEditor
{
    private int m_Count => m_ArrAttackDataEditor.Count;
    private List<SkillItemInfoEditor> m_ArrAttackDataEditor = new();
    private List<IBuffDaraEditor> m_ArrBuff = new();
    private int m_CurSelectIndex = 0;

    public void InitEditor()
    {
        for (int i = 0; i < m_DataList?.Count; i++)
        {
            var data = m_DataList[i];
            var item = EditorUtil.Copy<SkillItemInfoEditor>(data);
            item.InitEditor();
            m_ArrAttackDataEditor.Add(item);
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
        var count = m_ArrAttackDataEditor.Count;
        data.Add(count);
        for (int i = 0; i < count; i++)
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
            if (GuiStyleUtil.DrawAddButton())
            {
                var menu = new GenericMenu();
                for (var i = EnBuff.None + 1; i < EnBuff.EnumCount; i++)
                {
                    var buff = i;
                    var key = EditorUtil.GetEnumName(buff);
                    menu.AddItem(new() { text = key }, false, () => {
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
            if (GuiStyleUtil.DrawAddButton())
            {
                var item = new SkillItemInfoEditor();
                item.InitEditor();
                m_ArrAttackDataEditor.Add(item);
                m_CurSelectIndex = m_ArrAttackDataEditor.Count - 1;
            }
        }
        EditorGUILayout.EndHorizontal();
        if (m_CurSelectIndex < m_ArrAttackDataEditor.Count)
        {
            var data = m_ArrAttackDataEditor[m_CurSelectIndex];
            if (GuiStyleUtil.DrawCloseButton())
            {
                m_ArrAttackDataEditor.RemoveAt(m_CurSelectIndex);
            }
            else
            {
                data.Draw();
            }
        }
    }

}
