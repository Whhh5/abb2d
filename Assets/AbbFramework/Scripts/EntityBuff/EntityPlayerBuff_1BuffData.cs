using UnityEngine;
public class EntityPlayerBuff_1BuffData : EntityBuffData, IUpdate
{
    private int _EffectEntityID = -1;
    private float _LastTime = -1f;
    private readonly float _Interval = 1f;
    public override void OnPoolDestroy()
    {
        Entity3DMgr.Instance.SetEntityMoveSpeedIncrements(_TargetEntityID, -0.5f);
        UpdateMgr.Instance.Unregistener(this);
        EffectMgr.Instance.DestroyEffect(_EffectEntityID);
        base.OnPoolDestroy();
        _LastTime
            = _EffectEntityID
            = -1;
    }
    public override void OnEnable(int addKey, IEntityBuffParams buffParams)
    {
        base.OnEnable(addKey, buffParams);

        _EffectEntityID = EffectMgr.Instance.PlayEffect(2);

        Entity3DMgr.Instance.SetEntityMoveSpeedIncrements(_TargetEntityID, 0.5f);

        UpdateMgr.Instance.Registener(this);
    }

    public void Update()
    {
        if (_TargetEntityID <= 0)
            return;
        var pos = Entity3DMgr.Instance.GetEntityWorldPos(_TargetEntityID);
        var entityData = EntityMgr.Instance.GetEntityData(_EffectEntityID);
        entityData.SetPosition(pos);

        if (_LastTime + _Interval < ABBUtil.GetGameTimeSeconds())
        {
            _LastTime = ABBUtil.GetGameTimeSeconds();


            AttackMgr.Instance.AddHealth(_SourceEntityID, _TargetEntityID, 10);

        }
    }
}
