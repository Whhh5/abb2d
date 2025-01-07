using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ABBEventDataData : CustomPoolData
{
    public EnABBEvent abbevent;
    public override void OnPoolDestroy()
    {
        abbevent = EnABBEvent.EVENTDEFAULT;
    }
}
public class ABBEventMgr : Singleton<ABBEventMgr>
{
    private Dictionary<EnABBEvent, ABBEventData> m_ActionList = new();



    public void Register(EnABBEvent ev, int sourceID, int typeID, IABBEventExecute action)
    {
        if (!m_ActionList.TryGetValue(ev, out var evData))
        {
            var data = GameClassPoolMgr.Instance.Pull<ABBEventDataData>();
            data.abbevent = ev;
            evData = GameClassPoolMgr.Instance.Pull<ABBEventData>();
            GameClassPoolMgr.Instance.Push(data);
            m_ActionList.Add(ev, evData);
        }
        evData.AddEvent(sourceID, typeID, action);
    }
    public void Unregister(EnABBEvent ev, int sourceID, int typeID, IABBEventExecute action)
    {
        if (!m_ActionList.TryGetValue(ev, out var evData))
            return;
        evData.RemoveEvent(sourceID, typeID, action);
        if(evData.Count == 0)
        {
            m_ActionList.Remove(ev);
            GameClassPoolMgr.Instance.Push(evData);
        }
    }
    public void FireExecute(EnABBEvent ev, int sourceID, int typeID, object userData)
    {
        if (!m_ActionList.TryGetValue(ev, out var evData))
            return;
        evData.FireEvent(sourceID, typeID, userData);
    }
}

