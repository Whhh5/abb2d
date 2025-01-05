using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackLinkSkillData : ISkillData
{
    protected List<AttackLinkItemData> m_DataList = new();
    protected Dictionary<EnBuff, int[]> m_BuffList = new();
    public void InitData(int[] data)
    {
        var arrIndex = 0;
        var atkCount = data?[arrIndex++];
        for (int i = 0; i < atkCount; i++)
        {
            var atkData = GameClassPoolMgr.Instance.Pull<AttackLinkItemData>();
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
    public void AddAttackData(AttackLinkItemData atkData)
    {
        m_DataList.Add(atkData);
    }
    public int GetAttackDataCount()
    {
        return m_DataList.Count;
    }
    public AttackLinkItemData GetAttackData(int index)
    {
        return m_DataList[index];
    }
    public void RemoveAttackData(int index)
    {
        m_DataList.RemoveAt(index);
    }
    public void DestroyData()
    {
        foreach (var item in m_DataList)
        {
            item.Destroy();
            GameClassPoolMgr.Instance.Push(item);
        }
        m_DataList.Clear();
        m_BuffList.Clear();
    }
    public int GetCount()
    {
        return m_DataList.Count;
    }
    public AttackLinkItemData GetData(int index)
    {
        return m_DataList[index];
    }

    public void OnEnable(int entityID)
    {
        foreach (var item in m_BuffList)
        {
            var buffDataParams = BuffMgr.Instance.GetBuffData(item.Key, item.Value);
            Entity3DMgr.Instance.AddEntityBuff(entityID, item.Key, buffDataParams);
        }
    }
    public void OnDisable(int entityID)
    {
        foreach (var item in m_BuffList)
        {
            Entity3DMgr.Instance.RemoveEntityBuff(entityID, item.Key);
        }
    }
}
