using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EntityCCComData : IEntity3DComData, IUpdate
{
    private Entity3DData m_Entity3DData = null;
    private IEntity3DCCCom m_RigidCom = null;
    private CharacterController m_CC = null;
    private Transform m_Tran = null;
    public void AddCom(Entity3DData entity3DData)
    {
        m_Entity3DData = entity3DData;

    }

    public void RemomveCom()
    {
        m_Entity3DData = null;
    }

    public void OnCreateGO(Entity3D entity)
    {
        m_RigidCom = entity as IEntity3DCCCom;
        m_CC = m_RigidCom.GetCC();
        m_Tran = entity.transform;
        UpdateMgr.Instance.Registener(this);
    }
    public void OnDestroyGO()
    {
        UpdateMgr.Instance.Unregistener(this);
        m_CC = null;
        m_RigidCom = null;
    }

    public void IncrementMove(Vector3 motion)
    {
        if (!m_Entity3DData.IsLoadSuccess)
            return;
        m_CC.Move(motion);
    }
    public void Jump(float height)
    {
        if (!m_Entity3DData.IsLoadSuccess)
            return;
        m_CC.Move(Vector3.up * height);
    }
    public bool IsGrounded()
    {
        if (!m_Entity3DData.IsLoadSuccess)
            return true;
        return m_CC.isGrounded;
    }
    
    public void Update()
    {
        m_Entity3DData.SetPosition(m_Tran.position);
    }
}
public interface IEntity3DCCCom : IEntity3DCom
{
    public CharacterController GetCC();

}