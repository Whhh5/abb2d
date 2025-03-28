using System.Collections.Generic;
using UnityEngine;

public class EntityPoisonBuffData : EntityBuffData, IUpdate
{
    private class PoisonInfo : IClassPoolDestroy
    {
        public float _MinRadius = 1;
        public float _MaxRadius = -1;
        public int _EffectEntityID = -1;
        public float _LastTime = -1;
        public float _StartTime = -1;
        public float _Time = -1;
        public float _Interval = -1;

        public int _SourceEntityID = -1;
        public int _TargetEntityID = -1;

        public int _TargetLayer = -1;
        public Vector3 _TargetPos = Vector3.zero;

        public void OnPoolDestroy()
        {
            EffectMgr.Instance.DestroyEffect(_EffectEntityID);
            _MinRadius = 1;

            _LastTime
                = _StartTime
                = _MaxRadius
                = _Interval
                = _Time
                = _TargetLayer
                = _SourceEntityID
                = _EffectEntityID
                = _TargetEntityID
                = -1;
            _TargetPos = Vector3.zero;
        }
        public bool IsFinish()
        {
            return ABBUtil.GetGameTimeSeconds() > (_StartTime + _Time);
        }
        public void Update()
        {
            var slider = (ABBUtil.GetGameTimeSeconds() - _StartTime) / _Time;
            var radius = Mathf.Lerp(_MinRadius, _MaxRadius, Mathf.Clamp01(slider * _Time));

            if (ABBUtil.GetGameTimeSeconds() > _LastTime + _Interval)
            {
                _LastTime = ABBUtil.GetGameTimeSeconds();

                ref var entityList = ref EntityUtil.PhysicsOverlapSphere(out var count, _TargetPos, radius, _TargetLayer);
                for (int i = 0; i < count; i++)
                {
                    ref var entityInfo = ref entityList[i];
                    var userData = BuffUtil.ConvertBuffData(EnBuff.PoisonSub, new int[] { 1, 1000 });
                    BuffMgr.Instance.AddEntityBuff(_TargetEntityID, entityInfo.entityID, EnBuff.PoisonSub, userData);
                    BuffUtil.PushConvertBuffData(userData);
                }
            }
        }

    }

    private readonly float _MinRadius = 1;
    private readonly float _MaxRadius = 5 / 2f;
    private readonly float _Time = 10;
    private readonly float _Interval = 0.5f;

    private readonly List<int> _AddKeyList = new(2);
    private int _AddKeyListUpdateIndex = 0;
    private readonly Dictionary<int, PoisonInfo> _AddKey2PoisonInfo = new();

    public override void OnPoolDestroy()
    {
        while (_AddKeyList.Count > 0)
            UnloadEffect(_AddKeyList[^1]);
        UpdateMgr.Instance.Unregistener(this);
        base.OnPoolDestroy();
        _AddKeyList.Clear();
        _AddKey2PoisonInfo.Clear();
        _AddKeyListUpdateIndex = 0;
    }
    public override void OnEnable(int addKey, IEntityBuffParams buffParams)
    {
        base.OnEnable(addKey, buffParams);


        LoadEffect(addKey);

        UpdateMgr.Instance.Registener(this);
    }
    public override void ReOnEnable(int addKey, IEntityBuffParams buffParams)
    {
        base.ReOnEnable(addKey, buffParams);
        LoadEffect(addKey);

    }

    public override bool OnDisable(int addKey)
    {
        if (!base.OnDisable(addKey))
            return false;
        UnloadEffect(addKey);
        return true;
    }
    private void LoadEffect(int addKey)
    {
        var targetPos = Entity3DMgr.Instance.GetEntityWorldPos(_TargetEntityID);

        var poisonInfo = ClassPoolMgr.Instance.Pull<PoisonInfo>();
        poisonInfo._EffectEntityID = EffectMgr.Instance.PlayEffect(3);
        poisonInfo._StartTime = ABBUtil.GetGameTimeSeconds();
        poisonInfo._Time = _Time;
        poisonInfo._MinRadius = _MinRadius;
        poisonInfo._MaxRadius = _MaxRadius;
        poisonInfo._SourceEntityID = _SourceEntityID;
        poisonInfo._TargetEntityID = _TargetEntityID;
        poisonInfo._Interval = _Interval;
        poisonInfo._TargetPos = targetPos;
        poisonInfo._TargetLayer = Entity3DMgr.Instance.GetMonsterEnemyLayer(_TargetEntityID);


        var entityData = EntityMgr.Instance.GetEntityData(poisonInfo._EffectEntityID);
        entityData.SetPosition(targetPos + Vector3.up);

        _AddKeyList.Add(addKey);
        _AddKey2PoisonInfo.Add(addKey, poisonInfo);
    }
    private void UnloadEffect(int addKey)
    {
        if (!_AddKey2PoisonInfo.TryGetValue(addKey, out var poisonInfo))
            return;
        ClassPoolMgr.Instance.Push(poisonInfo);
        _AddKey2PoisonInfo.Remove(addKey);
        _AddKeyList.Remove(addKey);
    }


    public void Update()
    {
        var count = _AddKeyList.Count;
        if (count == 0)
            return;

        _AddKeyListUpdateIndex %= count;
        var addKey = _AddKeyList[_AddKeyListUpdateIndex];
        var poison = _AddKey2PoisonInfo[addKey];
        if (poison.IsFinish())
            UnloadEffect(addKey);
        else
        {
            poison.Update();
            _AddKeyListUpdateIndex++;
        }
    }
}
