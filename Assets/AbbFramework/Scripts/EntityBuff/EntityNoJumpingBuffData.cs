﻿using UnityEngine;
public class EntityNoJumpingBuffData : EntityBuffData
{
    public override void OnPoolDestroy()
    {
        var ccCom = Entity3DMgr.Instance.GetEntityCom<EntityCCComData>(_TargetEntityID);
        ccCom.SetJumpStatus(true);

        base.OnPoolDestroy();
    }

    public override void OnEnable(int addKey, IEntityBuffParams buffParams)
    {
        base.OnEnable(addKey, buffParams);
        var ccCom = Entity3DMgr.Instance.GetEntityCom<EntityCCComData>(_TargetEntityID);
        var curVelocity = ccCom.GetVerticalVelocity();
        ccCom.SetVerticalVelocity(Mathf.Min(curVelocity, 0));
        ccCom.SetJumpStatus(false);
    }
}
