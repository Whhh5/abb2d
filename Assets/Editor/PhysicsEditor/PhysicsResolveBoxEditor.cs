
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public partial class PhysicsResolveBoxEditor : PhysicsResolveBox, ISkillTypeEditor, ICustomSimulationEditor
{

    public void InitEditor()
    {

    }
    public void Draw()
    {
        EditorGUILayout.BeginVertical();
        {
            var att = m_CenterType.GetType().GetCustomAttribute<EnumNameAttribute>();
            m_CenterType = (EnPhysicsBoxCenterType)EditorGUILayout.EnumPopup(att?.name, m_CenterType, GUILayout.Width(300));

            var att2 = m_ExecuteType.GetType().GetCustomAttribute<EnumNameAttribute>();
            m_ExecuteType = (EnPhysicsBoxType)EditorGUILayout.EnumPopup(att2?.name, m_ExecuteType, GUILayout.Width(300));

            if (!(GUI.enabled = m_ExecuteType == EnPhysicsBoxType.Successive))
            {
                m_ExecuteTime = 0;
                m_UnitSizeZ = m_BoxSize.z;
            }

            m_ExecuteTime = EditorGUILayout.FloatField("检测时间", m_ExecuteTime, GUILayout.Width(300));
            m_UnitSizeZ = EditorGUILayout.Slider("检测一次单位大小", m_UnitSizeZ, 0f, m_BoxSize.z, GUILayout.Width(300));

            GUI.enabled = true;

            m_BoxSize = EditorGUILayout.Vector3Field("大小", m_BoxSize, GUILayout.Width(300));
            m_RotOffset = EditorGUILayout.Vector3Field("旋转偏移", m_RotOffset, GUILayout.Width(300));
            m_PosOffsetZ = EditorGUILayout.FloatField("位置偏移Z", m_PosOffsetZ, GUILayout.Width(300));
            m_PosOffsetY = EditorGUILayout.FloatField("位置偏移Y", m_PosOffsetY, GUILayout.Width(300));
            m_PosOffsetX = EditorGUILayout.FloatField("位置偏移X", m_PosOffsetX, GUILayout.Width(300));
        }
        EditorGUILayout.EndVertical();
    }

    public void GetStringData(ref List<int> data)
    {
        var index = data.Count;
        data.Add(Mathf.RoundToInt(m_PosOffsetX * 100));
        data.Add(Mathf.RoundToInt(m_PosOffsetY * 100));
        data.Add(Mathf.RoundToInt(m_PosOffsetZ * 100));
        data.Add((int)m_CenterType);
        data.Add((int)m_ExecuteType);
        data.Add(Mathf.RoundToInt(m_ExecuteTime * 100));
        data.Add(Mathf.RoundToInt(m_UnitSizeZ * 100));
        data.Add(Mathf.RoundToInt(m_BoxSize.x * 100));
        data.Add(Mathf.RoundToInt(m_BoxSize.y * 100));
        data.Add(Mathf.RoundToInt(m_BoxSize.z * 100));
        data.Add(Mathf.RoundToInt(m_RotOffset.x * 100));
        data.Add(Mathf.RoundToInt(m_RotOffset.y * 100));
        data.Add(Mathf.RoundToInt(m_RotOffset.z * 100));

        data.Insert(index, data.Count - index);
    }
}

public partial class PhysicsResolveBoxEditor
{
    private GameObject _GO = null;
    private Vector3 _CreateForward = Vector3.zero;
    private Vector3 _CreateUp = Vector3.zero;
    private Vector3 _CreateRight = Vector3.zero;
    private Vector3 _CreatePos = Vector3.zero;
    private Vector3 _CreateRot = Vector3.zero;
    public void UpdateSimulation(Rect rect, float itemStartTime, float itemEndTime)
    {
        InitSimulation();


        var interval = (m_CenterType == EnPhysicsBoxCenterType.Center) ? 0 : (m_BoxSize.z / 2);

        var localPos = _GO.transform.position - _CreatePos - _CreateForward * interval;

        m_PosOffsetX = Vector3.Dot(localPos, _CreateRight);
        m_PosOffsetY = Vector3.Dot(localPos, _CreateUp);
        m_PosOffsetZ = Vector3.Dot(localPos, _CreateForward);
        m_BoxSize = _GO.transform.localScale;
        m_RotOffset = _GO.transform.rotation.eulerAngles - _CreateRot;
    }

    public void InitSimulation()
    {
        if (_GO != null)
            return;
        var targetGO = SkillWindowEditor._PrefabObj;
        var assetCfg = ExcelUtil.GetCfg<AssetCfg>((int)EnLoadTarget.Pre_DrawBox);
        var ass = AssetDatabase.LoadAssetAtPath<GameObject>(assetCfg.strPath);
        _GO = GameObject.Instantiate(ass);
        _CreatePos = targetGO.transform.position;
        var interval = (m_CenterType == EnPhysicsBoxCenterType.Center) ? 0 : (m_BoxSize.z / 2);
        _CreateRight = targetGO.transform.right;
        _CreateUp = targetGO.transform.up;
        _CreateForward = targetGO.transform.forward;
        var posX = _CreateRight * m_PosOffsetX;
        var posY = _CreateUp * m_PosOffsetY;
        var posZ = _CreateForward * (m_PosOffsetZ + interval);
        _GO.transform.position = _CreatePos + posX + posY + posZ;
        _GO.transform.localScale = m_BoxSize;
        _CreateRot = targetGO.transform.localRotation.eulerAngles;
        _GO.transform.rotation = Quaternion.Euler(_CreateRot + m_RotOffset);
    }
    public void DestroySimulation()
    {
        if (_GO == null)
            return;

        GameObject.DestroyImmediate(_GO);
        _GO = null;
    }
}
