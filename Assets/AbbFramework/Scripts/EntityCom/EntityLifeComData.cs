using UnityEngine;

public sealed class EntityLifeComData : IEntity3DComData
{
    private int m_HealthPoint = -1;
    private int m_MaxHealthPoint = -1;

    public void AddCom(Entity3DData entity3DData)
    {
    }

    public void RemomveCom()
    {
    }

    public void OnCreateGO(Entity3D entity)
    {
    }

    public void OnDestroyGO()
    {
    }

    public int GetHealthPoint()
    {
        return m_HealthPoint;
    }
    public void SetHealthPoint(int value)
    {
        m_HealthPoint = Mathf.Clamp(value, 0, m_MaxHealthPoint);
    }
    public void AddHealthPoint(int value)
    {
        SetHealthPoint(m_HealthPoint + value);
    }
    public void RemoveHealthPoint(int value)
    {
        SetHealthPoint(m_HealthPoint + value);
    }
    public void SetMaxHealthPoint(int value)
    {
        m_MaxHealthPoint = value;
    }
    public int GetMaxHealthPoint()
    {
        return m_MaxHealthPoint;
    }
    public float GetHealthPointSchedule()
    {
        var schedule = (float)m_HealthPoint / m_MaxHealthPoint;
        return schedule;
    }
    public bool IsDie()
    {
        return m_HealthPoint == 0;
    }

}