using UnityEngine;
public class EntityPoisonSubBuffData : EntityBuffData, IUpdate
{
    private float _LastTime = -1;
    private readonly float _Insterval = 1f;

    private int _LayerCount = 0;

    private int _EffectEntityID = -1;
    public override void OnPoolDestroy()
    {
        EffectMgr.Instance.DestroyEffect(_EffectEntityID);
        UpdateMgr.Instance.Unregistener(this);

        base.OnPoolDestroy();
        _LastTime
            = _EffectEntityID
            = _LayerCount
            = -1;
    }
    public override void OnEnable(int addKey, IEntityBuffParams buffParams)
    {
        base.OnEnable(addKey, buffParams);
        _LayerCount = 1;
        _EffectEntityID = EffectMgr.Instance.PlayEffect(4);


        UpdateMgr.Instance.Registener(this);
    }
    public override void ReOnEnable(int addKey, IEntityBuffParams buffParams)
    {
        base.ReOnEnable(addKey, buffParams);

        _LayerCount++;
    }

    public void Update()
    {
        var worldPos = Entity3DMgr.Instance.GetEntityWorldPos(_TargetEntityID);
        var effectEntityData = EntityMgr.Instance.GetEntityData(_EffectEntityID);
        effectEntityData.SetPosition(worldPos + Vector3.up);

        if (_LastTime + _Insterval > ABBUtil.GetGameTimeSeconds())
            return;
        _LastTime = ABBUtil.GetGameTimeSeconds();

        AttackMgr.Instance.BuffAttackEntity(_SourceEntityID, _TargetEntityID, _LayerCount);
    }
}
