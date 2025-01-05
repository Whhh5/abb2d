using UnityEngine;


public class EntityMoveDownBuffData : EntityBuffData
{
    private EntityMoveDownBuffParams m_MoveDownParams = null;

    public override void OnDisable()
    {
        Entity3DMgr.Instance.SetEntityMoveSpeedIncrements(m_EntityID, -m_MoveDownParams.value);
        GameClassPoolMgr.Instance.Push(m_MoveDownParams);
        m_MoveDownParams = null;
    }

    public override void OnEnable(IEntityBuffParams buffParams)
    {
        m_MoveDownParams = buffParams as EntityMoveDownBuffParams;

        Entity3DMgr.Instance.SetEntityMoveSpeedIncrements(m_EntityID, m_MoveDownParams.value);
    }
}