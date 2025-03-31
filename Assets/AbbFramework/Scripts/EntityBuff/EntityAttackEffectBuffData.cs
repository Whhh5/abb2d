using UnityEngine;
public class EntityAttackEffectBuffData : EntityBuffData
{
    private readonly float _Radius = 3f;
    public override void OnPoolInit(EntityBuffDataUserData userData)
    {
        base.OnPoolInit(userData);
    }
    public override void OnEnable(int addKey, IEntityBuffParams buffParams)
    {
        base.OnEnable(addKey, buffParams);


        var layer = Entity3DMgr.Instance.GetMonsterFriendLayer(_TargetEntityID);
        var pos = Entity3DMgr.Instance.GetEntityWorldPos(_TargetEntityID);
        ref var hits = ref EntityUtil.PhysicsOverlapSphere(out var count, pos, _Radius, layer);
        var param = BuffUtil.ConvertBuffData(EnBuff.AttackEffectLoop, new int[] { 1, 1000 });
        for (int i = 0; i < count; i++)
        {
            ref var hit = ref hits[i];
            BuffMgr.Instance.AddEntityBuff(_TargetEntityID, hit.entityID, EnBuff.AttackEffectLoop, param);
        }
        BuffUtil.PushConvertBuffData(param);

        EffectMgr.Instance.PlayEffectOnce(19, pos);
    }


}
