using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public sealed class EntityBuffComData : Entity3DComData, IUpdate
{
    private class BuffTimeInfo : IClassPoolUserData
    {
        public List<int> addKey = new();
        public float endTime;

        public void OnPoolDestroy()
        {
            addKey.Clear();
            endTime = 1;
        }
    }
    private int m_EntityID = -1;

    private Dictionary<EnBuff, BuffTimeInfo> _TimeBuffList = new();
    private List<EnBuff> _BuffList = new();
    private int _BuffListIndex = 0;
    public override void OnPoolDestroy()
    {
        if (_BuffList.Count > 0)
        {
            UpdateMgr.Instance.Unregistener(this);
            while (_BuffList.Count > 0)
            {
                var buffInfo = _TimeBuffList[_BuffList[^1]];

                while (buffInfo.addKey.Count > 0)
                {
                    BuffMgr.Instance.RemoveEntityBuff(buffInfo.addKey[0]);
                }
            }
        }
        base.OnPoolDestroy();
        m_EntityID = -1;
        _TimeBuffList.Clear();
        _BuffList.Clear();
    }
    public override void OnPoolInit(Entity3DComDataUserData userData)
    {
        base.OnPoolInit(userData);
        m_EntityID = userData.entityID;
    }

    public void AddBuff(int addKey)
    {
        var buffType = BuffMgr.Instance.GetBuffType(addKey);
        if (buffType == EnBuffType.Time)
        {
            var buff = BuffMgr.Instance.GetBuff(addKey);
            var time = 7f;
            if (!_TimeBuffList.TryGetValue(buff, out var buffInfo))
            {
                buffInfo = ClassPoolMgr.Instance.Pull<BuffTimeInfo>();
                _TimeBuffList.Add(buff, buffInfo);
                _BuffList.Add(buff);
                if (_TimeBuffList.Count == 1)
                {
                    UpdateMgr.Instance.Registener(this);
                }
            }
            buffInfo.endTime = ABBUtil.GetGameTimeSeconds() + time;
            if (!buffInfo.addKey.Contains(addKey))
            {
                buffInfo.addKey.Add(addKey);
            }
        }
    }
    public void RemoveBuff(int addKey)
    {
        var buff = BuffMgr.Instance.GetBuff(addKey);
        if (!_TimeBuffList.TryGetValue(buff, out var buffInfo))
            return;
        if (!buffInfo.addKey.Remove(addKey))
            return;
        if(buffInfo.addKey.Count == 0)
        {
            ClassPoolMgr.Instance.Push(buffInfo);
            _TimeBuffList.Remove(buff);
            _BuffList.Remove(buff);
            if (_TimeBuffList.Count == 0)
            {
                UpdateMgr.Instance.Unregistener(this);
            }
        }
    }

    public void Update()
    {
        if (_BuffList.Count > 0)
        {
            _BuffListIndex %= _BuffList.Count;
            var buff = _BuffList[_BuffListIndex++];
            var buffInfo = _TimeBuffList[buff];

            if (ABBUtil.GetGameTimeSeconds() > buffInfo.endTime)
            {
                while (buffInfo.addKey.Count > 0)
                {
                    BuffMgr.Instance.RemoveEntityBuff(buffInfo.addKey[0]);
                }
            }

        }
    }
}
