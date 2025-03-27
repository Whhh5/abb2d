using UnityEngine;

public class EntityExpiosionBuffData : EntityBuffData, IUpdate
{
    private int _AtkValue = 10;
    private float _Interval = 1.5f;
    private float _LastTime = -1;

    public override void OnPoolDestroy()
    {
        UpdateMgr.Instance.Unregistener(this);
        base.OnPoolDestroy();
        _LastTime = -1;
    }

    public override void OnEnable(int addKey, IEntityBuffParams buffParams)
    {
        base.OnEnable(addKey, buffParams);

        _LastTime = ABBUtil.GetGameTimeSeconds();

        UpdateMgr.Instance.Registener(this);
    }


    public void Update()
    {
        var curTime = ABBUtil.GetGameTimeSeconds();
        if (_LastTime + _Interval / Count > curTime)
            return;
        _LastTime = curTime;

        var pos = Entity3DMgr.Instance.GetEntityWorldPos(_TargetEntityID);
        EffectMgr.Instance.PlayEffectOnce(10, pos + Vector3.up);

        AttackMgr.Instance.BuffAttackEntity(_SourceEntityID, _TargetEntityID, _AtkValue);
    }
}
