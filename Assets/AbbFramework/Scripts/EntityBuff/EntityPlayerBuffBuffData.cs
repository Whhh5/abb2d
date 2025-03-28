using UnityEngine;
public class EntityPlayerBuffBuffData : EntityBuffData, IUpdate
{
    private int _EffectEntityID = -1;
    private float _LastTime = -1;
    private readonly float _IntervalTime = 1f;
    private Vector3 _TargetPos = Vector3.zero;
    private int _TargetLayer = -1;

    private float _Slider = 1;
    private readonly float _MaxSlider = 6;
    public override void OnPoolDestroy()
    {
        UpdateMgr.Instance.Unregistener(this);
        EffectMgr.Instance.DestroyEffect(_EffectEntityID);
        base.OnPoolDestroy();
        _EffectEntityID = -1;
        _LastTime = -1;
        _TargetPos = Vector3.zero;
        _Slider = 1;
    }
    public override void OnEnable(int addKey, IEntityBuffParams buffParams)
    {
        base.OnEnable(addKey, buffParams);

        LoadEffect();

        UpdateMgr.Instance.Registener(this);
    }

    private void LoadEffect()
    {
        _EffectEntityID = EffectMgr.Instance.PlayEffect(1);

        var playerEntity = EntityMgr.Instance.GetEntityData(_TargetEntityID);
        var playerPos = playerEntity.WorldPos;
        var monsterData = playerEntity as MonsterBaseData;
        var entityData = EntityMgr.Instance.GetEntityData(_EffectEntityID);
        entityData.SetPosition(playerPos);
        _TargetPos = playerPos;
        _TargetLayer = monsterData.GetFriendLayer();
    }

    public override void ReOnEnable(int addKey, IEntityBuffParams buffParams)
    {
        base.ReOnEnable(addKey, buffParams);

        EffectMgr.Instance.DestroyEffect(_EffectEntityID);
        _Slider = 1;
        LoadEffect();
    }

    public void Update()
    {
        if (_Slider != _MaxSlider)
        {
            _Slider = Mathf.Min(_MaxSlider, _Slider + ABBUtil.GetTimeDelta() * 5);
            var entityData = EntityMgr.Instance.GetEntityData(_EffectEntityID);
            entityData.SetLocalScale(Vector3.one * _Slider);
        }


        if (ABBUtil.GetGameTimeSeconds() - _LastTime > _IntervalTime)
        {
            ref var entityList = ref EntityUtil.PhysicsOverlapSphere(out var count, _TargetPos, _Slider * 0.5f, _TargetLayer);
            for (int i = 0; i < count; i++)
            {
                ref var entityInfo = ref entityList[i];

                var userData = BuffUtil.ConvertBuffData(EnBuff.PlayerBuff_1, new int[] { 1, 1000 });
                BuffMgr.Instance.AddEntityBuff(_SourceEntityID, entityInfo.entityID, EnBuff.PlayerBuff_1, userData);
                BuffUtil.PushConvertBuffData(userData);
            }
        }
    }
}
