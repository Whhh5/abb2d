using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class EntityWaterBuffData : EntityBuffData, IUpdate
{
    private float _LastTime = -1;
    private int _EffectEntityID = -1;
    private Vector3 _EntityPos;
    private int _Layer = -1;
    private IEntityBuffParams _SubBuffParams = null;

    private readonly float _HalfSize = 7 / 2f;
    private readonly float _Interval = 1f;
    private EntityPhysicsInfo[] _PhysicsHit = new EntityPhysicsInfo[50];
    public override void OnPoolDestroy()
    {
        BuffUtil.PushConvertBuffData(_SubBuffParams);
        EffectMgr.Instance.DestroyEffect(_EffectEntityID);
        UpdateMgr.Instance.Unregistener(this);
        base.OnPoolDestroy();
        _LastTime
            = _EffectEntityID
            = _Layer
            = -1;
        _EntityPos = Vector3.zero;
        _SubBuffParams = null;
    }
    public override void OnEnable(int addKey, IEntityBuffParams buffParams)
    {
        base.OnEnable(addKey, buffParams);


        _EntityPos = Entity3DMgr.Instance.GetEntityWorldPos(_TargetEntityID);
        _Layer = Entity3DMgr.Instance.GetMonsterEnemyLayer(_TargetEntityID);
        _EffectEntityID = EffectMgr.Instance.PlayEffect(13, _EntityPos + Vector3.up * _HalfSize);
        _LastTime = ABBUtil.GetGameTimeSeconds();
        _SubBuffParams = BuffUtil.ConvertBuffData(EnBuff.WaterLoopHit, new int[] { 1, 200 });
        UpdateMgr.Instance.Registener(this);
    }

    public override void ReOnEnable(int addKey, IEntityBuffParams buffParams)
    {
        base.ReOnEnable(addKey, buffParams);

        RecreateEffect();
    }
    private void RecreateEffect()
    {
        EffectMgr.Instance.DestroyEffect(_EffectEntityID);

        _LastTime = ABBUtil.GetGameTimeSeconds();
        _EntityPos = Entity3DMgr.Instance.GetEntityWorldPos(_TargetEntityID);
        _EffectEntityID = EffectMgr.Instance.PlayEffect(13, _EntityPos + Vector3.up * _HalfSize);
    }

    public void Update()
    {
        var curTime = ABBUtil.GetGameTimeSeconds();
        if (curTime < _LastTime + _Interval)
            return;

        var count = EntityUtil.PhysicsOverlapBox(ref _PhysicsHit, _EntityPos, Vector3.one * _HalfSize, Quaternion.identity, _Layer);
        for (int i = 0; i < count; i++)
        {
            ref var hit = ref _PhysicsHit[i];

            BuffMgr.Instance.AddEntityBuff(_TargetEntityID, hit.entityID, EnBuff.WaterLoopHit, _SubBuffParams);
        }
    }
}
