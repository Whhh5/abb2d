
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class PhysicsResolveBoxEditor : PhysicsResolveBox, IEditorItem
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

            m_ExecuteTime = EditorGUILayout.FloatField("检测时间", m_ExecuteTime, GUILayout.Width(300));
            m_UnitSizeZ = EditorGUILayout.FloatField("检测一次单位大小", m_UnitSizeZ, GUILayout.Width(300));
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
