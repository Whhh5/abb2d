using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PlayerMgr : Singleton<PlayerMgr>
{
    public override EnManagerFuncType FuncType => base.FuncType | EnManagerFuncType.Update;

    private int m_PlayerEntityID = -1;
    private PlayerEntityData m_PlayerEntityData = null;
    private EntityAnimComData m_AnimCom = null;
    private bool m_IsMove = false;
    private Vector3 m_Motion = Vector3.zero;

    public override void OnDisable()
    {
        base.OnDisable();

        ABBInputMgr.Instance.RemoveListaner(KeyCode.A, OnClick_KeyCodeDownA);
        ABBInputMgr.Instance.RemoveListaner(KeyCode.D, OnClick_KeyCodeDownD);
        ABBInputMgr.Instance.RemoveListaner(KeyCode.W, OnClick_KeyCodeDownW);
        ABBInputMgr.Instance.RemoveListaner(KeyCode.S, OnClick_KeyCodeDownS);
        //ABBInputMgr.Instance.RemoveListanerUp(KeyCode.A, OnClick_KeyCodeUpA);
        //ABBInputMgr.Instance.RemoveListanerUp(KeyCode.D, OnClick_KeyCodeUpD);
        //ABBInputMgr.Instance.RemoveListanerUp(KeyCode.W, OnClick_KeyCodeUpW);
        //ABBInputMgr.Instance.RemoveListanerUp(KeyCode.S, OnClick_KeyCodeUpS);
        ABBInputMgr.Instance.RemoveListanerDown(KeyCode.K, OnClick_KeyCodeDownK);
        ABBInputMgr.Instance.RemoveListanerDown(KeyCode.J, OnClick_KeyCodeDownJ);
        ABBInputMgr.Instance.RemoveListanerDown(KeyCode.U, OnClick_KeyCodeDownU);
        ABBInputMgr.Instance.RemoveListaner(KeyCode.O, OnClick_KeyCodeO);
        ABBInputMgr.Instance.RemoveListanerDown(KeyCode.L, OnClick_KeyCodeDownL);
    }
    public override async UniTask OnEnableAsync()
    {
        await base.OnEnableAsync();

        ABBInputMgr.Instance.AddListaner(KeyCode.A, OnClick_KeyCodeDownA);
        ABBInputMgr.Instance.AddListaner(KeyCode.D, OnClick_KeyCodeDownD);
        ABBInputMgr.Instance.AddListaner(KeyCode.W, OnClick_KeyCodeDownW);
        ABBInputMgr.Instance.AddListaner(KeyCode.S, OnClick_KeyCodeDownS);
        //ABBInputMgr.Instance.AddListanerUp(KeyCode.A, OnClick_KeyCodeUpA);
        //ABBInputMgr.Instance.AddListanerUp(KeyCode.D, OnClick_KeyCodeUpD);
        //ABBInputMgr.Instance.AddListanerUp(KeyCode.W, OnClick_KeyCodeUpW);
        //ABBInputMgr.Instance.AddListanerUp(KeyCode.S, OnClick_KeyCodeUpS);
        ABBInputMgr.Instance.AddListanerDown(KeyCode.K, OnClick_KeyCodeDownK);
        ABBInputMgr.Instance.AddListanerDown(KeyCode.J, OnClick_KeyCodeDownJ);
        ABBInputMgr.Instance.AddListanerDown(KeyCode.U, OnClick_KeyCodeDownU);
        ABBInputMgr.Instance.AddListaner(KeyCode.O, OnClick_KeyCodeO);
        ABBInputMgr.Instance.AddListanerDown(KeyCode.L, OnClick_KeyCodeDownL);
        ABBInputMgr.Instance.AddListanerDown(KeyCode.F, OnClick_KeyCodeDownF);
        ABBInputMgr.Instance.AddListanerDown(KeyCode.I, OnClick_KeyCodeDownI);

    }

    public void CreatePlayerEntity(Vector3 startPos)
    {
        m_PlayerEntityID = Entity3DMgr.Instance.CreateEntityData<PlayerEntityData>();
        m_PlayerEntityData = Entity3DMgr.Instance.GetEntity3DData<PlayerEntityData>(m_PlayerEntityID);
        m_PlayerEntityData.SetPosition(startPos);
        m_PlayerEntityData.AddEntityCom<EntityAnimComData>();
        m_PlayerEntityData.AddEntityCom<EntityCCComData>();
        m_PlayerEntityData.AddEntityCom<EntityBuffComData>();
        m_PlayerEntityData.AddMonition<EntityDirectionMonitorData>();
        m_AnimCom = m_PlayerEntityData.GetEntityCom<EntityAnimComData>();
        var ccCom = m_PlayerEntityData.GetEntityCom<EntityCCComData>();
        ccCom.SetGravity(20);
        ccCom.SetIsJumping(false);
        ccCom.SetJumpCount(0);
        ccCom.SetJumpHeight(7);
        ccCom.SetJumpSpeed(1);
        ccCom.SetMoveDirection(Vector3.zero);
        ccCom.SetMoveSpeed(5);
        ccCom.SetMoveSpeedIncrements(1);
        ccCom.SetVerticalVelocity(-2);
        ccCom.SetJumpSpeedIncrements(1);
        ccCom.SetMoveStatus(true);
        ccCom.SetJumpStatus(true);
        ccCom.SetRotationSpeed(1);
        ccCom.SetIsCanRotation(true);
        ccCom.SetIsGravity(true);
        Entity3DMgr.Instance.LoadEntity(m_PlayerEntityID);

        var animCom = m_PlayerEntityData.GetEntityCom<EntityAnimComData>();
        animCom.AddCmd(EnEntityCmd.Idle);
    }
    public void DestroyPlayer()
    {
        m_PlayerEntityData.RemoveEntityCom<EntityAnimComData>();
        m_PlayerEntityData.RemoveEntityCom<EntityCCComData>();
        m_PlayerEntityData.RemoveMonitor<EntityDirectionMonitorData>();
        Entity3DMgr.Instance.UnloadEntity(m_PlayerEntityID);
        Entity3DMgr.Instance.RecycleEntityData(m_PlayerEntityID);
        m_PlayerEntityID = -1;
        m_AnimCom = null;
        m_PlayerEntityData = null;
    }
    public void SetPlayMove(bool isMove)
    {
        m_IsMove = isMove;
    }
    public void IncrementMovePlayer(Vector3 value)
    {
        if (m_PlayerEntityID < 0)
            return;
        var rigidCom = m_PlayerEntityData.GetEntityCom<EntityCCComData>();
        rigidCom.IncrementMove(value);
    }
    public void Jump(float value)
    {
        if (m_PlayerEntityID < 0)
            return;
        var rigidCom = m_PlayerEntityData.GetEntityCom<EntityCCComData>();
        rigidCom.Jump(value);
    }


    private void OnClick_KeyCodeDownA()
    {
        var pos = CameraMgr.Instance.GetCameraRight();
        //pos.y = 0.2f;
        Entity3DMgr.Instance.IncrementSetEntityMoveDirection(m_PlayerEntityID, -pos.normalized);
    }
    private void OnClick_KeyCodeDownD()
    {
        var pos = CameraMgr.Instance.GetCameraRight();
        //pos.y = -0.2f;
        Entity3DMgr.Instance.IncrementSetEntityMoveDirection(m_PlayerEntityID, pos.normalized);
    }
    private void OnClick_KeyCodeDownW()
    {
        var pos = m_PlayerEntityData.WorldPos - CameraMgr.Instance.GetCameraWorldPos();
        pos.y = 0;
        Entity3DMgr.Instance.IncrementSetEntityMoveDirection(m_PlayerEntityID, pos.normalized);
    }
    private void OnClick_KeyCodeDownS()
    {
        var pos = CameraMgr.Instance.GetCameraWorldPos() - m_PlayerEntityData.WorldPos;
        pos.y = 0;
        Entity3DMgr.Instance.IncrementSetEntityMoveDirection(m_PlayerEntityID, pos.normalized);
    }
    private void OnClick_KeyCodeDownK()
    {
        Entity3DMgr.Instance.ExecuteEntityJump(m_PlayerEntityID);
    }

    private void OnClick_KeyCodeDownJ()
    {
        Entity3DMgr.Instance.AddEntityCmd(m_PlayerEntityID, EnEntityCmd.Attack);
    }


    private void OnClick_KeyCodeDownU()
    {
        Entity3DMgr.Instance.AddEntityCmd(m_PlayerEntityID, EnEntityCmd.Skill1);
    }
    private void OnClick_KeyCodeO()
    {
        Entity3DMgr.Instance.AddEntityCmd(m_PlayerEntityID, EnEntityCmd.Skill2);
    }

    private void OnClick_KeyCodeDownL()
    {
        Entity3DMgr.Instance.AddEntityCmd(m_PlayerEntityID, EnEntityCmd.Teleport);
    }

    public void OnClick_KeyCodeDownF()
    {
        //Entity3DMgr.Instance.AddEntityCmd(m_PlayerEntityID, EnEntityCmd.Injured);
        Entity3DMgr.Instance.AddEntityCmd(m_PlayerEntityID, EnEntityCmd.LayerMixer);

        //m_PlayerEntityData.SetPosition(new Vector3(10, 2, 10));
    }
    private bool applyRootMotion = false;
    public void OnClick_KeyCodeDownI()
    {
        //var data = GameClassPoolMgr.Instance.Pull<AttackCmdData>();
        //data.cmd = EnEntityCmd.Skill3;
        //Entity3DMgr.Instance.AddEntityCmd(m_PlayerEntityID, EnEntityCmd.Skill3, data);
        //GameClassPoolMgr.Instance.Push(data);

        applyRootMotion = !applyRootMotion;
        Entity3DMgr.Instance.SetApplyRootMotion(m_PlayerEntityID, applyRootMotion);
    }
}
