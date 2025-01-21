
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class SkillItemInfoEditor : SkillItemInfo, ISkillTypeEditor
{
    private List<ISkillScheduleActionEditor> m_AtkLinkScheduleList = new();
    private List<IBuffDaraEditor> m_BuffDataList = new();
    public void InitEditor()
    {
        for (int i = 0; i < m_ArrAtkLinkSchedule?.Length; i++)
        {
            var data = m_ArrAtkLinkSchedule[i];
            var scheduleType = data.GetScheduleType();
            var editorType = GetScheduleEditorType(scheduleType);
            var item = EditorUtil.Copy<ISkillScheduleActionEditor>(data, editorType);
            item.InitEditor();
            m_AtkLinkScheduleList.Add(item);
        }

        foreach (var item in arrBuff)
        {
            var buffEditorData = SkillFactroyEditor.GetBuffDataEditor(item.Key);
            buffEditorData.InitParams(item.Value);
            buffEditorData.InitEditor();
            m_BuffDataList.Add(buffEditorData);
        }
    }
    public Type GetScheduleEditorType(EnAtkLinkScheculeType scheduleType)
    {
        return scheduleType switch
        {
            EnAtkLinkScheculeType.Buff => typeof(AttackLinkBuffItemEditor),
            EnAtkLinkScheculeType.Effect => typeof(AttackLinkEffectItemEditor),
            EnAtkLinkScheculeType.Physics => typeof(SkillItemPhysicsDataEditor),
            EnAtkLinkScheculeType.Behaviour => typeof(AttackLinkBehaviourItemEditor),
            _ => null
        };
    }
    private void AddAtkLinkScheduleList(ISkillScheduleActionEditor item)
    {
        m_AtkLinkScheduleList.Add(item);
    }
    public void Draw()
    {
        EditorGUILayout.BeginVertical();
        {
            _ClipID = EditorGUILayout.IntField("剪辑类型", _ClipID, GUILayout.Width(300));
            canNextTime = EditorGUILayout.Slider("可以进行下一个动作的时间", canNextTime, 0, 1, GUILayout.Width(300));
            atkEndTime = EditorGUILayout.Slider("攻击结束时间", atkEndTime, 0, 1, GUILayout.Width(300));

            DrawBuff();

            DrawScheduleEvent();
        }
        EditorGUILayout.EndVertical();
    }

    public void GetStringData(ref List<int> data)
    {
        var index = data.Count;
        data.Add((int)_ClipID);
        data.Add(Mathf.RoundToInt(canNextTime * 100));
        data.Add(Mathf.RoundToInt(atkEndTime * 100));

        data.Insert(index, data.Count - index);


        m_AtkLinkScheduleList.Sort(SortArray);
        var effectCount = m_AtkLinkScheduleList.Count;
        data.Add(effectCount);
        for (int i = 0; i < effectCount; i++)
        {
            var item = m_AtkLinkScheduleList[i];
            data.Add((int)item.GetScheduleType());
            var tempIndex = data.Count;
            item.GetStringData(ref data);
            var tempInterval = data.Count - tempIndex;
            data.Insert(tempIndex, tempInterval);
        }

        var buffCount = m_BuffDataList.Count;
        data.Add(buffCount);
        for (int i = 0; i < m_BuffDataList.Count; i++)
        {
            var buff = m_BuffDataList[i];
            data.Add((int)buff.Buff);
            var buffIndex = data.Count;
            buff.GetStringData(ref data);
            data.Insert(buffIndex, data.Count - buffIndex);
        }
    }

    private void DrawBuff()
    {
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("buff", GUILayout.Width(50));
            if (GUILayout.Button("➕", GUILayout.Width(50)))
            {
                var rect = new Rect()
                {
                    center = new Vector2(50, 100),
                    width = 200,
                };
                List<GUIContent> contents = new();
                Dictionary<int, EnBuff> index2Buff = new();
                for (var i = EnBuff.None + 1; i < EnBuff.EnumCount; i++)
                {
                    index2Buff.Add(contents.Count, i);
                    var name = EditorUtil.GetEnumName(i);
                    contents.Add(new() { text = name });
                }

                EditorUtility.DisplayCustomMenu(rect, contents.ToArray(), 0, (object userData, string[] options, int selected) =>
                {
                    var buff = index2Buff[selected];
                    var type = SkillFactroyEditor.GetBuffDataEditor(buff);
                    m_BuffDataList.Add(type);

                }, null);
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            for (int i = 0; i < m_BuffDataList.Count; i++)
            {
                var buffData = m_BuffDataList[i];
                var name = EditorUtil.GetClassName(buffData);
                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField(name, GUILayout.Width(50));
                        if (GUILayout.Button("❌", GUILayout.Width(50)))
                        {
                            m_BuffDataList.RemoveAt(i);
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
    private int SortArray(ISkillScheduleActionEditor item1, ISkillScheduleActionEditor item2)
    {
        if (item1.GetEnterSchedule() < item2.GetEnterSchedule())
            return -1;
        if (item1.GetEnterSchedule() > item2.GetEnterSchedule())
            return 1;
        return 0;
    }

    private void DrawScheduleEvent()
    {
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label("进度事件检测", GUILayout.Width(100));
            if (GUILayout.Button("添加", GUILayout.Width(100)))
            {
                var rect = new Rect()
                {
                    center = new Vector2(50, 100),
                    width = 200,
                };
                List<GUIContent> contents = new();
                var index2ScheduleType = new Dictionary<int, EnAtkLinkScheculeType>();

                for (var i = EnAtkLinkScheculeType.None + 1; i < EnAtkLinkScheculeType.EnumCount; i++)
                {
                    index2ScheduleType.Add(contents.Count, i);
                    var name = EditorUtil.GetEnumName(i);
                    contents.Add(new() { text = name });
                }

                EditorUtility.DisplayCustomMenu(rect, contents.ToArray(), 0, (object userData, string[] options, int selected) =>
                {
                    var scheduleType = index2ScheduleType[selected];
                    var type = GetScheduleEditorType(scheduleType);
                    var insType = Activator.CreateInstance(type);
                    var value = insType as ISkillScheduleActionEditor;
                    value.SetScheduleType(scheduleType);
                    value.InitEditor();
                    AddAtkLinkScheduleList(value);

                }, null);

            }
        }
        EditorGUILayout.EndHorizontal();

        var dic = new Dictionary<EnAtkLinkScheculeType, List<ISkillScheduleActionEditor>>();
        foreach (var item in m_AtkLinkScheduleList)
        {
            var scheduleType = item.GetScheduleType();
            if (!dic.TryGetValue(scheduleType, out var list))
            {
                list = new();
                dic.Add(scheduleType, list);
            }
            list.Add(item);
        }

        GUILayout.Space(20);

        foreach (var item in dic)
        {
            EditorGUILayout.BeginVertical();
            {
                var keyType = item.Key.GetType();
                var fieldInfo = keyType.GetField(item.Key.ToString());
                var enumName = fieldInfo.GetCustomAttribute<EditorFieldNameAttribute>();
                EditorGUILayout.BeginHorizontal(GUILayout.Height(20));
                {
                    //EditorGUILayout.Space(100);
                    GUILayout.Button("", GUILayout.Width(200), GUILayout.Height(3));
                    EditorGUILayout.LabelField(enumName?.fieldName ?? $"{item.Key}", GUILayout.Width(100));
                    GUILayout.Button("", GUILayout.ExpandWidth(true), GUILayout.Height(3));
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                {
                    for (int i = 0; i < item.Value.Count; i++)
                    {
                        var value = item.Value[i];
                        EditorGUILayout.BeginVertical();
                        {
                            if (GUILayout.Button("❌", GUILayout.Width(50)))
                            {
                                m_AtkLinkScheduleList.Remove(value);
                            }
                            else
                            {
                                value.Draw();
                            }
                        }
                        EditorGUILayout.EndVertical();
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            GUILayout.Space(20);
        }
    }
}