using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

public class AICommonStepInfo
{
    public float delayTime;
    public float lastExcuteTime;
    public float radius;
}
public class AIPastorModule : AIModule, IUpdate
{

    private EnGameLayer _TargetLayer = EnGameLayer.None;
    private Dictionary<EnEntityCmd, AICommonStepInfo> _CmdList = null;
    private EnEntityCmd _CurCmd = EnEntityCmd.None;

    private float _LastTeleportTime = -1;
    private float _DelayTeleportTime = 3;

    public override void Finish()
    {
        UpdateMgr.Instance.Unregistener(this);
        _CmdList = null;
        _TargetLayer = EnGameLayer.None;
        _CurCmd = EnEntityCmd.None;
        _LastTeleportTime = -1;
    }
    public override void PreExecute()
    {
        _TargetLayer = EnGameLayer.Player;
        _CmdList = new()
        {
            { EnEntityCmd.PlayerBuff,new(){ delayTime = 15,radius = 1} },
            { EnEntityCmd.PastorWaterBuff,new(){ delayTime = 5,radius = 4} },
            { EnEntityCmd.PastorEcliBuff,new(){ delayTime = 10,radius = 2} },
            { EnEntityCmd.PastorAttackBuff,new(){ delayTime = 7,radius = 3} },
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
        if (_CurCmd > EnEntityCmd.None)
            return false;

        return true;
    }

    public override bool IsExecute()
    {
        return true;
    }

    public override bool IsNextModule()
    {
        if (_CurCmd > EnEntityCmd.None)
            return false;

        return true;
    }


    public void Update()
    {

        if (_CurCmd == EnEntityCmd.PlayerBuff || _CurCmd == EnEntityCmd.None)
            UpdatePlayerBuff();
        if (_CurCmd == EnEntityCmd.PastorEcliBuff || _CurCmd == EnEntityCmd.None)
            UpdatePastorEcliBuff(EnEntityCmd.PastorEcliBuff);
        if (_CurCmd == EnEntityCmd.PastorAttackBuff || _CurCmd == EnEntityCmd.None)
            UpdatePastorEcliBuff(EnEntityCmd.PastorAttackBuff);
        if (_CurCmd == EnEntityCmd.PastorWaterBuff || _CurCmd == EnEntityCmd.None)
            UpdatePastorEcliBuff(EnEntityCmd.PastorWaterBuff);
        if (_CurCmd == EnEntityCmd.None)
            UpdateMove();
    }

    private void UpdatePlayerBuff()
    {
        if (!Entity3DMgr.Instance.TryGetEntitysByLayer(_TargetLayer, out var entityIDList))
            return;

        var curTime = ABBUtil.GetGameTimeSeconds();
        var cmdInfo = _CmdList[EnEntityCmd.PlayerBuff];
        var worldPos = Entity3DMgr.Instance.GetEntityWorldPos(EntityID);
        if (cmdInfo.lastExcuteTime + cmdInfo.delayTime < curTime)
        {
            var targetEntityID = -1;
            var minDis = float.MaxValue;
            var targetPos = Vector3.zero;
            void ExecuteEntityTest(int entityID)
            {
                var maxHealth = Entity3DMgr.Instance.GetEntityMaxHealthValue(entityID);
                var curHealth = Entity3DMgr.Instance.GetEntityHealthValue(entityID);
                if (curHealth >= maxHealth)
                    return;
                var pos = Entity3DMgr.Instance.GetEntityWorldPos(entityID);
                var disSqr = Vector3.SqrMagnitude(worldPos - pos);
                if (disSqr < minDis)
                {
                    targetEntityID = entityID;
                    minDis = disSqr;
                    targetPos = pos;
                }
            }
            foreach (var entityID in entityIDList)
            {
                if (!EntityUtil.IsValid(entityID))
                    continue;
                if (entityID == EntityID)
                    continue;
                ExecuteEntityTest(entityID);
            }
            if (targetEntityID <= 0)
                ExecuteEntityTest(EntityID);
            if (targetEntityID > 0)
            {
                if (minDis > cmdInfo.radius * cmdInfo.radius)
                {
                    var dir = Vector3.Normalize(targetPos - worldPos);
                    Entity3DMgr.Instance.IncrementSetEntityMoveDirection(EntityID, dir);
                    Entity3DMgr.Instance.SetEntityLookAtDirection(EntityID, dir);
                    if (_DelayTeleportTime + _LastTeleportTime < curTime)
                    {
                        Entity3DMgr.Instance.AddEntityCmd(EntityID, EnEntityCmd.Teleport);
                        _LastTeleportTime = curTime;
                    }
                    _CurCmd = EnEntityCmd.PlayerBuff;
                }
                else
                {
                    if (Entity3DMgr.Instance.IsAddEntityCmd(EntityID, EnEntityCmd.PlayerBuff))
                    {
                        cmdInfo.lastExcuteTime = curTime;
                        Entity3DMgr.Instance.AddEntityCmd(EntityID, EnEntityCmd.PlayerBuff);
                    }
                    _CurCmd = EnEntityCmd.None;
                }
            }
            else
            {
                _CurCmd = EnEntityCmd.None;
            }
        }
    }
    private void UpdatePastorEcliBuff(EnEntityCmd cmd)
    {
        if (!Entity3DMgr.Instance.TryGetEntitysByLayer(_TargetLayer, out var entityIDList))
            return;

        var curTime = ABBUtil.GetGameTimeSeconds();
        var cmdInfo = _CmdList[cmd];
        var worldPos = Entity3DMgr.Instance.GetEntityWorldPos(EntityID);
        if (cmdInfo.lastExcuteTime + cmdInfo.delayTime < curTime)
        {
            var targetEntityID = -1;
            var minDis = float.MaxValue;
            var targetPos = Vector3.zero;
            foreach (var entityID in entityIDList)
            {
                if (!EntityUtil.IsValid(entityID))
                    continue;
                if (entityID == EntityID)
                    continue;
                var pos = Entity3DMgr.Instance.GetEntityWorldPos(entityID);
                if (!EntityUtil.PhysicsOverlapSphere1(pos + Vector3.up, 2, 1 << (int)EnGameLayer.Monster, out var monsterID))
                    continue;
                var disSqr = Vector3.SqrMagnitude(worldPos - pos);
                if (disSqr < minDis)
                {
                    targetEntityID = entityID;
                    minDis = disSqr;
                    targetPos = pos;
                }
            }
            if (targetEntityID > 0)
            {
                if (minDis > cmdInfo.radius * cmdInfo.radius)
                {
                    var dir = Vector3.Normalize(targetPos - worldPos);
                    Entity3DMgr.Instance.IncrementSetEntityMoveDirection(EntityID, dir);
                    Entity3DMgr.Instance.SetEntityLookAtDirection(EntityID, dir);
                    if (_DelayTeleportTime + _LastTeleportTime < curTime)
                    {
                        Entity3DMgr.Instance.AddEntityCmd(EntityID, EnEntityCmd.Teleport);
                        _LastTeleportTime = curTime;
                    }
                    _CurCmd = cmd;
                }
                else
                {
                    if (Entity3DMgr.Instance.IsAddEntityCmd(EntityID, cmd))
                    {
                        cmdInfo.lastExcuteTime = curTime;
                        Entity3DMgr.Instance.AddEntityCmd(EntityID, cmd);
                    }
                    _CurCmd = EnEntityCmd.None;
                }
            }
            else
            {
                _CurCmd = EnEntityCmd.None;
            }
        }
    }

    private void UpdateMove()
    {
        var pos = Entity3DMgr.Instance.GetEntityWorldPos(EntityID);
        if (!EntityUtil.PhysicsOverlapSphere1(pos + Vector3.up, 5, 1 << (int)EnGameLayer.Monster, out var entityID))
            return;
        var targetPos = Entity3DMgr.Instance.GetEntityWorldPos(entityID);
        var dir = Vector3.Normalize(pos - targetPos);

        Entity3DMgr.Instance.IncrementSetEntityMoveDirection(EntityID, dir);
        Entity3DMgr.Instance.SetEntityLookAtDirection(EntityID, dir);

        //var curTime = ABBUtil.GetGameTimeSeconds();
        //if (_DelayTeleportTime + _LastTeleportTime < curTime)
        //{
        //    Entity3DMgr.Instance.AddEntityCmd(EntityID, EnEntityCmd.Teleport);
        //    _LastTeleportTime = curTime;
        //}
    }
}
