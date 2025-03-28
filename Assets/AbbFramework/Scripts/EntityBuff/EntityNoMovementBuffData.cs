using UnityEngine;
public class EntityNoMovementBuffData : EntityBuffData
{
    public override void OnPoolDestroy()
    {
        var ccCom = Entity3DMgr.Instance.GetEntityCom<EntityCCComData>(_TargetEntityID);
        ccCom.SetMoveStatus(true);
        base.OnPoolDestroy();
    }

    public override void OnEnable(int addKey, IEntityBuffParams buffParams)
    {
        var ccCom = Entity3DMgr.Instance.GetEntityCom<EntityCCComData>(_TargetEntityID);
        ccCom.SetMoveStatus(false);
    }
}
