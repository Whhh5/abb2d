using UnityEngine;

public class EntityMoveDownBuffParams : IEntityBuffParams<EntityMoveDownBuffParamsUserData>
{
    private float m_Value;

    public void OnPoolDestroy()
    {
        m_Value = -1;
    }

    public void OnPoolEnable()
    {
    }

    public void OnPoolInit(EntityMoveDownBuffParamsUserData userData)
    {
        m_Value = userData.value;
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
