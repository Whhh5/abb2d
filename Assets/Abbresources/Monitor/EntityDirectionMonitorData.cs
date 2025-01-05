using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnDirectionType
{
    None,
    Left,
    Right,
    Up,
    Down,
    Forward,
    back,
}
public class EntityDirectionMonitorData : IEntityMonitorEntity, IUpdate
{
    private Entity3DData m_Entity3DData = null;
    private Vector3 m_LastPos = Vector3.zero;
    public void StartMonitor(Entity3DData entityeDData)
    {
        m_Entity3DData = entityeDData;
        UpdateMgr.Instance.Registener(this);
        m_LastPos = entityeDData.WorldPos;
    }

    public void StopMonitor()
    {
        UpdateMgr.Instance.Unregistener(this);
        m_Entity3DData = null;
    }

    public void Update()
    {
        if (m_LastPos.x == m_Entity3DData.WorldPos.x && m_LastPos.z == m_Entity3DData.WorldPos.z)
            return;
        var dir = m_Entity3DData.WorldPos - m_LastPos;
        m_LastPos = m_Entity3DData.WorldPos;

        m_Entity3DData.SetDirType(dir);
    }
}
