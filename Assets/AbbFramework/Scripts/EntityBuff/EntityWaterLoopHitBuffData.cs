using UnityEngine;
public class EntityWaterLoopHitBuffData : EntityBuffData, IUpdate
{
    private int _EffectEntityID = -1;
    private int _AddKey = -1;
    public override void OnPoolDestroy()
    {
        UpdateMgr.Instance.Unregistener(this);
        EffectMgr.Instance.DestroyEffect(_EffectEntityID);
        BuffMgr.Instance.RemoveEntityBuff(_AddKey);
        base.OnPoolDestroy();
        _AddKey
            = _EffectEntityID
            = -1;
    }

    public override void OnEnable(int addKey, IEntityBuffParams buffParams)
    {
        base.OnEnable(addKey, buffParams);

        var userData = BuffUtil.ConvertBuffData(EnBuff.MovingChanges, new int[] { 1, -50 });
        _AddKey = BuffMgr.Instance.AddEntityBuff(_SourceEntityID, _TargetEntityID, EnBuff.MovingChanges, userData);
        BuffUtil.PushConvertBuffData(userData);


        _EffectEntityID = EffectMgr.Instance.PlayEffect(14);

        var pos = Entity3DMgr.Instance.GetEntityWorldPos(_TargetEntityID);
        EffectMgr.Instance.PlayEffectOnce(18, pos);

        UpdateMgr.Instance.Registener(this);
    }

    public void Update()
    {
        var pos = Entity3DMgr.Instance.GetEntityWorldPos(_TargetEntityID);
        var entityData = EntityMgr.Instance.GetEntityData(_EffectEntityID);
        entityData.SetPosition(pos);
    }
}
