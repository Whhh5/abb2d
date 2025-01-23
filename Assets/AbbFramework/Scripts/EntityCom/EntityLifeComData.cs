using UnityEngine;

public sealed class EntityLifeComData : IEntity3DComData<PoolNaNUserData>
{
    private int m_HealthPoint = -1;
    private int m_MaxHealthPoint = -1;

    public void OnPoolInit(PoolNaNUserData userData)
    {
    }

    public void OnPoolDestroy()
    {
    }

    public void OnCreateGO(int entityID)
    {
        
    }

    public void PoolConstructor()
    {
    }

    public void OnPoolEnable()
    {
    }

    public void PoolRelease()
    {
    }

    public void OnDestroyGO(int entityID)
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