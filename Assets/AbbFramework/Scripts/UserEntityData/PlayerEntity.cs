using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerEntityData : MonsterBaseData
{
    private PlayerEntity m_PlayerEntity => m_Entity as PlayerEntity;


    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
    }
    public override void OnGOCreate()
    {
        base.OnGOCreate();
    }
    public override void OnGODestroy()
    {
        base.OnGODestroy();
    }
}
public class PlayerEntity : MonsterBase, IEntity3DCCCom
{
    [SerializeField]
    private CharacterController m_CharacterController = null;
    [SerializeField]
    private Trigger3D m_IsGroundTrigger = null;
    [SerializeField]
    private Transform m_TopPoint = null;

    public override void OnUnload()
    {
        base.OnUnload();
    }
    public override void LoadCompeletion()
    {
        m_CharacterController.enabled = false;
        base.LoadCompeletion();
        transform.position = m_EntityData.WorldPos;
        m_CharacterController.enabled = true;
    }
    public CharacterController GetCC()
    {
        return m_CharacterController;
    }
    public bool IsGrounded()
    {
        return m_IsGroundTrigger.IsEnter();
    }

    public Vector3 GetTopPoint()
    {
        return m_TopPoint.position;
    }
}
