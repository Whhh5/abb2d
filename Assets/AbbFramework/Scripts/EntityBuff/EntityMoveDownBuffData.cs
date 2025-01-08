using UnityEngine;


public class EntityMoveDownBuffData : EntityBuffData
{
    private EntityMoveDownBuffParams m_MoveDownParams = null;

    public override void OnDisable()
    {
        Entity3DMgr.Instance.SetEntityMoveSpeedIncrements(m_EntityID, -m_MoveDownParams.GetValue());
        ClassPoolMgr.Instance.Push(m_MoveDownParams);
        m_MoveDownParams = null;
    }

    public override void OnEnable(IEntityBuffParams buffParams)
    {
        m_MoveDownParams = buffParams as EntityMoveDownBuffParams;

        Entity3DMgr.Instance.SetEntityMoveSpeedIncrements(m_EntityID, m_MoveDownParams.GetValue());
    }
}