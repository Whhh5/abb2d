
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public partial class SkillEffectScheduleActionEditor : SkillEffectScheduleAction, ISkillScheduleActionEditor
{
    private List<int> m_TempParams = new(new int[2]);
    public void InitEditor()
    {
        m_TempParams[0] = effectParams?.Length > 0 ? effectParams[0] : 0;
        m_TempParams[1] = effectParams?.Length > 1 ? effectParams[1] : 0;
    }
    public void Draw()
    {
        EditorGUILayout.BeginVertical();
        {
            EditorUtil.DrawCfgField<EffectCfg>("特效ID", effectID, id => effectID = id, 300);
            schedule = EditorGUILayout.Slider("进度", schedule, 0, 1, GUILayout.Width(300));

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("pos偏移", GUILayout.Width(100));
                offsetX = EditorGUILayout.FloatField(offsetX, GUILayout.Width(50));
                offsetY = EditorGUILayout.FloatField(offsetY, GUILayout.Width(50));
                offsetZ = EditorGUILayout.FloatField(offsetZ, GUILayout.Width(50));
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("rot偏移", GUILayout.Width(100));
                offsetRotX = EditorGUILayout.FloatField(offsetRotX, GUILayout.Width(50));
                offsetRotY = EditorGUILayout.FloatField(offsetRotY, GUILayout.Width(50));
                offsetRotZ = EditorGUILayout.FloatField(offsetRotZ, GUILayout.Width(50));
            }
            EditorGUILayout.EndHorizontal();

            traceType = (EnEffectTraceType)EditorGUILayout.EnumPopup(traceType, GUILayout.Width(100));
            bindingType = (EnEffectBindingType)EditorGUILayout.EnumPopup(bindingType, GUILayout.Width(100));
        }
        EditorGUILayout.EndVertical();
    }

    public void GetStringData(ref List<int> data)
    {
        var index = data.Count;
        data.Add(effectID);
        data.Add(Mathf.RoundToInt(schedule * 100));
        data.Add(Mathf.RoundToInt(offsetX * 100));
        data.Add(Mathf.RoundToInt(offsetY * 100));
        data.Add(Mathf.RoundToInt(offsetZ * 100));
        data.Add((int)traceType);
        data.Add((int)bindingType);
        data.Add(Mathf.RoundToInt(offsetRotX * 100));
        data.Add(Mathf.RoundToInt(offsetRotY * 100));
        data.Add(Mathf.RoundToInt(offsetRotZ * 100));
        data.Insert(index, data.Count - index);

        data.Add(m_TempParams.Count);
        data.AddRange(m_TempParams);
    }
    public float GetEnterSchedule()
    {
        return schedule;
    }

}

public partial class SkillEffectScheduleActionEditor
{
    private EffectEntity _EffectGO = null;
    private Vector3 _TargetForward = Vector3.zero;
    private Vector3 _TargetUp = Vector3.zero;
    private Vector3 _TargetRight = Vector3.zero;
    private Vector3 _TargetRot = Vector3.zero;
    private Vector3 _TargetPos = Vector3.zero;
    public void Sumilation(Rect rect, float itemStartTime, float itemEndTime)
    {
        var curTime = SkillWindowEditor._LastUpdateTime;
        var maxTime = SkillWindowEditor._MaxTime;

        var effectCfg = ExcelUtil.GetCfg<EffectCfg>(effectID);
        var effectTime = effectCfg.fDelayDestroyTime;
        var showTime = (itemEndTime - itemStartTime) * schedule + itemStartTime;
        var hideTime = showTime + effectCfg.fDelayDestroyTime;

        if (curTime < showTime || curTime >= hideTime || curTime == maxTime)
        {
            DestroyEffectEntity();
            return;
        }
        InitSimulation();

        var time = curTime - showTime;
        _EffectGO.SetSimulationTime(time);

        var bgRect = new Rect(rect)
        {
            x = rect.x + rect.width * showTime / maxTime,
            y = rect.y + rect.height + 2,
            width = effectTime / maxTime * rect.width,
            height = 5,
        };
        EditorGUI.DrawRect(bgRect, Color.blue);
        var sliderRect = new Rect(bgRect)
        {
            width = bgRect.width * time / effectTime,
        };
        EditorGUI.DrawRect(sliderRect, Color.yellow);


        // 设置位置
        var curPos = _EffectGO.transform.position;
        var curRot = _EffectGO.transform.rotation;

        var localPos = curPos - _TargetPos;
        var localRot = curRot.eulerAngles - _TargetRot;

        //var offsetX2 = new Vector3(_TargetRight.x * localPos.x, _TargetRight.y * localPos.y, _TargetRight.z * localPos.z);
        //var offsetY2 = new Vector3(_TargetUp.x * localPos.x, _TargetUp.y * localPos.y, _TargetUp.z * localPos.z);
        //var offsetZ2 = new Vector3(_TargetForward.x * localPos.x, _TargetForward.y * localPos.y, _TargetForward.z * localPos.z);

        //offsetX = offsetX2.x + offsetY2.x + offsetZ2.z;
        //offsetY = offsetX2.y + offsetY2.y + offsetZ2.y;
        //offsetZ = offsetX2.z + offsetY2.z + offsetZ2.z;

        offsetX = Vector3.Dot(localPos, _TargetRight);
        offsetY = Vector3.Dot(localPos, _TargetUp);
        offsetZ = Vector3.Dot(localPos, _TargetForward);

        offsetRotX = localRot.x;
        offsetRotY = localRot.y;
        offsetRotZ = localRot.z;
    }

    private void InitSimulation()
    {
        if (_EffectGO != null)
            return;
        if (effectID <= 0)
            return;
        var effectCfg = ExcelUtil.GetCfg<EffectCfg>(effectID);
        var assetCfg = ExcelUtil.GetCfg<AssetCfg>(effectCfg.nAssetID);
        var ass = AssetDatabase.LoadAssetAtPath<GameObject>(assetCfg.strPath);
        if (ass == null || !ass.TryGetComponent<EffectEntity>(out var com))
            return;

        _EffectGO = GameObject.Instantiate(com, null);

        _TargetRight = SkillWindowEditor._PrefabObj.transform.right;
        _TargetUp = SkillWindowEditor._PrefabObj.transform.up;
        _TargetForward = SkillWindowEditor._PrefabObj.transform.forward;
        _TargetRot = SkillWindowEditor._PrefabObj.transform.rotation.eulerAngles;
        _TargetPos = SkillWindowEditor._PrefabObj.transform.position;
        _EffectGO.transform.SetPositionAndRotation(_TargetPos
            + _TargetRight * offsetX
            + _TargetUp * offsetY
            + _TargetForward * offsetZ
            , Quaternion.Euler(
                _TargetRot
                + new Vector3(offsetRotX, offsetRotY, offsetRotZ)));

        _EffectGO.Stop();
        _EffectGO.SetSimulationTime(0);
        Selection.activeGameObject = _EffectGO.gameObject;
    }
    private void DestroyEffectEntity()
    {
        if (_EffectGO == null)
            return;
        GameObject.DestroyImmediate(_EffectGO.gameObject);
        _EffectGO = null;
    }
}