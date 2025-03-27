using UnityEngine;

public class EntityNoRotationBuffData : EntityBuffData
{
    public override void OnPoolDestroy()
    {
        Entity3DMgr.Instance.SetEntityRotationStatus(_TargetEntityID, true);
        base.OnPoolDestroy();
    }

    public override void OnEnable(int addKey, IEntityBuffParams buffParams)
    {
        Entity3DMgr.Instance.SetEntityRotationStatus(_TargetEntityID, false);
    }
}