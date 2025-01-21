using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


public class LayerMixerInfo : IClassPool<LayerMixerInfoUserData>
{
    private EnAnimLayer m_Layer = EnAnimLayer.None;
    private ScriptPlayable<BridgePlayableAdapter> m_LayerAdapter;
    private List<int> m_UnuseIndex = new(GlobalConfig.Int2);
    private int m_InputPortCount = 0;
    public EnAnimLayerStatus m_Status = EnAnimLayerStatus.None;
    private Dictionary<int, PlayableAdapter> m_Port2Adapter = new();

    public void OnPoolDestroy()
    {
        m_Port2Adapter.Clear();
        m_Status = EnAnimLayerStatus.None;
        m_LayerAdapter = default;
        m_InputPortCount = 0;
        m_Layer = EnAnimLayer.None;
        m_UnuseIndex.Clear();
    }
    public void PoolConstructor()
    {
    }

    public void OnPoolInit(LayerMixerInfoUserData userData)
    {
        m_LayerAdapter = userData.layerAdapter;
        m_Layer = userData.layer;
    }

    public void OnPoolEnable()
    {
    }

    public void PoolRelease()
    {
    }
    public int GetConnectCount()
    {
        var count = m_InputPortCount - m_UnuseIndex.Count;
        return count;
    }
    private int GetInputPort()
    {
        if (m_UnuseIndex.Count == 0)
        {
            var index = m_InputPortCount++;
            m_LayerAdapter.SetInputCount(m_InputPortCount);
            return index;
        }
        var port = m_UnuseIndex[^1];
        m_UnuseIndex.RemoveAt(m_UnuseIndex.Count - 1);
        return port;
    }
    private void RecycleInputPort(int index)
    {
        m_UnuseIndex.Add(index);
    }
    public PlayableAdapter GetAdapter(int portID)
    {
        if (!m_Port2Adapter.TryGetValue(portID, out var adapter))
            return null;
        return adapter;
    }
    public void Connect(int portID, PlayableAdapter adapter, int weight = GlobalConfig.Int1)
    {
        var playable = adapter.GetPlayable();
        m_LayerAdapter.ConnectInput(portID, playable, GlobalConfig.Int0, weight);
        m_Port2Adapter.Add(portID, adapter);
    }
    public int Connect(PlayableAdapter adapter, int weight = GlobalConfig.Int1)
    {
        var portID = GetInputPort();
        Connect(portID, adapter, weight);
        return portID;
    }
    public void Disconnect(int inputPortID)
    {
        var adapter = DisconnectNoDestroy(inputPortID);
        PlayableAdapter.Destroy(adapter);
    }

    public PlayableAdapter DisconnectNoDestroy(int inputPortID)
    {
        if (!m_Port2Adapter.TryGetValue(inputPortID, out var adapter))
            return null;
        m_Port2Adapter.Remove(inputPortID);
        m_LayerAdapter.DisconnectInput(inputPortID);
        RecycleInputPort(inputPortID);
        return adapter;
    }
    public void DisconnectAll()
    {
        var idList = new List<int>(m_Port2Adapter.Count);
        foreach (var item in m_Port2Adapter)
            idList.Add(item.Key);
        foreach (var item in idList)
            Disconnect(item);
    }
    public bool IsStatus(EnAnimLayerStatus target)
    {
        return m_Status == target;
    }
    public void SetStatus(EnAnimLayerStatus status)
    {
        m_Status = status;
    }
    public void SetSpeed(int portID, float speed)
    {
        if (!m_Port2Adapter.TryGetValue(portID, out var adapter))
            return;
        adapter.SetSpeed(speed);
    }
    public bool ContainsPortID(int portID)
    {
        var result = m_Port2Adapter.ContainsKey(portID);
        return result;
    }

}