using UnityEngine;
public class EntityAttackEffectLoopBuffData : EntityBuffData, IUpdate
{
    private int _EffectEntityID = -1;
    private int _DefenseValue = 100;
    public override void OnPoolDestroy()
    {
        var defenseValue = Entity3DMgr.Instance.GetEntityDefenseValue(_TargetEntityID);
        Entity3DMgr.Instance.SetEntityDefenseValue(_TargetEntityID, defenseValue - _DefenseValue);

        EffectMgr.Instance.DestroyEffect(_EffectEntityID);
        UpdateMgr.Instance.Unregistener(this);
        base.OnPoolDestroy();
        _EffectEntityID = -1;
    }
    public override void OnEnable(int addKey, IEntityBuffParams buffParams)
    {
        base.OnEnable(addKey, buffParams);

        _EffectEntityID = EffectMgr.Instance.PlayEffect(20);

        UpdateMgr.Instance.Registener(this);

        var defenseValue = Entity3DMgr.Instance.GetEntityDefenseValue(_TargetEntityID);
        Entity3DMgr.Instance.SetEntityDefenseValue(_TargetEntityID, defenseValue + _DefenseValue);
    }

    public void Update()
    {
        var pos = Entity3DMgr.Instance.GetEntityWorldPos(_TargetEntityID);
        var entityData = EntityMgr.Instance.GetEntityData(_EffectEntityID);
        entityData.SetPosition(pos);
    }
}
