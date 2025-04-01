
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public interface ICustomSimulationEditor
{
    public void UpdateSimulation(Rect rect, float itemStartTime, float itemEndTime);
    public void DestroySimulation();
}
public partial class PhysicsResolveSphereEditor : PhysicsResolveSphere, ISkillTypeEditor, ICustomSimulationEditor
{
    public void InitEditor()
    {

    }
    public void Draw()
    {
        EditorGUILayout.BeginVertical();
        {
            m_Radius = EditorGUILayout.FloatField("半径", m_Radius, GUILayout.Width(300));
            m_PosOffsetZ = EditorGUILayout.FloatField("Z", m_PosOffsetZ, GUILayout.Width(300));
            m_PosOffsetX = EditorGUILayout.FloatField("X", m_PosOffsetX, GUILayout.Width(300));
            m_PosOffsetY = EditorGUILayout.FloatField("Y", m_PosOffsetY, GUILayout.Width(300));
        }
        EditorGUILayout.EndVertical();
    }

    public void GetStringData(ref List<int> data)
    {
        var index = data.Count;
        data.Add(Mathf.RoundToInt(m_PosOffsetX * 100));
        data.Add(Mathf.RoundToInt(m_PosOffsetY * 100));
        data.Add(Mathf.RoundToInt(m_PosOffsetZ * 100));
        data.Add(Mathf.RoundToInt(m_Radius * 100));

        data.Insert(index, data.Count - index);
    }


}
public partial class PhysicsResolveSphereEditor
{
    private GameObject _GO = null;
    private Vector3 _CreatePos = Vector3.zero;
    public void UpdateSimulation(Rect rect, float itemStartTime, float itemEndTime)
    {
        InitSimulation();

        var localPos = _GO.transform.position - _CreatePos;
        m_PosOffsetX = localPos.x;
        m_PosOffsetY = localPos.y;
        m_PosOffsetZ = localPos.z;

        m_Radius = _GO.transform.localScale.x / 2;
    }

    public void InitSimulation()
    {
        if (_GO != null)
            return;
        var targetGO = SkillWindowEditor._PrefabObj;
        var assetCfg = ExcelUtil.GetCfg<AssetCfg>((int)EnLoadTarget.Pre_DrawSphere);
        var ass = AssetDatabase.LoadAssetAtPath<GameObject>(assetCfg.strPath);
        _GO = GameObject.Instantiate(ass);
        _CreatePos = targetGO.transform.position;
        _GO.transform.position = _CreatePos + new Vector3(m_PosOffsetX, m_PosOffsetY, m_PosOffsetZ);
        _GO.transform.localScale = m_Radius * 2 * Vector3.one;
    }
    public void DestroySimulation()
    {
        if (_GO == null)
            return;

        GameObject.DestroyImmediate(_GO);
        _GO = null;
    }
}
