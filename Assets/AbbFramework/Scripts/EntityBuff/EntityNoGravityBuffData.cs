using UnityEngine;

public class EntityNoGravityBuffData : EntityBuffData
{
    public override void OnPoolDestroy()
    {
        Entity3DMgr.Instance.SetEntityIsGravity(_TargetEntityID, true);

        base.OnPoolDestroy();
    }

    public override void OnEnable(int addKey, IEntityBuffParams buffParams)
    {
        Entity3DMgr.Instance.SetEntityIsGravity(_TargetEntityID, false);
        Entity3DMgr.Instance.SetEntityVerticalVelocity(_TargetEntityID, 0);
        
    }
}