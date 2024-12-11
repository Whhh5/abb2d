using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PlayerMgr : Singleton<PlayerMgr>
{
    public override EnManagerFuncType FuncType => base.FuncType | EnManagerFuncType.Update;

    private int m_PlayerEntityID = -1;
    private bool m_IsMove = false;
    private Vector3 m_Motion = Vector3.zero;

    public override void OnDisable()
    {
        base.OnDisable();

        ABBInputMgr.Instance.RemoveListanerDown(KeyCode.A, OnClick_KeyCodeDownA);
        ABBInputMgr.Instance.RemoveListanerDown(KeyCode.D, OnClick_KeyCodeDownD);
        ABBInputMgr.Instance.RemoveListanerDown(KeyCode.W, OnClick_KeyCodeDownW);
        ABBInputMgr.Instance.RemoveListanerDown(KeyCode.S, OnClick_KeyCodeDownS);
        ABBInputMgr.Instance.RemoveListanerUp(KeyCode.A, OnClick_KeyCodeUpA);
        ABBInputMgr.Instance.RemoveListanerUp(KeyCode.D, OnClick_KeyCodeUpD);
        ABBInputMgr.Instance.RemoveListanerUp(KeyCode.W, OnClick_KeyCodeUpW);
        ABBInputMgr.Instance.RemoveListanerUp(KeyCode.S, OnClick_KeyCodeUpS);
        ABBInputMgr.Instance.RemoveListanerDown(KeyCode.V, OnClick_KeyCodeDownV);
        ABBInputMgr.Instance.RemoveListanerUp(KeyCode.V, OnClick_KeyCodeUpV);
        ABBInputMgr.Instance.RemoveListanerDown(KeyCode.Space, OnClick_KeyCodeSpace);
        ABBInputMgr.Instance.RemoveListanerUp(KeyCode.Alpha1, OnClick_KeyCodeUp1);
        ABBInputMgr.Instance.RemoveListanerUp(KeyCode.Alpha2, OnClick_KeyCodeUp2);
        ABBInputMgr.Instance.RemoveListanerUp(KeyCode.Alpha3, OnClick_KeyCodeUp3);
        ABBInputMgr.Instance.RemoveListanerUp(KeyCode.Alpha4, OnClick_KeyCodeUp4);
        ABBInputMgr.Instance.RemoveListanerUp(KeyCode.Alpha5, OnClick_KeyCodeUp5);
        ABBInputMgr.Instance.RemoveListanerDown(KeyCode.Alpha1, OnClick_KeyCodeDown1);
        ABBInputMgr.Instance.RemoveListanerDown(KeyCode.Alpha2, OnClick_KeyCodeDown2);
        ABBInputMgr.Instance.RemoveListanerDown(KeyCode.Alpha3, OnClick_KeyCodeDown3);
        ABBInputMgr.Instance.RemoveListanerDown(KeyCode.Alpha4, OnClick_KeyCodeDown4);
        ABBInputMgr.Instance.RemoveListanerDown(KeyCode.Alpha5, OnClick_KeyCodeDown5);
        ABBInputMgr.Instance.RemoveListanerDown(KeyCode.Q, OnClick_KeyCodeDownQ);
    }
    public override async UniTask OnEnableAsync()
    {
        await base.OnEnableAsync();

        ABBInputMgr.Instance.AddListanerDown(KeyCode.A, OnClick_KeyCodeDownA);
        ABBInputMgr.Instance.AddListanerDown(KeyCode.D, OnClick_KeyCodeDownD);
        ABBInputMgr.Instance.AddListanerDown(KeyCode.W, OnClick_KeyCodeDownW);
        ABBInputMgr.Instance.AddListanerDown(KeyCode.S, OnClick_KeyCodeDownS);
        ABBInputMgr.Instance.AddListanerUp(KeyCode.A, OnClick_KeyCodeUpA);
        ABBInputMgr.Instance.AddListanerUp(KeyCode.D, OnClick_KeyCodeUpD);
        ABBInputMgr.Instance.AddListanerUp(KeyCode.W, OnClick_KeyCodeUpW);
        ABBInputMgr.Instance.AddListanerUp(KeyCode.S, OnClick_KeyCodeUpS);
        ABBInputMgr.Instance.AddListanerDown(KeyCode.V, OnClick_KeyCodeDownV);
        ABBInputMgr.Instance.AddListanerUp(KeyCode.V, OnClick_KeyCodeUpV);
        ABBInputMgr.Instance.AddListanerDown(KeyCode.Space, OnClick_KeyCodeSpace);
        ABBInputMgr.Instance.AddListanerUp(KeyCode.Alpha1, OnClick_KeyCodeUp1);
        ABBInputMgr.Instance.AddListanerUp(KeyCode.Alpha2, OnClick_KeyCodeUp2);
        ABBInputMgr.Instance.AddListanerUp(KeyCode.Alpha3, OnClick_KeyCodeUp3);
        ABBInputMgr.Instance.AddListanerUp(KeyCode.Alpha4, OnClick_KeyCodeUp4);
        ABBInputMgr.Instance.AddListanerUp(KeyCode.Alpha5, OnClick_KeyCodeUp5);
        ABBInputMgr.Instance.AddListanerDown(KeyCode.Alpha1, OnClick_KeyCodeDown1);
        ABBInputMgr.Instance.AddListanerDown(KeyCode.Alpha2, OnClick_KeyCodeDown2);
        ABBInputMgr.Instance.AddListanerDown(KeyCode.Alpha3, OnClick_KeyCodeDown3);
        ABBInputMgr.Instance.AddListanerDown(KeyCode.Alpha4, OnClick_KeyCodeDown4);
        ABBInputMgr.Instance.AddListanerDown(KeyCode.Alpha5, OnClick_KeyCodeDown5);
        ABBInputMgr.Instance.AddListanerDown(KeyCode.Q, OnClick_KeyCodeDownQ);

    }

    public void CreatePlayerEntity(Vector3 startPos)
    {
        m_PlayerEntityID = EntityMgr.Instance.CreateEntityData<PlayerEntityData>();
        var playerEntityData = EntityMgr.Instance.GetEntityData<PlayerEntityData>(m_PlayerEntityID);
        playerEntityData.SetPosition(startPos);
        playerEntityData.AddEntityCom<Entity3DAnimComData>();
        playerEntityData.AddEntityCom<EntityCCComData>();
        playerEntityData.AddMonition<EntityDirectionMonitorData>();
        EntityMgr.Instance.LoadEntity(m_PlayerEntityID);

        var animCom = playerEntityData.GetEntityCom<Entity3DAnimComData>();
        animCom.AddAnim(EnLoadTarget.Anim_Battle_idle);
    }
    public void DestroyPlayer()
    {
        var playerEntityData = EntityMgr.Instance.GetEntityData<PlayerEntityData>(m_PlayerEntityID);
        playerEntityData.RemoveEntityCom<Entity3DAnimComData>();
        playerEntityData.RemoveEntityCom<EntityCCComData>();
        playerEntityData.RemoveMonitor<EntityDirectionMonitorData>();
        EntityMgr.Instance.UnloadEntity(m_PlayerEntityID);
        EntityMgr.Instance.RecycleEntityData(m_PlayerEntityID);
        m_PlayerEntityID = -1;
    }
    public void SetPlayMove(bool isMove)
    {
        m_IsMove = isMove;
    }
    public void IncrementMovePlayer(Vector3 value)
    {
        if (m_PlayerEntityID < 0)
            return;
        var playerEntityData = EntityMgr.Instance.GetEntityData<PlayerEntityData>(m_PlayerEntityID);
        var rigidCom = playerEntityData.GetEntityCom<EntityCCComData>();
        rigidCom.IncrementMove(value);
    }
    public void Jump(float value)
    {
        if (m_PlayerEntityID < 0)
            return;
        var playerEntityData = EntityMgr.Instance.GetEntityData<PlayerEntityData>(m_PlayerEntityID);
        var rigidCom = playerEntityData.GetEntityCom<EntityCCComData>();
        rigidCom.Jump(value);
    }
    public bool IsGrounded()
    {
        if (m_PlayerEntityID < 0)
            return true;
        var playerEntityData = EntityMgr.Instance.GetEntityData<PlayerEntityData>(m_PlayerEntityID);
        var rigidCom = playerEntityData.GetEntityCom<EntityCCComData>();
        var result = rigidCom.IsGrounded();
        return result;
    }


    private void OnClick_KeyCodeDownA()
    {
        m_MoveDirection += Vector3.left;
    }
    private void OnClick_KeyCodeUpA()
    {
        m_MoveDirection += -Vector3.left;
    }
    private void OnClick_KeyCodeDownD()
    {
        m_MoveDirection += -Vector3.left;
    }
    private void OnClick_KeyCodeUpD()
    {
        m_MoveDirection += Vector3.left;
    }
    private void OnClick_KeyCodeDownW()
    {
        m_MoveDirection += Vector3.forward;
    }
    private void OnClick_KeyCodeUpW()
    {
        m_MoveDirection += -Vector3.forward;
    }
    private void OnClick_KeyCodeDownS()
    {
        m_MoveDirection += -Vector3.forward;
    }
    private void OnClick_KeyCodeUpS()
    {
        m_MoveDirection += Vector3.forward;
    }
    private void OnClick_KeyCodeSpace()
    {
        if (IsGrounded())
        {
            m_VerticalVelocity = m_JumpHeight;
        }
    }

    private void OnClick_KeyCodeDownV()
    {
        var playerEntityData = EntityMgr.Instance.GetEntityData<PlayerEntityData>(m_PlayerEntityID);
        var entityAnimCom = playerEntityData.GetEntityCom<Entity3DAnimComData>();
        entityAnimCom.AddAnim(EnLoadTarget.Anim_Magic_04_5_Loop);
        //entityAnimCom.AddAnim(EnLoadTarget.Anim_Battle_idle);
    }
    private void OnClick_KeyCodeUpV()
    {
        var playerEntityData = EntityMgr.Instance.GetEntityData<PlayerEntityData>(m_PlayerEntityID);
        var entityAnimCom = playerEntityData.GetEntityCom<Entity3DAnimComData>();
        entityAnimCom.RemoveAnim(EnLoadTarget.Anim_Magic_04_5_Loop);
        //entityAnimCom.RemoveAnim(EnLoadTarget.Anim_Battle_idle);

    }

    private PlayableAdapter AddPlayerAnim(EnLoadTarget target)
    {
        var playerEntityData = EntityMgr.Instance.GetEntityData<PlayerEntityData>(m_PlayerEntityID);
        var entityAnimCom = playerEntityData.GetEntityCom<Entity3DAnimComData>();
        return entityAnimCom.AddAnim(target);
    }
    private void RemovePlayerAnim(EnLoadTarget target)
    {
        var playerEntityData = EntityMgr.Instance.GetEntityData<PlayerEntityData>(m_PlayerEntityID);
        var entityAnimCom = playerEntityData.GetEntityCom<Entity3DAnimComData>();
        entityAnimCom.RemoveAnim(target);
    }
    private void RemovePlayerAnim(PlayableAdapter playableAdapter)
    {
        var playerEntityData = EntityMgr.Instance.GetEntityData<PlayerEntityData>(m_PlayerEntityID);
        var entityAnimCom = playerEntityData.GetEntityCom<Entity3DAnimComData>();
        entityAnimCom.RemoveAnim(playableAdapter);
    }

    private void OnClick_KeyCodeDown1()
    {
        AddPlayerAnim(EnLoadTarget.Anim_Attack_01);
    }
    private void OnClick_KeyCodeUp1()
    {
        RemovePlayerAnim(EnLoadTarget.Anim_Attack_01);
    }
    private void OnClick_KeyCodeDown2()
    {
        AddPlayerAnim(EnLoadTarget.Anim_Attack_02);
    }
    private void OnClick_KeyCodeUp2()
    {
        RemovePlayerAnim(EnLoadTarget.Anim_Attack_02);
    }
    private void OnClick_KeyCodeDown3()
    {
        AddPlayerAnim(EnLoadTarget.Anim_Attack_03);
    }
    private void OnClick_KeyCodeUp3()
    {
        RemovePlayerAnim(EnLoadTarget.Anim_Attack_03);
    }
    private void OnClick_KeyCodeDown4()
    {
        AddPlayerAnim(EnLoadTarget.Anim_Attack_04);
    }
    private void OnClick_KeyCodeUp4()
    {
        RemovePlayerAnim(EnLoadTarget.Anim_Attack_04);
    }
    private void OnClick_KeyCodeDown5()
    {
        AddPlayerAnim(EnLoadTarget.Anim_Attack_05);
    }
    private void OnClick_KeyCodeUp5()
    {
        RemovePlayerAnim(EnLoadTarget.Anim_Attack_05);
    }

    private float m_LastAttackTime = -1;
    private int m_AttackCount = 6;
    private float m_Interval = 1f;
    private float m_MinInterval = 0.8f;
    private int m_CurCount = 0;
    private PlayableAdapter m_CurPlayAdapter = null;
    private Dictionary<int, EnLoadTarget> m_Count2Anim = new()
    {
        {0, EnLoadTarget.Anim_Attack_01 },
        {1, EnLoadTarget.Anim_Attack_02 },
        {2, EnLoadTarget.Anim_Attack_03 },
        {3, EnLoadTarget.Anim_Attack_04 },
        {4, EnLoadTarget.Anim_Attack_05 },
        {5, EnLoadTarget.Anim_Attack_06 },
    };
    private void OnClick_KeyCodeDownQ()
    {
        var curTime = ABBUtil.GetGameTimeSeconds();
        var interval = curTime - m_LastAttackTime;
        if (interval < m_MinInterval)
            return;
        if (interval > m_Interval)
            m_CurCount = -1;
        m_LastAttackTime = curTime;
        m_CurCount = (m_CurCount + 1) % m_AttackCount;
        var anim = m_Count2Anim[m_CurCount];
        m_CurPlayAdapter = AddPlayerAnim(anim);
    }

    private PlayableAdapter m_RunPlayableAdapter = null;
    private Vector3 m_MoveDirection;
    private float m_Speed = 5;
    private float m_JumpHeight = 7;
    private float m_VerticalVelocity = -2;
    private float m_Gravity = 20;
    public override void Update()
    {
        base.Update();

        //var curTime = ABBUtil.GetGameTimeSeconds();
        //var interval = curTime - m_LastAttackTime;
        //if (interval > m_Interval * 2)
        //{
        //    if (m_CurPlayAdapter != null)
        //    {
        //        RemovePlayerAnim(m_CurPlayAdapter);
        //        m_CurPlayAdapter = null;
        //    }
        //}

        var timeDelta = ABBUtil.GetTimeDelta();
        if (IsGrounded())
        {
            if (m_VerticalVelocity < -2)
            {
                m_VerticalVelocity = -2;
            }
        }
        else
        {
            m_VerticalVelocity += -m_Gravity * timeDelta;
        }

        if (Mathf.Abs(m_MoveDirection.x) + Mathf.Abs(m_MoveDirection.y) + Mathf.Abs(m_MoveDirection.z) == 0)
        {
            if (m_RunPlayableAdapter != null)
            {
                RemovePlayerAnim(m_RunPlayableAdapter);
                m_RunPlayableAdapter = null;
            }
        }
        else
        {
            if (m_RunPlayableAdapter == null)
            {
                m_RunPlayableAdapter = AddPlayerAnim(EnLoadTarget.Anim_Battle_run);
            }
        }
        IncrementMovePlayer(m_MoveDirection * m_Speed * timeDelta + Vector3.up * m_VerticalVelocity * timeDelta);
    }
}
