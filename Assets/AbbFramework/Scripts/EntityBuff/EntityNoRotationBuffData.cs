using UnityEngine;

public class EntityNoRotationBuffData : EntityBuffData
{
    public override void OnDisable()
    {
        Entity3DMgr.Instance.SetEntityRotationStatus(m_EntityID, true);
    }

    public override void OnEnable(IEntityBuffParams buffParams)
    {
        Entity3DMgr.Instance.SetEntityRotationStatus(m_EntityID, false);
    }
}