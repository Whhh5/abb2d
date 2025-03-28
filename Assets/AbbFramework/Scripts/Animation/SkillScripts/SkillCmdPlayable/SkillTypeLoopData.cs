using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTypeLoopData : ISkillTypeData<AttackLinkSkillDataUserData>
{
    protected List<SkillItemInfo> m_DataList = new();
    protected Dictionary<EnBuff, int[]> m_BuffList = new();

    private List<int> _BuffAddKeyList = new();

    public void OnPoolDestroy()
    {
        foreach (var item in m_DataList)
        {
            ClassPoolMgr.Instance.Push(item);
        }
        m_DataList.Clear();
        m_BuffList.Clear();
        _BuffAddKeyList.Clear();
    }

    public void PoolConstructor()
    {
    }

    public void OnPoolInit(AttackLinkSkillDataUserData userData)
    {
        var data = userData.arrParams;
        var arrIndex = 0;
        var atkCount = data?[arrIndex++];
        for (int i = 0; i < atkCount; i++)
        {
            var atkData = ClassPoolMgr.Instance.Pull<SkillItemInfo>();
            var arrCount = data[arrIndex++];
            atkData.Init(data, arrCount, ref arrIndex);
            AddAttackData(atkData);
        }

        var buffCount = arrIndex < data?.Length ? data[arrIndex++] : default;
        for (int i = 0; i < buffCount; i++)
        {
            var buff = (EnBuff)data[arrIndex++];
            var paramCount = data[arrIndex++];
            var arrParams = data.Copy(arrIndex, paramCount);
            arrIndex += paramCount;
            m_BuffList.Add(buff, arrParams);
        }
    }

    public void OnPoolEnable()
    {
    }

    public void PoolRelease()
    {
    }
    public void AddAttackData(SkillItemInfo atkData)
    {
        m_DataList.Add(atkData);
    }
    public int GetAttackDataCount()
    {
        return m_DataList.Count;
    }
    public SkillItemInfo GetAttackData(int index)
    {
        return m_DataList[index];
    }
    public void RemoveAttackData(int index)
    {
        m_DataList.RemoveAt(index);
    }
    public int GetCount()
    {
        return m_DataList.Count;
    }
    public SkillItemInfo GetData(int index)
    {
        return m_DataList[index];
    }

    public void OnEnable(int entityID)
    {
        foreach (var item in m_BuffList)
        {
            var buffDataParams = BuffUtil.ConvertBuffData(item.Key, item.Value);
            var addKey = BuffMgr.Instance.AddEntityBuff(entityID, entityID, item.Key, buffDataParams);
            BuffUtil.PushConvertBuffData(buffDataParams);
            if (BuffMgr.Instance.GetBuffType(addKey) != EnBuffType.Time)
                _BuffAddKeyList.Add(addKey);
        }
    }
    public void OnDisable(int entityID)
    {
        foreach (var addKey in _BuffAddKeyList)
        {
            BuffMgr.Instance.RemoveEntityBuff(addKey);
        }
        _BuffAddKeyList.Clear();
    }
}
