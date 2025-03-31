using UnityEngine;
public class EntityElectrifiedLoopHitBuffData : EntityBuffData, IUpdate
{

    private readonly int[] _EffectEntityIDList = new int[3];
    private readonly float _AngleInterval = 1f;
    private readonly float _AtkInterval = 0.1f;
    private readonly int _Count = 3;
    private readonly float _Radius = 2f;
    private readonly int _AtkValue = 20;

    private float _StartTime = -1;
    private float _LastTime = -1;
    private int _EnemyLayer = -1;
    public override void OnPoolDestroy()
    {
        UpdateMgr.Instance.Unregistener(this);
        for (int i = 0; i < _Count; i++)
            EffectMgr.Instance.DestroyEffect(_EffectEntityIDList[i]);
        base.OnPoolDestroy();
        _StartTime
            = _LastTime
            = _EnemyLayer
            = -1;
    }
    public override void OnEnable(int addKey, IEntityBuffParams buffParams)
    {
        base.OnEnable(addKey, buffParams);

        _EnemyLayer = Entity3DMgr.Instance.GetMonsterEnemyLayer(_SourceEntityID);
        for (int i = 0; i < _Count; i++)
        {
            _EffectEntityIDList[i] = EffectMgr.Instance.PlayEffect(16);
        }
        _StartTime = ABBUtil.GetGameTimeSeconds();
        UpdateMgr.Instance.Registener(this);
    }

    public void Update()
    {
        var curTime = ABBUtil.GetGameTimeSeconds();
        var angle = (curTime - _StartTime) / _AngleInterval * Mathf.PI * 2;
        var targetWorldPos = Entity3DMgr.Instance.GetEntityWorldPos(_TargetEntityID);
        for (int i = 0; i < _Count; i++)
        {
            var itemAngle = angle + i * (Mathf.PI * 2 / _Count);
            var localPos = new Vector3(Mathf.Cos(itemAngle) * _Radius, 1, Mathf.Sin(itemAngle) * _Radius);

            var entityData = EntityMgr.Instance.GetEntityData(_EffectEntityIDList[i]);
            entityData.SetPosition(targetWorldPos + localPos);
        }


        if (curTime > _LastTime + _AtkInterval)
        {
            _LastTime = curTime;
            for (int i = 0; i < _Count; i++)
            {
                var entityData = EntityMgr.Instance.GetEntityData(_EffectEntityIDList[i]);
                ref var hits = ref EntityUtil.PhysicsOverlapSphere(out var count, entityData.WorldPos, 0.8f, _EnemyLayer);
                for (int j = 0; j < count; j++)
                {
                    ref var hit = ref hits[j];
                    EffectMgr.Instance.PlayEffectOnce(15, hit.closestPoint);
                    AttackMgr.Instance.AttackEntity(_SourceEntityID, hit.entityID, _AtkValue);
                }
            }
        }
    }
}
