
using System;
using System.Collections.Generic;
using System.Reflection;
using DG.DemiEditor;
using UnityEditor;
using UnityEngine;

public class SkillItemInfoEditor : SkillItemInfo, ISkillTypeEditor
{
    public int ScheduleEditorCount => m_AtkLinkScheduleList.Count;
    private List<ISkillScheduleActionEditor> m_AtkLinkScheduleList = new();
    private List<IBuffDaraEditor> m_BuffDataList = new();

    private Dictionary<EnAtkLinkScheculeType, bool> _ScheduleFoldout = new();
    public void InitEditor()
    {
        for (int i = 0; i < m_ArrAtkLinkSchedule?.Length; i++)
        {
            var data = m_ArrAtkLinkSchedule[i];
            var scheduleType = data.GetScheduleType();
            var editorType = SkillFactroyEditor.GetScheduleEditorType(scheduleType);
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
    private void AddAtkLinkScheduleList(ISkillScheduleActionEditor item)
    {
        m_AtkLinkScheduleList.Add(item);
    }

    public ISkillScheduleActionEditor GetISkillScheduleActionEditor(int index)
    {
        return m_AtkLinkScheduleList[index];
    }
    public void Draw()
    {
        DrawBaseInfo();

        GUILayout.Space(10);

        DrawBuff();

        GUILayout.Space(10);

        DrawScheduleEvent();
    }

    public void GetStringData(ref List<int> data)
    {
        var index = data.Count;
        data.Add((int)_ClipID);
        data.Add(Mathf.RoundToInt(canNextTime * 100));
        data.Add(Mathf.RoundToInt(atkEndTime * 100));
        data.Add(Mathf.RoundToInt(_IsAutoRemove ? 1 : 0));

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

    private void DrawBaseInfo()
    {
        var boxStyle = GuiStyleUtil.CreateLayoutBoxBackgroud(Color.white);
        EditorGUILayout.BeginVertical(boxStyle);
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorUtil.DrawClipID(_ClipID, value => _ClipID = value);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                var txtContent = new GUIContent()
                {
                    text = "next",
                    tooltip = "可以进行下一个动作的时间",
                };
                EditorGUILayout.LabelField(txtContent, GUILayout.Width(50));
                var rect = GUILayoutUtility.GetRect(150, 15, GUI.skin.box); // 设置进度条高度
                canNextTime = GuiStyleUtil.DrawSlider(canNextTime, rect);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                var txtContent = new GUIContent()
                {
                    text = "Atk End",
                    tooltip = "攻击结束时间",
                };
                EditorGUILayout.LabelField(txtContent, GUILayout.Width(50));
                var rect = GUILayoutUtility.GetRect(150, 15, GUI.skin.box); // 设置进度条高度
                atkEndTime = GuiStyleUtil.DrawSlider(atkEndTime, rect);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                var txtContent = new GUIContent()
                {
                    text = "Auto Rem",
                    tooltip = "播放完成自动过度到下一个动作",
                };
                EditorGUILayout.LabelField(txtContent, GUILayout.Width(50));
                _IsAutoRemove = EditorGUILayout.Toggle(_IsAutoRemove, GUILayout.Width(30));
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
    }
    private void DrawBuff()
    {
        var style = GuiStyleUtil.CreateLayoutBoxBackgroud(Color.white);
        EditorGUILayout.BeginVertical(style);
        {
            var horRect = EditorGUILayout.BeginHorizontal();
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
                        if (m_BuffDataList.FindIndex((item) => item.Buff == buff) >= 0)
                            continue;
                        var key = EditorUtil.GetEnumName(buff);
                        menu.AddItem(new() { text = key }, false, () =>
                        {
                            var type = SkillFactroyEditor.GetBuffDataEditor(buff);
                            m_BuffDataList.Add(type);
                        });
                    }
                    menu.ShowAsContext();
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                for (int i = 0; i < m_BuffDataList.Count; i++)
                {
                    //var rect = GUILayoutUtility.GetRect(150, 15, GUI.skin.box);

                    var buffData = m_BuffDataList[i];
                    var name = EditorUtil.GetClassName(buffData);
                    var boxStyle = GuiStyleUtil.CreateLayoutBoxBackgroud(new Color32(255, 255, 255, 50));
                    var verRect = EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width(100));
                    {
                        //EditorGUI.DrawRect(verRect, new Color(0.5f, 0.5f, 0.5f, 0.5f));
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField(name, GUILayout.Width(50));
                            var closeBtnRect = new Rect()
                            {
                                center = new Vector2(verRect.width - 5 - 30, -15) + verRect.position,
                                size = Vector2.one * 30,
                            };

                            if (GuiStyleUtil.DrawCloseButton(closeBtnRect))
                            {
                                m_BuffDataList.RemoveAt(i);
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
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
        EditorGUILayout.EndVertical();
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
        var style = GuiStyleUtil.CreateLayoutBoxBackgroud(Color.white);
        EditorGUILayout.BeginVertical(style);
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("进度事件检测", GUILayout.Width(100));
                if (GUILayout.Button("添加", GUILayout.Width(100)))
                {
                    var menu = new GenericMenu();

                    for (var i = EnAtkLinkScheculeType.None + 1; i < EnAtkLinkScheculeType.EnumCount; i++)
                    {
                        var scheduleType = i;
                        var key = EditorUtil.GetEnumName(scheduleType);
                        menu.AddItem(new() { text = key }, false, () =>
                        {
                            var type = SkillFactroyEditor.GetScheduleEditorType(scheduleType);
                            var insType = Activator.CreateInstance(type);
                            var value = insType as ISkillScheduleActionEditor;
                            value.SetScheduleType(scheduleType);
                            value.InitEditor();
                            AddAtkLinkScheduleList(value);
                        });
                    }
                    menu.ShowAsContext();

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

            foreach (var item in dic)
            {
                var keyType = item.Key.GetType();
                var fieldInfo = keyType.GetField(item.Key.ToString());
                var enumName = fieldInfo.GetCustomAttribute<EditorFieldNameAttribute>();

                if (!_ScheduleFoldout.ContainsKey(item.Key))
                    _ScheduleFoldout.Add(item.Key, true);

                var boxStyle = GuiStyleUtil.CreateLayoutBoxBackgroud(new Color32(255, 255, 255, 50));
                EditorGUILayout.BeginVertical(boxStyle);
                {
                    if (_ScheduleFoldout[item.Key] = EditorGUILayout.BeginFoldoutHeaderGroup(_ScheduleFoldout[item.Key], enumName?.fieldName))
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            boxStyle = GuiStyleUtil.CreateLayoutBoxBackgroud(new Color32(255, 0, 0, 255));
                            for (int i = 0; i < item.Value.Count; i++)
                            {
                                var value = item.Value[i];
                                var rect = EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width(320), GUILayout.ExpandWidth(false));
                                {
                                    var closeRect = new Rect(rect)
                                    {
                                        x = rect.x + rect.width - 30,
                                        y = rect.y - 30 / 1.5f,
                                        width = 30,
                                        height = 30,
                                    };
                                    if (GuiStyleUtil.DrawCloseButton(closeRect))
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
                    EditorGUILayout.EndFoldoutHeaderGroup();
                }
                EditorGUILayout.EndVertical();
            }
        }
        EditorGUILayout.EndVertical();
    }
}