using UnityEngine;
public class EntityElectrifiedBuffData : EntityBuffData
{
    private readonly float _Radius = 3;
    public override void OnPoolDestroy()
    {


    }
    public override void OnEnable(int addKey, IEntityBuffParams buffParams)
    {
        base.OnEnable(addKey, buffParams);



        var pos = Entity3DMgr.Instance.GetEntityWorldPos(_TargetEntityID);
        var layer = Entity3DMgr.Instance.GetMonsterFriendLayer(_TargetEntityID);
        ref var entityIDs = ref EntityUtil.PhysicsOverlapSphere(out var count, pos + Vector3.up, _Radius, layer);
        var param = BuffUtil.ConvertBuffData(EnBuff.ElectrifiedLoopHit, new int[] { 1, 1000 });
        for (int i = 0; i < count; i++)
        {
            ref var hit = ref entityIDs[i];

            BuffMgr.Instance.AddEntityBuff(_TargetEntityID, hit.entityID, EnBuff.ElectrifiedLoopHit, param);
        }
        BuffUtil.PushConvertBuffData(param);
        EffectMgr.Instance.PlayEffectOnce(15, pos + Vector3.up);
    }
}
