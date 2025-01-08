using UnityEngine;

public class EntityMoveDownBuffParams : IEntityBuffParams
{
    private float m_Value;

    public void OnPoolDestroy()
    {
        m_Value = -1;
    }

    public void OnPoolEnable()
    {
    }

    public void OnPoolInit<T>(ref T userData) where T : struct, IPoolUserData
    {
        if (userData is not EntityMoveDownBuffParamsUserData data)
            return;
        m_Value = data.value;
    }

    public void PoolConstructor()
    {
    }

    public void PoolRelease()
    {
    }
    public float GetValue()
    {
        return m_Value;
    }
}
