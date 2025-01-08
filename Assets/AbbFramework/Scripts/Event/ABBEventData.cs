﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ABBEventData : IGamePool
{
    private int m_Count = 0;
    public int Count => m_Count;
    private EnABBEvent m_EventType = EnABBEvent.EVENTDEFAULT;

    private Dictionary<int, Dictionary<int, HashSet<IABBEventExecute>>> m_EventList = new();

    public void OnPoolDestroy()
    {
        m_Count = 0;
        m_EventList.Clear();
    }

    public void PoolConstructor()
    {
    }

    public void OnPoolInit<T>(ref T userData) where T : struct, IPoolUserData
    {
        if (userData is not ABBEventDataUserData data)
            return;
        //var data = (ABBEventDataUserData)userData;
        m_EventType = data.abbevent;
    }

    public void OnPoolEnable()
    {
    }

    public void PoolRelease()
    {
    }
    public void AddEvent(int sourceID, int typeID, IABBEventExecute eventExecute)
    {
        m_Count++;
        if (!m_EventList.TryGetValue(sourceID, out var sourceList))
        {
            sourceList = new();
            m_EventList.Add(sourceID, sourceList);
        }
        if(!sourceList.TryGetValue(typeID, out var eventList))
        {
            eventList = new();
            sourceList.Add(typeID, eventList);
        }
        eventList.Add(eventExecute);
    }
    public void RemoveEvent(int sourceID, int typeID, IABBEventExecute eventExecute)
    {
        if (!m_EventList.TryGetValue(sourceID, out var sourceList))
            return;
        if (!sourceList.TryGetValue(typeID, out var eventList))
            return;
        eventList.Remove(eventExecute);
        m_Count--;
    }
    public void FireEvent(int sourceID, int typeID, object userData)
    {
        if(sourceID > 0)
        {
            if (!m_EventList.TryGetValue(sourceID, out var sourceList))
                return;
            if(typeID > 0)
            {
                if (!sourceList.TryGetValue(typeID, out var eventList))
                    return;
                foreach (var item in eventList)
                {
                    item.EventExecute(m_EventType, sourceID, typeID, userData);
                }
            }
            else
            {
                foreach (var typeList in sourceList)
                {
                    foreach (var item in typeList.Value)
                    {
                        item.EventExecute(m_EventType, sourceID, typeID, userData);
                    }
                }
            }
        }else
        {
            foreach (var sourceList in m_EventList)
            {
                if (typeID > 0)
                {
                    if (!sourceList.Value.TryGetValue(typeID, out var eventList))
                        return;
                    foreach (var item in eventList)
                    {
                        item.EventExecute(m_EventType, sourceID, typeID, userData);
                    }
                }
                else
                {
                    foreach (var typeList in sourceList.Value)
                    {
                        foreach (var item in typeList.Value)
                        {
                            item.EventExecute(m_EventType, sourceID, typeID, userData);
                        }
                    }
                }
            }
        }
    }
}

