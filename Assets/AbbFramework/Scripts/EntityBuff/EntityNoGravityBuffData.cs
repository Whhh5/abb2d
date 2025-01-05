using UnityEngine;

public class EntityNoGravityBuffData : EntityBuffData
{
    public override void OnDisable()
    {
        Entity3DMgr.Instance.SetEntityIsGravity(m_EntityID, true);
    }

    public override void OnEnable(IEntityBuffParams buffParams)
    {
        Entity3DMgr.Instance.SetEntityIsGravity(m_EntityID, false);
        Entity3DMgr.Instance.SetEntityVerticalVelocity(m_EntityID, 0);
        
    }
}