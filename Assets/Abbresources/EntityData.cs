using System;
using UnityEngine;

public class EntityData: IGamePool
{
	protected int m_EntityID;
	public int EntityID => m_EntityID;
    protected int m_GOID;
    public int GOID=> m_GOID;
    protected bool m_IsLoadSuccess = false;
    public bool IsLoadSuccess => m_IsLoadSuccess;
    private EnLoadTarget m_LoadTarget = EnLoadTarget.None;
    public EnLoadTarget LoadTarget=> m_LoadTarget;
	private EnLoadStatus m_LoadStatus = EnLoadStatus.None;
    public EnLoadStatus LoadStatus => m_LoadStatus;
	private Vector3 m_WorldPos;
	public Vector3 WorldPos => m_WorldPos;
	private Transform m_ParentTran = null;
	public Transform ParentTran => m_ParentTran;
	protected Entity m_Entity = null;
    public virtual void OnPoolRecycle()
	{
		m_EntityID
			= m_GOID
			= -1;
		m_IsLoadSuccess = false;
		m_LoadStatus = EnLoadStatus.None;
		m_WorldPos = Vector3.zero;
        m_LoadTarget = EnLoadTarget.None;
		m_ParentTran = null;
    }

	public virtual void OnGOCreate()
    {
        var go = ABBGOMgr.Instance.GetGo(m_GOID);
        m_Entity = go.GetComponent<Entity>();
    }
    public virtual void OnGODestroy()
    {
		m_Entity = null;
    }





    public virtual void SetEntityID(int entityID)
	{
		m_EntityID = entityID;
    }
    public void SetLoadStatus(EnLoadStatus loadStatus)
    {
        m_LoadStatus = loadStatus;
    }
    public void SetLoadTarget(EnLoadTarget loadTarget)
    {
        m_LoadTarget = loadTarget;
    }
    public virtual void SetGOID(int goID)
	{
		m_GOID = goID;
    }
	public void SetIsLoadSuccess(bool isloadSuccess)
	{
		m_IsLoadSuccess = isloadSuccess;
    }
	public void SetPosition(Vector3 worldPos)
	{
		m_WorldPos = worldPos;
		if (m_IsLoadSuccess)
			m_Entity.SetPosition();
    }
	public void SetParentTran(Transform parentTran)
	{
		m_ParentTran = parentTran;
		if (m_IsLoadSuccess)
			m_Entity.SetParentTran();
    }
}

