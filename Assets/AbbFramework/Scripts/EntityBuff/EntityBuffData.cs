using UnityEngine;

public class EntityBuffDataUserData : CustomPoolData
{
    public int entityID;
    public EnBuff buff;
    public override void OnPoolDestroy()
    {
        entityID = -1;
        buff = EnBuff.None;
    }
}
public abstract class EntityBuffData : IGamePool
{
    private EnBuff m_Buff = EnBuff.None;
    private EnBuffType m_BuffType = EnBuffType.None;
    private int m_Count = GlobalConfig.IntM1;
    protected int m_EntityID = GlobalConfig.IntM1;
    protected Entity3DData m_EntityData = null;
    protected EntityAnimComData m_AnimCom = null;
    protected EntityCCComData m_CCCom = null;
    public void OnPoolDestroy()
    {
        m_BuffType = EnBuffType.None;
        m_Buff = EnBuff.None;
        m_Count
            = m_EntityID
            = GlobalConfig.IntM1;
        m_EntityData = null;
        m_CCCom = null;
        m_AnimCom = null;
    }
    public void PoolConstructor()
    {
    }

    public void OnPoolInit(CustomPoolData userData)
    {
        var data = userData as EntityBuffDataUserData;

        m_Count = 0;
        m_EntityID = data.entityID;
        m_Buff = data.buff;
        m_BuffType = BuffMgr.Instance.GetBuffType(data.buff);
        m_EntityData = Entity3DMgr.Instance.GetEntity3DData(data.entityID);
        m_AnimCom = m_EntityData.GetEntityCom<EntityAnimComData>();
        m_CCCom = m_EntityData.GetEntityCom<EntityCCComData>();
    }

    public void OnPoolEnable()
    {
    }

    public void PoolRelease()
    {
    }
    public int GetCount()
    {
        return m_Count;
    }
    public virtual void AddCount()
    {
        m_Count++;
    }
    public virtual void RemoveCount()
    {
        m_Count--;
    }
    public EnBuff GetBuff()
    {
        return m_Buff;
    }
    public EnBuffType GetBuffType()
    {
        return m_BuffType;
    }
    public abstract void OnEnable(IEntityBuffParams buffParams);
    public abstract void OnDisable();
    public bool TryRemoveBuff()
    {
        return true;
    }
    public virtual void ReOnEnable(IEntityBuffParams buffParams)
    {

    }

}