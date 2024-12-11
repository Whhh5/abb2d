using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerEntityData : Entity3DData
{
    private PlayerEntity m_PlayerEntity => m_Entity as PlayerEntity;
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_PlayerEntity;


    public override void OnPoolRecycle()
    {
        base.OnPoolRecycle();
    }
    public override void OnGOCreate()
    {
        base.OnGOCreate();
        CameraMgr.Instance.SetLookAtTran(m_GOID);
    }
    public override void OnGODestroy()
    {
        CameraMgr.Instance.ClearLookAt();
        base.OnGODestroy();
    }

    #region rigidbody

    #endregion

}
public class PlayerEntity : Entity3D, IEntity3DCCCom
{
    private PlayerEntityData m_PlayerData = null;
    [SerializeField]
    private CharacterController m_CharacterController = null;
    public override void OnUnload()
    {
        m_PlayerData = null;
        base.OnUnload();
    }
    public override void LoadCompeletion()
    {
        base.LoadCompeletion();
        m_PlayerData = m_EntityData as PlayerEntityData;
    }
    public CharacterController GetCC()
    {
        return m_CharacterController;
    }
}
