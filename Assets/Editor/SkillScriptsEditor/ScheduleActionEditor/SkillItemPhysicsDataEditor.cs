﻿
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public partial class SkillPhysicsScheduleActionEditor : SkillPhysicsScheduleAction, ISkillScheduleActionEditor
{
    private ISkillTypeEditor m_PhysicsResolveSphereEditor = null;
    private IBuffDaraEditor _BuffEditorData = null;
    public void InitEditor()
    {
        UpdatePhysicsResolveSphereEditor();
        _BuffEditorData = SkillFactroyEditor.GetBuffDataEditor(buff);
        _BuffEditorData?.InitEditor();
        _BuffEditorData?.InitParams(arrBuffParams);
    }
    private void UpdatePhysicsResolveSphereEditor()
    {
        switch (physicsType)
        {
            case EnPhysicsType.Sphere:
                m_PhysicsResolveSphereEditor = EditorUtil.Copy<PhysicsResolveSphereEditor>(physicsResolve ?? new PhysicsResolveSphereEditor());
                break;
            case EnPhysicsType.Box:
                m_PhysicsResolveSphereEditor = EditorUtil.Copy<PhysicsResolveBoxEditor>(physicsResolve ?? new PhysicsResolveBoxEditor());
                break;
            default:
                break;
        }
        m_PhysicsResolveSphereEditor?.InitEditor();
    }
    public void Draw()
    {
        EditorGUILayout.BeginVertical();
        {
            var type = (EnPhysicsType)EditorGUILayout.EnumPopup("检测类型", physicsType, GUILayout.Width(300));
            if (type != physicsType)
            {
                switch (type)
                {
                    case EnPhysicsType.Sphere:
                        m_PhysicsResolveSphereEditor = EditorUtil.Copy<PhysicsResolveSphereEditor>(new PhysicsResolveSphere());
                        break;
                    case EnPhysicsType.Box:
                        m_PhysicsResolveSphereEditor = EditorUtil.Copy<PhysicsResolveBoxEditor>(new PhysicsResolveBox());
                        break;
                    default:
                        break;
                }
                physicsType = type;
            }
            atkSchedule = EditorGUILayout.Slider("检测进度", atkSchedule, 0, 1, GUILayout.Width(300));
            atkValue = EditorGUILayout.IntField("伤害值", atkValue, GUILayout.Width(300));
            EditorUtil.DrawCfgField<EffectCfg>("击中特效", effectID, selectID => effectID = selectID, 300);

            m_PhysicsResolveSphereEditor?.Draw();

            EditorGUILayout.BeginHorizontal();
            {
                EditorUtil.DrawCfgField<BuffCfg>("buff", (int)buff, id =>
                {
                    if (id == (int)buff)
                        return;
                    buff = (EnBuff)id;
                    _BuffEditorData = SkillFactroyEditor.GetBuffDataEditor(buff);
                    _BuffEditorData?.InitEditor();
                    _BuffEditorData?.InitParams(arrBuffParams);

                }, 300);

                if (GUILayout.Button("Def", GUILayout.Width(30)))
                {
                    buff = EnBuff.None;
                    _BuffEditorData = null;
                }
            }
            EditorGUILayout.EndHorizontal();
            _BuffEditorData?.Draw();
        }
        EditorGUILayout.EndVertical();
    }

    public void GetStringData(ref List<int> data)
    {
        var index = data.Count;
        data.Add(Mathf.RoundToInt(atkSchedule * 100));
        data.Add(Mathf.RoundToInt(atkValue));
        data.Add((int)physicsType);
        data.Add(effectID);
        data.Add((int)buff);
        data.Insert(index, data.Count - index);

        m_PhysicsResolveSphereEditor.GetStringData(ref data);

        var count = data.Count;
        _BuffEditorData?.GetStringData(ref data);
        data.Insert(count, data.Count - count);
    }

    public float GetEnterSchedule()
    {
        return atkSchedule;
    }

}
public partial class SkillPhysicsScheduleActionEditor
{
    public void Sumilation(Rect rect, float itemStartTime, float itemEndTime)
    {
        if (m_PhysicsResolveSphereEditor == null)
            return;
        if (m_PhysicsResolveSphereEditor is not ICustomSimulationEditor simulationEditor)
            return;
        var curTime = SkillWindowEditor._LastUpdateTime;
        var maxTime = SkillWindowEditor._MaxTime;
        var targetObj = SkillWindowEditor._PrefabObj;
        var localShowTime = (itemEndTime - itemStartTime) * atkSchedule;
        var showTime = localShowTime + itemStartTime;
        var hideTime = showTime + 1f;

        if (curTime < showTime || curTime >= hideTime)
        {
            simulationEditor.DestroySimulation();
            return;
        }
        simulationEditor.UpdateSimulation(rect, itemStartTime, itemEndTime);
    }

}