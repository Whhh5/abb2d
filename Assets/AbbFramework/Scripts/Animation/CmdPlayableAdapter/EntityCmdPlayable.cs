using UnityEngine;

public class EntityCmdPlayable : PlayableBehaviourAdapter
{
    protected int m_EntityID = -1;
    protected PlayableGraphAdapter m_GraphAdapter = null;
    public override void OnPoolRecycle()
    {
        base.OnPoolRecycle();
        m_EntityID = -1;
        m_GraphAdapter = null;
    }
    public virtual void Init(int entityID, PlayableGraphAdapter graphAdapter)
    {
        m_EntityID = entityID;
        m_GraphAdapter = graphAdapter;
    }
    public virtual void ReAddCmd()
    {

    }
}


public class EntityAttackCmdPlayable : EntityCmdPlayable
{

}