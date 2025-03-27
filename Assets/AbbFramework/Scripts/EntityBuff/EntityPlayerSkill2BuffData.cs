using System.Collections.Generic;
using UnityEngine;

public class EntityPlayerSkill2BuffData : EntityBuffData, IUpdate
{
    private int _Count = 10;
    private List<int> _EffectList = new(new int[10]);
    private int _EffectListIndex = 0;

    private float _Interval = 0.1f;
    private float _StartTime = 0;
    private float _LastTime = 0;
    public override void OnPoolDestroy()
    {
        for (int i = 0; i < _EffectList.Count; i++)
        {
            if (EffectMgr.Instance.IsValid(_EffectList[i]))
                EffectMgr.Instance.DestroyEffect(_EffectList[i]);
            _EffectList[i] = 0;
        }

        UpdateMgr.Instance.Unregistener(this);
        base.OnPoolDestroy();
        _StartTime
            = -1;
        _LastTime
            = _EffectListIndex
            = 0;
    }
    public override void OnPoolInit(EntityBuffDataUserData userData)
    {
        base.OnPoolInit(userData);
        _StartTime = ABBUtil.GetGameTimeSeconds();

        UpdateMgr.Instance.Registener(this);
    }


    public void Update()
    {
        for (int i = 0; i < _Count; i++)
        {
            var effectEntityID = _EffectList[i];
            if (!EffectMgr.Instance.IsValid(effectEntityID))
                break;
            var worldPos = Entity3DMgr.Instance.GetEntityWorldPos(_TargetEntityID);
            var entityData = EntityMgr.Instance.GetEntityData(effectEntityID);
            entityData.SetPosition(worldPos + Vector3.up);
        }


        var time = ABBUtil.GetGameTimeSeconds() - _StartTime;
        var count = time / _Interval;
        var createCount = Mathf.FloorToInt(count - _LastTime / _Interval);
        if (createCount <= 0)
            return;
        _LastTime = time;
        for (int i = 0; i < createCount; i++)
        {
            var rot = Random.Range(0, 360);
            var index = (_EffectListIndex + i) % _Count;
            var effectID = _EffectList[index];
            if (effectID > 0)
                EffectMgr.Instance.DestroyEffect(effectID);
            effectID = EffectMgr.Instance.PlayEffect(5);
            _EffectList[_EffectListIndex + i] = effectID;

            var worldPos = Entity3DMgr.Instance.GetEntityWorldPos(_TargetEntityID);
            var entityData = EntityMgr.Instance.GetEntityData(effectID);
            entityData.SetLocalRotation(Vector3.up * rot);
            entityData.SetPosition(worldPos + Vector3.up);
        }
        _EffectListIndex = (_EffectListIndex + createCount) % _Count;
    }
}
