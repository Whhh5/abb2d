using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class EntityExpiosion2BuffData : EntityBuffData, IUpdate
{
    private float _StartTime = 0;
    private Vector3 _StartPos;
    private Vector3 _Direction;
    private Vector3 _RightDirection;
    private float _Distance = 20;

    private float _Time = 1f;
    private float _IntervalDis = 1f;
    private float _LastSlider = 0;
    private int _Layer;


    public override void OnPoolDestroy()
    {
        UpdateMgr.Instance.Unregistener(this);
        base.OnPoolDestroy();
        _LastSlider = 0;
    }

    public override void OnEnable(int addKey, IEntityBuffParams buffParams)
    {
        base.OnEnable(addKey, buffParams);

        _Direction = Entity3DMgr.Instance.GetEntityForward(_SourceEntityID);
        _RightDirection = Entity3DMgr.Instance.GetEntityRight(_SourceEntityID);
        _StartPos = Entity3DMgr.Instance.GetEntityWorldPos(_SourceEntityID) + _Direction * 1f;
        _StartTime = ABBUtil.GetGameTimeSeconds();
        _Layer = Entity3DMgr.Instance.GetMonsterEnemyLayer(_SourceEntityID);

        UpdateMgr.Instance.Registener(this);
    }


    public void Update()
    {
        if (_LastSlider == 1)
            return;

        var curTime = ABBUtil.GetGameTimeSeconds();
        var slider = Mathf.Clamp01((curTime - _StartTime) / _Time);


        if (slider != 1 && (_IntervalDis / _Distance) > (slider - _LastSlider))
            return;

        _LastSlider = slider;

        if (slider == 1)
        {
            var effectPos = _StartPos + _Direction * _Distance;
            EffectMgr.Instance.PlayEffectOnce(12, effectPos);
        }
        else
        {
            var pos = _Direction * _Distance * slider;

            var posXOffset = Mathf.Sin(Mathf.PI * slider) * 5;

            for (int i = -1; i < 2; i += 2)
            {
                var effectPos = _StartPos + pos + _RightDirection * posXOffset * i;

                EffectMgr.Instance.PlayEffectOnce(11, effectPos);

                ref var arrEntity = ref EntityUtil.PhysicsOverlapSphere(out var entityCount, effectPos, 3, _Layer);
                for (int j = 0; j < entityCount; j++)
                {
                    ref var info = ref arrEntity[j];
                    AttackMgr.Instance.AttackEntity(_TargetEntityID, info.entityID, 10);
                    EffectMgr.Instance.PlayEffectOnce(9, info.closestPoint);
                    BuffMgr.Instance.AddEntityBuff(_TargetEntityID, info.entityID, EnBuff.Expiosion);
                }
            }
        }
    }
}
