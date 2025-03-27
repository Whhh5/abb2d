using System.Collections.Generic;
using UnityEngine;



public class BuffMgr : Singleton<BuffMgr>
{
    #region struct
    private class AddBuffInfo : IClassPoolNone
    {
        public int entityID;
        public EnBuff buff;
    }
    private class EntityBuffInfo : IClassPoolDestroy
    {
        //private Dictionary<EnBuffType, List<EnBuff>> _BuffInfo = new();
        private readonly Dictionary<EnBuff, int> _Buff2DataIDDic = new();
        public int Count => _Buff2DataIDDic.Count;

        public void OnPoolDestroy()
        {
            _Buff2DataIDDic.Clear();
        }
        public bool Contains(EnBuff buff)
        {
            if (!_Buff2DataIDDic.ContainsKey(buff))
                return false;
            return true;
        }
        public int GetBuffDataID(EnBuff buff)
        {
            return _Buff2DataIDDic[buff];
        }
        public bool TryGetBuffDataID(EnBuff buff, out int buffDataID)
        {
            if (!_Buff2DataIDDic.TryGetValue(buff, out buffDataID))
                return false;
            return true;
        }
        public void AddBuff(EnBuff buff, int buffDataID)
        {
            _Buff2DataIDDic.Add(buff, buffDataID);
        }
        public void RemoveBuff(EnBuff buff)
        {
            _Buff2DataIDDic.Remove(buff);
        }
    }
    #endregion

    public override EnManagerFuncType FuncType => base.FuncType | EnManagerFuncType.Update;
    private Dictionary<int, AddBuffInfo> _AddKeyData = new();
    private Dictionary<int, EntityBuffInfo> _Entity2BuffInfo = new();
    private Dictionary<int, IEntityBuffData> _ID2BuffDataDic = new();

    private List<int> _TimeBuffList = new();

    public EnBuffType GetBuffType(EnBuff buff)
    {
        return buff switch
        {
            EnBuff.NoJumping => EnBuffType.Persistence,
            EnBuff.NoMovement => EnBuffType.Persistence,
            EnBuff.MovingChanges => EnBuffType.Persistence,
            EnBuff.NoRotation => EnBuffType.Persistence,
            EnBuff.NoGravity => EnBuffType.Persistence,
            EnBuff.PlayerBuff => EnBuffType.Time,
            EnBuff.PlayerBuff_1 => EnBuffType.Time,
            EnBuff.Poison => EnBuffType.Time,
            EnBuff.PoisonSub => EnBuffType.Time,
            EnBuff.Expiosion => EnBuffType.Time,
            EnBuff.PlayerSkill2 => EnBuffType.Persistence,
            _ => EnBuffType.Persistence,
        };
    }
    public IEntityBuffData CreateBuffData(EnBuff buff, int sourceEntityID, int targetEntityID)
    {
        var data = ClassPoolMgr.Instance.Pull<EntityBuffDataUserData>();
        data.targetEntityID = targetEntityID;
        data.sourceEntityID = sourceEntityID;
        data.buff = buff;
        IEntityBuffData buffData = buff switch
        {
            EnBuff.NoMovement => ClassPoolMgr.Instance.Pull<EntityNoMoveBuffData>(data),
            EnBuff.NoJumping => ClassPoolMgr.Instance.Pull<EntityNoJumpBuffData>(data),
            EnBuff.MovingChanges => ClassPoolMgr.Instance.Pull<EntityMoveDownBuffData>(data),
            EnBuff.NoRotation => ClassPoolMgr.Instance.Pull<EntityNoRotationBuffData>(data),
            EnBuff.NoGravity => ClassPoolMgr.Instance.Pull<EntityNoGravityBuffData>(data),
            EnBuff.PlayerBuff => ClassPoolMgr.Instance.Pull<EntityPlayerBuffData>(data),
            EnBuff.PlayerBuff_1 => ClassPoolMgr.Instance.Pull<EntityPlayerBuff_1Data>(data),
            EnBuff.Poison => ClassPoolMgr.Instance.Pull<EntityPoisonBuffData>(data),
            EnBuff.PoisonSub => ClassPoolMgr.Instance.Pull<EntityPoisonSubBuffData>(data),
            EnBuff.PlayerSkill2 => ClassPoolMgr.Instance.Pull<EntityPlayerSkill2BuffData>(data),
            EnBuff.Expiosion => ClassPoolMgr.Instance.Pull<EntityExpiosionBuffData>(data),
            _ => null,
        };
        ClassPoolMgr.Instance.Push(data);
        return buffData;
    }
    public void DestroyBuffData(IEntityBuffParams buffData)
    {
        if (buffData == null)
            return;
        ClassPoolMgr.Instance.Push(buffData);
    }
    public IEntityBuffParams ConvertBuffData(EnBuff buff, int[] arrParams)
    {
        switch (buff)
        {
            case EnBuff.MovingChanges:
                {
                    var param = ClassPoolMgr.Instance.Pull<EntityMoveDownBuffParams>();
                    param.value = arrParams[0] / 100f;
                    return param;
                }
            default:
                return null;
        }
    }
    public void PushConvertBuffData(IEntityBuffParams param)
    {
        ClassPoolMgr.Instance.Push(param);
    }

