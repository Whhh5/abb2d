using UnityEngine;

public class AIMonsterBattleModule : AIModule, IUpdate
{
    private float _Radius = 5;
    private float _AtkRadius = 3;
    private int _TargetEntityID = -1;
    public override void PreExecute()
    {
    }
    public override void Reexecute()
    {

    }
    public override void Execute()
    {
        UpdateMgr.Instance.Registener(this);
    }

    public override void Finish()
    {
        UpdateMgr.Instance.Unregistener(this);
        _TargetEntityID = -1;
    }


    public override bool IsNextModule()
    {
        if (!EntityUtil.IsValid(_TargetEntityID))
            return true;

        var curPos = Entity3DMgr.Instance.GetEntityWorldPos(GetEntityID());
        var targetPos = Entity3DMgr.Instance.GetEntityWorldPos(_TargetEntityID);
        if (Vector3.SqrMagnitude(curPos - targetPos) > _Radius * _Radius)
            return true;

        return false;
    }
    public override bool IsBreak()
    {
        return true;
    }

    public override bool IsExecute()
    {
        var worldPos = Entity3DMgr.Instance.GetEntityWorldPos(GetEntityID());
        var entityData = Entity3DMgr.Instance.GetMonsterEntityData(GetEntityID());
        if (EntityUtil.PhysicsOverlapSphere1(
            worldPos
            , _Radius
            , entityData.GetEnemyLayer()
            , out var entityID))
        {
            _TargetEntityID = entityID;
            return true;
        }
        return false;
    }


    private EnEntityCmd _CurCmd = EnEntityCmd.None;
    private float _LastBuffTime = 0;
    public void Update()
    {
        if (!EntityUtil.IsValid(_TargetEntityID))
            return;



        //if (!Entity3DMgr.Instance.GetEntityCmdIsEnd(EntityID, _CurCmd))
        //{
        //    Entity3DMgr.Instance.AddEntityCmd(EntityID, _CurCmd);
        //    return;
        //}

        var curPos = Entity3DMgr.Instance.GetEntityWorldPos(EntityID);
        var targetPos = Entity3DMgr.Instance.GetEntityWorldPos(_TargetEntityID);
        var dir = Vector3.Normalize(targetPos - curPos);
        if (Vector3.SqrMagnitude(curPos - targetPos) > _AtkRadius * _AtkRadius)
        {
            Entity3DMgr.Instance.IncrementSetEntityMoveDirection(EntityID, dir);

            Entity3DMgr.Instance.SetEntityLookAtDirection(EntityID, dir);
        }
        else
        {
            var curRot = Entity3DMgr.Instance.GetEntityRotation(EntityID);
            var rot = Quaternion.LookRotation(dir);
            var angle = Quaternion.Angle(Quaternion.Euler(curRot), rot);
            if (angle > 30)
            {
                //var curCmd = Entity3DMgr.Instance.GetEntityCurCmd(EntityID);
                Entity3DMgr.Instance.SetEntityLookAtDirection(EntityID, dir);
            }
            else
            {
                var time = ABBUtil.GetGameTimeSeconds();
                var arrCmd = _LastBuffTime + 5 < time
                    ? new EnEntityCmd[] { EnEntityCmd.Monster0Skill1, EnEntityCmd.Monster0Skill2, EnEntityCmd.Monster0Buff1 }
                    : new EnEntityCmd[] { EnEntityCmd.Monster0Skill1, EnEntityCmd.Monster0Skill2 };
                var index = Random.Range(0, arrCmd.Length);
                _CurCmd = arrCmd[index];
                if (_CurCmd == EnEntityCmd.Monster0Buff1)
                {
                    _LastBuffTime = time;
                }
                Entity3DMgr.Instance.AddEntityCmd(EntityID, _CurCmd);
            }
        }
    }
}
