
using System.Collections.Generic;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using static UnityEditor.Rendering.InspectorCurveEditor;

public class SkillTypeLinkDataEditor : SkillTypeLinkData, ISkillTypeEditor, ISkillSimulationEditor
{
    private int m_Count => m_ArrAttackDataEditor.Count;
    private List<SkillItemInfoEditor> m_ArrAttackDataEditor = new();
    private List<IBuffDaraEditor> m_ArrBuff = new();
    private int m_CurSelectIndex = 0;

    private float _SimulationMaxTime = -1;

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

        _SimulationMaxTime = 0f;
        var clipCfgList = ExcelUtil.ReadEditorCfgList<ClipCfg>();
        var assetCfgList = ExcelUtil.ReadEditorCfgList<AssetCfg>();
        foreach (var item in m_ArrAttackDataEditor)
        {
            var clipID = item.GetClipID();
            var clipCfg = clipCfgList.Find(item => item.nClipID == clipID);
            var clipAssetID = clipCfg.nAssetID;
            var clipPath = assetCfgList.Find(item => item.nAssetID == clipAssetID).strPath;
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
            _SimulationMaxTime += clip.length;
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
        var verRect = GuiStyleUtil.CreateLayoutBoxBackgroud(Color.white);
        EditorGUILayout.BeginVertical(verRect);
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("buff", GUILayout.Width(50));
                var titleRect = GUILayoutUtility.GetLastRect();
                var pos = titleRect.position + new Vector2(titleRect.width, 0);
                var addRect = new Rect(pos.x, pos.y, 100, titleRect.height);
                if (GuiStyleUtil.DrawAddButton(addRect))
                {
                    var menu = new GenericMenu();
                    for (var i = EnBuff.None + 1; i < EnBuff.EnumCount; i++)
                    {
                        var buff = i;
                        if (m_ArrBuff.FindIndex((item) => item.Buff == buff ? true : false) >= 0)
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
                    var buffRect = EditorGUILayout.BeginVertical(verRect, GUILayout.Width(100));
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField(name, GUILayout.Width(50));
                            var closeRect = new Rect()
                            {
                                position = buffRect.position + new Vector2(buffRect.width - 35, -15),
                                size = Vector2.one * 30,
                            };
                            if (GuiStyleUtil.DrawCloseButton(closeRect))
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
        EditorGUILayout.EndVertical();
    }

    private void DrawAtkData()
    {

        EditorGUILayout.BeginHorizontal();
        {
            //EditorGUILayout.LabelField(m_Count.ToString(), GUILayout.Width(40));
            var addRect = GUILayoutUtility.GetRect(20, 20, GUI.skin.button, GUILayout.ExpandWidth(false));
            if (GuiStyleUtil.DrawAddButton(addRect))
            {
                var item = new SkillItemInfoEditor();
                item.InitEditor();
                m_ArrAttackDataEditor.Add(item);
                m_CurSelectIndex = m_ArrAttackDataEditor.Count - 1;
            }

            for (int i = 0; i < m_Count; i++)
            {
                if (GuiStyleUtil.DrawButton(new GUIContent()
                {
                    text = $"{i}",
                    image = m_CurSelectIndex == i ? EditorLoad.LoadTexture2D(EnEditorRes.btn_red) : null
                },
                    null, () =>
                {
                    if (EditorUtility.DisplayDialog("删除阶段技能？", "此操作不可恢复", "yes", "cancel"))
                    {
                        m_ArrAttackDataEditor.RemoveAt(i);
                        if (m_CurSelectIndex == i)
                            m_CurSelectIndex = Mathf.Min(m_Count - 1, m_CurSelectIndex);
                        i--;
                    }
                }, 50, 20))
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


    //private void _Simulation
    public float GetMaxSimulationTime()
    {
        return _SimulationMaxTime;
    }
    private List<(float start, float end)> _StepDataList = null;
    private AnimationMixerPlayable _MixerPlayable;
    public void InitSimulation(ref PlayableGraph graph)
    {
        _StepDataList = new(m_Count);
        var assetCfgList = ExcelUtil.ReadEditorCfgList<AssetCfg>();
        var clipCfgList = ExcelUtil.ReadEditorCfgList<ClipCfg>();
        var output = graph.GetOutput(0);
        _MixerPlayable = AnimationMixerPlayable.Create(graph, m_Count);
        output.SetSourcePlayable(_MixerPlayable);
        var time = 0f;
        for (int i = 0; i < m_Count; i++)
        {
            var clipID = m_ArrAttackDataEditor[i].GetClipID();
            var clipCfg = clipCfgList.Find(item => item.nClipID == clipID);
            var clipAssetID = clipCfg.nAssetID;
            var clipPath = assetCfgList.Find(item => item.nAssetID == clipAssetID).strPath;
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);

            var clipPlayable = AnimationClipPlayable.Create(graph, clip);
            _MixerPlayable.ConnectInput(i, clipPlayable, 0);
            _MixerPlayable.SetInputWeight(i, 0);

            _StepDataList.Add(new(time, time + clip.length));
            time += clip.length;
        }
    }

    ISkillScheduleActionEditor selectID = null;
    public void UpdateSimulation(float time)
    {

        var mainVerRect = EditorGUILayout.BeginVertical();
        {
            var rectList = new List<Rect>();
            var itemInterval = 2;
            var height = 5f;
            var trackInterval = 2;
            var maxHeight = height * ((int)EnAtkLinkScheculeType.EnumCount - 1) + trackInterval * ((int)EnAtkLinkScheculeType.EnumCount - 2);
            var mainRect = GUILayoutUtility.GetRect(
                new GUIContent()
                , GUI.skin.box
                , GUILayout.ExpandWidth(true)
                , GUILayout.Height(maxHeight));
            EditorGUILayout.BeginHorizontal();
            {
                var maxWidth = mainVerRect.width - itemInterval * (m_Count - 1);
                var tempWidth = maxWidth / m_Count;
                var nextPosX = 0f;
                for (int i = 0; i < m_Count; i++)
                {
                    var (start, end) = _StepDataList[i];
                    var length = end - start;
                    var isPlay = time >= start && time <= end;
                    var weight = isPlay ? 1 : 0;
                    _MixerPlayable.SetInputWeight(i, weight);
                    var schedule = time % _SimulationMaxTime - start;
                    if (isPlay)
                    {
                        _MixerPlayable.GetInput(i).SetTime(schedule);
                    }
                    var width = length / (_SimulationMaxTime / m_Count) * tempWidth;
                    var rect = new Rect(mainRect)
                    {
                        x = nextPosX,
                        width = width,
                    };
                    nextPosX += width + itemInterval;
                    rectList.Add(rect);
                    EditorGUI.DrawRect(rect, Color.grey);
                    var slider = Mathf.Clamp01(schedule / length);
                    var sliderRect = new Rect(rect)
                    {
                        width = rect.width * slider,
                    };
                    EditorGUI.DrawRect(sliderRect, Color.green);

                    {
                        var curE = Event.current;
                        var mousePos = curE.mousePosition;
                        if (rect.Contains(mousePos))
                        {
                            var style = new GUIStyle(GUI.skin.label)
                            {
                                alignment = TextAnchor.MiddleCenter,
                            };
                            EditorGUI.LabelField(rect, $"{length}", style);
                        }
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                for (var i = EnAtkLinkScheculeType.None + 1; i < EnAtkLinkScheculeType.EnumCount - 1; i++)
                {
                    var posY = (float)(i - 1) * (height + trackInterval);

                    var tempRect = new Rect(mainRect)
                    {
                        position = new Vector2(mainRect.x, mainRect.position.y + posY),
                        height = height,
                    };
                    var col = new Color(0.5f, 0.5f, 0.5f, 0.3f);
                    EditorGUI.DrawRect(tempRect, col);
                }

                for (int i = 0; i < m_Count; i++)
                {
                    var atkData = m_ArrAttackDataEditor[i];
                    var itemRect = rectList[i];
                    var count = atkData.ScheduleEditorCount;
                    for (int k = 0; k < count; k++)
                    {
                        var scheduleData = atkData.GetISkillScheduleActionEditor(k);
                        var slider = scheduleData.GetEnterSchedule();
                        var type = scheduleData.GetScheduleType();
                        var rectPos = new Vector2(itemRect.position.x + itemRect.width * slider, mainRect.y);
                        var rect = new Rect(rectPos.x, rectPos.y + ((int)type - 1) * (height + 1f), 2, height);

                        var color = new Color()
                        {
                            r = (int)type / (float)EnAtkLinkScheculeType.EnumCount,
                            g = (float)k / count,
                            b = 1,
                            a = 1,
                        };
                        EditorGUI.DrawRect(rect, color);

                        if (rect.Contains(Event.current.mousePosition))
                        {
                            selectID = scheduleData;
                        }
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();


        
        if (selectID != null)
        {
            selectID.Draw();
        }
    }

    public void DrawSimulation()
    {

    }
}