    public int AddEntityBuff(int sourceEntityID, int targetEntityID, EnBuff buff)
    {
        var addKey = AddEntityBuff(sourceEntityID, targetEntityID, buff, null);
        return addKey;
    }
    public int AddEntityBuff(int sourceEntityID, int targetEntityID, EnBuff buff, IEntityBuffParams buffParams)
    {
        if (!Entity3DMgr.Instance.ContainsEntityCom<EntityBuffComData>(targetEntityID))
            return -1;
        var addKey = ABBUtil.GetTempKey();

        if (!_Entity2BuffInfo.TryGetValue(targetEntityID, out var entityBuffInfo))
        {
            entityBuffInfo = ClassPoolMgr.Instance.Pull<EntityBuffInfo>();
            _Entity2BuffInfo.Add(targetEntityID, entityBuffInfo);
        }

        if (!entityBuffInfo.TryGetBuffDataID(buff, out var buffDataID))
        {
            buffDataID = CreateEntityBuffData(sourceEntityID, targetEntityID, buff);
            var buffData = GetBuffData(buffDataID);
            buffData.OnEnable(addKey, buffParams);
            entityBuffInfo.AddBuff(buff, buffDataID);
        }
        else
        {
            var buffData = GetBuffData(buffDataID);
            buffData.ReOnEnable(addKey, buffParams);
        }

        var buffInfo = ClassPoolMgr.Instance.Pull<AddBuffInfo>();
        buffInfo.buff = buff;
        buffInfo.entityID = targetEntityID;
        _AddKeyData.Add(addKey, buffInfo);

        var buffCom = Entity3DMgr.Instance.GetEntityCom<EntityBuffComData>(targetEntityID);
        buffCom.AddBuff(addKey);

        return addKey;
    }
    public void RemoveEntityBuff(int addBuffKey)
    {
        if (!_AddKeyData.TryGetValue(addBuffKey, out var addInfo))
            return;

        var animCom = Entity3DMgr.Instance.GetEntityCom<EntityBuffComData>(addInfo.entityID);
        animCom.RemoveBuff(addBuffKey);

        _AddKeyData.Remove(addBuffKey);
        if (!_Entity2BuffInfo.TryGetValue(addInfo.entityID, out var entityBuffInfo))
            return;

        if (!entityBuffInfo.TryGetBuffDataID(addInfo.buff, out var buffDataID))
            return;

        var buffData = GetBuffData(buffDataID);

        buffData.OnDisable(addBuffKey);

        if (buffData.IsRemove())
        {
            entityBuffInfo.RemoveBuff(addInfo.buff);
            if (entityBuffInfo.Count == 0)
            {
                _Entity2BuffInfo.Remove(addInfo.entityID);
                ClassPoolMgr.Instance.Push(entityBuffInfo);
            }
            RemoveEntityBuffData(buffDataID);
        }

        ClassPoolMgr.Instance.Push(addInfo);
    }
    public bool ContainsBuff(int addBuffKey)
    {
        if (!_AddKeyData.ContainsKey(addBuffKey))
            return false;
        return true;
    }
    public bool ContainsBuff(int entityID, int addBuffKey)
    {
        if (!_AddKeyData.TryGetValue(addBuffKey, out var addInfo))
            return false;
        if (addInfo.entityID != entityID)
            return false;
        return true;
    }
    public bool ContainsBuff(int entityID, EnBuff buff)
    {
        if (!_Entity2BuffInfo.TryGetValue(entityID, out var buffInfo))
            return false;
        if (!buffInfo.Contains(buff))
            return false;
        return true;
    }
    public EnBuffType GetBuffType(int addKey)
    {
        var buff = GetBuff(addKey);
        var buffType = GetBuffType(buff);
        return buffType;
    }
    public EnBuff GetBuff(int addKey)
    {
        if (!_AddKeyData.TryGetValue(addKey, out var buffInfo))
            return EnBuff.None;
        return buffInfo.buff;
    }
    private IEntityBuffData GetBuffData(int buffDataID)
    {
        return _ID2BuffDataDic[buffDataID];
    }
    private IEntityBuffData GetBuffData(int entityID, EnBuff buff)
    {
        var buffDataID = _Entity2BuffInfo[entityID].GetBuffDataID(buff);
        var data = _ID2BuffDataDic[buffDataID];
        return data;
    }
    public IEntityBuffData GetBuffDataByAddKey(int addKey)
    {
        if (!_AddKeyData.TryGetValue(addKey, out var addInfo))
            return null;
        var data = GetBuffData(addInfo.entityID, addInfo.buff);
        return data;
    }

    private int CreateEntityBuffData(int sourceEntityID, int targetEntityID, EnBuff buff)
    {
        var key = ABBUtil.GetTempKey();
        var buffData = BuffMgr.Instance.CreateBuffData(buff, sourceEntityID, targetEntityID);
        _ID2BuffDataDic.Add(key, buffData);
        return key;
    }

    private void RemoveEntityBuffData(int buffDataID)
    {
        if (!_ID2BuffDataDic.TryGetValue(buffDataID, out var buffData))
            return;
        _ID2BuffDataDic.Remove(buffDataID);
        ClassPoolMgr.Instance.Push(buffData);
    }

    public override void Update()
    {
        base.Update();



    }
}

