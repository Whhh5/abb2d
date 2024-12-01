using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerEntityData: EntityData
{
    private PlayerEntity m_PlayerEntity => m_Entity as PlayerEntity;

    public int m_Num = 0;
    public int Num => m_Num;
    public override void OnPoolRecycle()
    {
        m_Num = 0;
        base.OnPoolRecycle();
    }
    public override void OnGOCreate()
    {
        base.OnGOCreate();
        CameraMgr.Instance.SetLookAtTran(m_GOID);
    }
    public void SetNum(int num)
    {
        m_Num = num;
        if (m_IsLoadSuccess)
            m_PlayerEntity.SetNum();
    }
    public void IncrementMove(Vector3 value)
    {
        if (!m_IsLoadSuccess)
            return;
        m_PlayerEntity.IncrementMove(value);
    }
}
public class PlayerEntity : Entity
{
    private PlayerEntityData m_PlayerData => m_EntityData as PlayerEntityData;
    [SerializeField]
    private TextMeshPro m_TMP = null;
    [SerializeField]
    private CharacterController m_ChaCol = null;
    public override void LoadCompeletion()
    {
        base.LoadCompeletion();
        SetNum();
    }
    public void SetNum()
    {
        m_TMP.text = m_PlayerData.Num.ToString();
    }
    public void IncrementMove(Vector3 value)
    {
        m_ChaCol.Move(value);
    }
}
