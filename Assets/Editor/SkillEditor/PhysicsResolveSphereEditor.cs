
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PhysicsResolveSphereEditor : PhysicsResolveSphere, IEditorItem
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

