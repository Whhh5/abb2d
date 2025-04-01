using System.Collections.Generic;
using UnityEngine;

public class AIBlueWarriorBattleModule : AIModule, IUpdate
{
    private Dictionary<EnEntityCmd, AICommonStepInfo> _CmdList = null;
    private EnEntityCmd _CurCmd = EnEntityCmd.None;

    private float _LastTeleportTime = -1;
    private readonly float _DelayTeleportTime = 3;

    private int _Skill1TargetEntityID = -1;

    private bool _IsAttacking = false;
    public override void Finish()
    {
        UpdateMgr.Instance.Unregistener(this);
        _CmdList = null;
        _CurCmd = EnEntityCmd.None;
        _LastTeleportTime = -1;
        _Skill1TargetEntityID = -1;
    }
    public override void PreExecute()
    {
        _CmdList = new()
        {
            { EnEntityCmd.BlueWarriorAttack,new(){ delayTime = 0.5f,radius = 1} },
            { EnEntityCmd.BlueWarriorSkill1,new(){ delayTime = 5,radius = 4} },
        };
    }
    public override void Execute()
    {
        UpdateMgr.Instance.Registener(this);
    }
    public override void Reexecute()
    {
    }


    public override bool IsBreak()
    {
        return _CurCmd == EnEntityCmd.None;
    }

    public override bool IsExecute()
    {
        return true;
    }

    public override bool IsNextModule()
    {
        return _CurCmd == EnEntityCmd.None;
    }

    public void Update()
    {
        if (_CurCmd == EnEntityCmd.None || _CurCmd == EnEntityCmd.Run || _CurCmd == EnEntityCmd.BlueWarriorSkill1)
            UpdateSkill1();
        if (_CurCmd == EnEntityCmd.None || _CurCmd == EnEntityCmd.Run || _CurCmd == EnEntityCmd.BlueWarriorAttack)
            UpdateAttack();
        if (_CurCmd == EnEntityCmd.None || _CurCmd == EnEntityCmd.Run)
            UpdateMove();
    }

    private void UpdateSkill1()
    {
        if (!Entity3DMgr.Instance.GetEntityCmdIsEnd(EntityID, EnEntityCmd.BlueWarriorSkill1))
            return;
        var curTime = ABBUtil.GetGameTimeSeconds();
        if (!_CmdList.TryGetValue(EnEntityCmd.BlueWarriorSkill1, out var stepInfo))
            return;
        if (stepInfo.lastExcuteTime + stepInfo.delayTime > curTime)
            return;
        var pos = Entity3DMgr.Instance.GetEntityWorldPos(EntityID);
        if (!EntityUtil.IsValid(_Skill1TargetEntityID))
        {
            if (!EntityUtil.PhysicsOverlapSphere1(pos, 10, 1 << (int)EnGameLayer.Monster, out _Skill1TargetEntityID))
            {
                _CurCmd = EnEntityCmd.None;
                return;
            }
        }
        _CurCmd = EnEntityCmd.BlueWarriorSkill1;
        var targetPos = Entity3DMgr.Instance.GetEntityWorldPos(_Skill1TargetEntityID);
        var dir = Vector3.Normalize(targetPos - pos);
        var forward = Entity3DMgr.Instance.GetEntityForward(EntityID);

        var angle = Vector3.Angle(forward, dir);
        var isRot = angle > 10;
        var isMove = Vector3.SqrMagnitude(targetPos - pos) > 25;
        if (isRot || isMove)
        {
            if (isRot)
                Entity3DMgr.Instance.SetEntityLookAtDirection(EntityID, dir);
            if (isMove)
            {
                if (_LastTeleportTime + _DelayTeleportTime < curTime)
                {
                    Entity3DMgr.Instance.AddEntityCmd(EntityID, EnEntityCmd.Teleport);
                    _LastTeleportTime = curTime;
                }
                Entity3DMgr.Instance.IncrementSetEntityMoveDirection(EntityID, dir);
            }
            return;
        }

        Entity3DMgr.Instance.AddEntityCmd(EntityID, _CurCmd);
        _CurCmd = EnEntityCmd.None;
        stepInfo.lastExcuteTime = curTime;
    }

    private void UpdateAttack()
    {
        var curTime = ABBUtil.GetGameTimeSeconds();
        if (!_CmdList.TryGetValue(EnEntityCmd.BlueWarriorAttack, out var stepInfo))
            return;
        if (stepInfo.lastExcuteTime + stepInfo.delayTime > curTime)
            return;
        if (_CmdList.TryGetValue(EnEntityCmd.BlueWarriorSkill1, out var skillInfo))
        {
            if (skillInfo.lastExcuteTime + skillInfo.delayTime < curTime)
            {
                _CurCmd = EnEntityCmd.None;
                return;
            }
        }


        var pos = Entity3DMgr.Instance.GetEntityWorldPos(EntityID);
        if (!EntityUtil.IsValid(_Skill1TargetEntityID))
        {
            if (!EntityUtil.PhysicsOverlapSphere1(pos, 5, 1 << (int)EnGameLayer.Monster, out _Skill1TargetEntityID))
            {
                _CurCmd = EnEntityCmd.None;
                return;
            }
        }
        _CurCmd = EnEntityCmd.BlueWarriorAttack;
        var targetPos = Entity3DMgr.Instance.GetEntityWorldPos(_Skill1TargetEntityID);
        var dir = Vector3.Normalize(targetPos - pos);
        var forward = Entity3DMgr.Instance.GetEntityForward(EntityID);

        var isMove = Vector3.SqrMagnitude(targetPos - pos) > 9;
        if (!_IsAttacking || isMove || Vector3.Angle(forward, dir) > 70)
        {
            var isAngle = Vector3.Angle(forward, dir) > 3;
            if (isAngle)
                Entity3DMgr.Instance.SetEntityLookAtDirection(EntityID, dir);
            if (isMove)
            {
                if (_LastTeleportTime + _DelayTeleportTime < curTime)
                {
                    Entity3DMgr.Instance.AddEntityCmd(EntityID, EnEntityCmd.Teleport);
                    _LastTeleportTime = curTime;
                }
                Entity3DMgr.Instance.IncrementSetEntityMoveDirection(EntityID, dir);
            }
            _IsAttacking = isAngle || isMove;
            return;
        }

        Entity3DMgr.Instance.AddEntityCmd(EntityID, _CurCmd);
        //_IsAttacking = true;
        stepInfo.lastExcuteTime = curTime;
    }

    private void UpdateMove()
    {
        var playerEntityID = PlayerMgr.Instance.GetCurPlayerEntityID();
        if (!EntityUtil.IsValid(playerEntityID))
        {
            return;
        }

        var pos = Entity3DMgr.Instance.GetEntityWorldPos(EntityID);
        var playerPos = Entity3DMgr.Instance.GetEntityWorldPos(playerEntityID);
        if (Vector3.SqrMagnitude(pos - playerPos) < 25)
        {
            return;
        }

        _CurCmd = EnEntityCmd.Run;
        var curTime = ABBUtil.GetGameTimeSeconds();
        if (_LastTeleportTime + _DelayTeleportTime < curTime)
        {
            Entity3DMgr.Instance.AddEntityCmd(EntityID, EnEntityCmd.Teleport);
            _LastTeleportTime = curTime;
        }

        var dir = Vector3.Normalize(playerPos - pos);
        Entity3DMgr.Instance.IncrementSetEntityMoveDirection(EntityID, dir);
        Entity3DMgr.Instance.SetEntityLookAtDirection(EntityID, dir);
    }
}
