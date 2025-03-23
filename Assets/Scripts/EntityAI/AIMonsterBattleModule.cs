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
            , ref _TargetEntityID))
        {
            return true;
        }
        return false;
    }

    public void Update()
    {
        if (!EntityUtil.IsValid(_TargetEntityID))
            return;
        var curPos = Entity3DMgr.Instance.GetEntityWorldPos(EntityID);
        var targetPos = Entity3DMgr.Instance.GetEntityWorldPos(_TargetEntityID);
        var dir = Vector3.Normalize(targetPos - curPos);
        if (Vector3.SqrMagnitude(curPos - targetPos) > _AtkRadius * _AtkRadius)
        {
            Entity3DMgr.Instance.IncrementSetEntityMoveDirection(EntityID, dir);

            Entity3DMgr.Instance.SetEntityMoveDirection(EntityID, dir);
        }
        else
        {

            var curRot = Entity3DMgr.Instance.GetEntityRotation(EntityID);
            var rot = Quaternion.LookRotation(dir);
            var angle = Quaternion.Angle(Quaternion.Euler(curRot), rot);
            if (angle > 30)
            {
                //var curCmd = Entity3DMgr.Instance.GetEntityCurCmd(EntityID);
                Entity3DMgr.Instance.SetEntityMoveDirection(EntityID, dir);
            }
            else
            {
                var cmd = Random.Range(0, 1f) > 0.5f ? EnEntityCmd.Monster0Skill1 : EnEntityCmd.Monster0Skill2;
                Entity3DMgr.Instance.AddEntityCmd(EntityID, cmd);
            }

        }
    }
}
