using UnityEngine;
public class EntityBattleElectrificationTraceBuffData : EntityBuffData, IUpdate
{
    private int _TargetEffectEntiyID = -1;
    private int _EffectEntiyID = -1;
    private Vector3 _CurWorldPos = Vector3.zero;
    private readonly int _AtkValue = 1;
    private readonly float _MinDisSqr = 1;
    public override void OnPoolDestroy()
    {
        UpdateMgr.Instance.Unregistener(this);

        EffectMgr.Instance.DestroyEffect(_TargetEffectEntiyID);
        EffectMgr.Instance.DestroyEffect(_EffectEntiyID);
        base.OnPoolDestroy();

        _TargetEffectEntiyID
            = _EffectEntiyID
            = -1;
        _CurWorldPos = Vector3.zero;
    }
    public override void OnEnable(int addKey, IEntityBuffParams buffParams)
    {
        base.OnEnable(addKey, buffParams);

        _TargetEffectEntiyID = EffectMgr.Instance.PlayEffect(31);
        _EffectEntiyID = EffectMgr.Instance.PlayEffect(32);

        var pos = Entity3DMgr.Instance.GetEntityWorldPos(_TargetEntityID);
        var targetEffectEntityData = EntityMgr.Instance.GetEntityData(_TargetEffectEntiyID);
        targetEffectEntityData.SetPosition(pos);

        var entityData = EntityMgr.Instance.GetEntityData(_EffectEntiyID);
        entityData.SetPosition(pos);

        _CurWorldPos = pos;

        UpdateMgr.Instance.Registener(this);
    }

    public void Update()
    {
        var pos = Entity3DMgr.Instance.GetEntityWorldPos(_TargetEntityID);
        var targetEffectEntityData = EntityMgr.Instance.GetEntityData(_TargetEffectEntiyID);
        targetEffectEntityData.SetPosition(pos);

        var effectPos = Vector3.LerpUnclamped(_CurWorldPos, pos, ABBUtil.GetTimeDelta() * 2);
        var entityData = EntityMgr.Instance.GetEntityData(_EffectEntiyID);
        entityData.SetPosition(effectPos);

        if (Vector3.SqrMagnitude(_CurWorldPos - effectPos) < _MinDisSqr)
        {
            AttackMgr.Instance.BuffAttackEntity(_SourceEntityID, _TargetEntityID, _AtkValue);
        }
        _CurWorldPos = effectPos;
    }
}
