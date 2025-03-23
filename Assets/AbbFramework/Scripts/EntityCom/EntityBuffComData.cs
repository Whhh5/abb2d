using System.Collections.Generic;
using UnityEngine;

public sealed class EntityBuffComData : Entity3DComData
{
    private Dictionary<EnBuffType, List<EnBuff>> m_BuffTypeDic = new();
    private Dictionary<EnBuff, EntityBuffData> m_BuffDic = new();
    private int m_EntityID = -1;
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        m_BuffTypeDic.Clear();
        m_BuffDic.Clear();
        m_EntityID = -1;
    }
    public override void OnPoolInit(Entity3DComDataUserData userData)
    {
        base.OnPoolInit(userData);
        m_EntityID = userData.entityID;
    }


    public bool ContainsBuff(EnBuff buff)
    {
        var contains = m_BuffDic.ContainsKey(buff);
        return contains;
    }
    private bool TryGetBuffData(EnBuff buff, out EntityBuffData buffData)
    {
        if (!m_BuffDic.TryGetValue(buff, out buffData))
            return false;
        return true;
    }
    public EntityBuffData CreateEntityBuffData(EnBuff buff)
    {
        var buffData = BuffMgr.Instance.CreateBuffData(buff, m_EntityID);
        var buffType = buffData.GetBuffType();
        m_BuffDic.Add(buff, buffData);
        if (!m_BuffTypeDic.TryGetValue(buffType, out var list))
        {
            list = new();
            m_BuffTypeDic.Add(buffType, list);
        }
        list.Add(buff);
        return buffData;
    }
    public void RemoveEntityBuffData(EnBuff buff)
    {
        if (!TryGetBuffData(buff, out var buffData))
            return;
        m_BuffDic.Remove(buff);
        var buffType = buffData.GetBuffType();
        var buffList = m_BuffTypeDic[buffType];
        if (buffList.Remove(buff))
            if (m_BuffTypeDic.Count == 0)
                m_BuffTypeDic.Remove(buffType);
        BuffMgr.Instance.DestroyBuffData(buffData);
    }
    public void AddBuff(EnBuff buff, IEntityBuffParams buffParams)
    {
        if (!TryGetBuffData(buff, out var buffData))
        {
            buffData = CreateEntityBuffData(buff);
            buffData.OnEnable(buffParams);
        }
        else
        {
            buffData.ReOnEnable(buffParams);
        }
    }
    public void RemoveBuff(EnBuff buff)
    {
        if (!TryGetBuffData(buff, out var buffData))
            return;
        if (buffData.TryRemoveBuff())
        {
            buffData.OnDisable();
            RemoveEntityBuffData(buff);
        }
    }
}
