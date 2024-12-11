using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using static UnityEditor.Experimental.GraphView.GraphView;


public enum EnAnimLayerStatus
{
    None,
    Entering,
    Playing,
    Exiting,
    Nothing,
}
public class LayerMixerInfo : IGamePool
{
    private EnAnimLayer m_Layer = EnAnimLayer.None;
    private ScriptPlayable<AdapterPlayable> m_LayerAdapter;
    private List<int> m_UnuseIndex = new(GlobalConfig.Int2);
    private int m_InputPortCount = 0;
    public EnAnimLayerStatus m_Status = EnAnimLayerStatus.None;
    private Dictionary<int, PlayableAdapter> m_Port2Adapter = new();

    public void OnPoolRecycle()
    {
        m_Port2Adapter.Clear();
        m_Status = EnAnimLayerStatus.None;
        m_LayerAdapter = default;
        m_InputPortCount = 0;
        m_Layer = EnAnimLayer.None;
        m_UnuseIndex.Clear();
    }
    public int GetConnectCount()
    {
        var count = m_InputPortCount - m_UnuseIndex.Count;
        return count;
    }
    public void InitInfo(EnAnimLayer layer, ScriptPlayable<AdapterPlayable> layerAdapter)
    {
        m_LayerAdapter = layerAdapter;
        m_Layer = layer;
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
public class PlayableLayerMixerAdapter : PlayableAdapter, IUpdate
{
    public static PlayableLayerMixerAdapter Create(int entityID, PlayableGraphAdapter graph)
    {
        var data = GameClassPoolMgr.Instance.Pull<PlayableLayerMixerAdapter>();
        data.Initialization(entityID, graph);
        return data;
    }
    private PlayableGraphAdapter m_GraphAdapter = null;
    private AnimationLayerMixerPlayable m_LayerMixer;
    //private ScriptPlayable<AdapterPlayable> m_Adapter;
    private Dictionary<EnAnimLayer, LayerMixerInfo> m_Layer2unusePortDic = new();
    private Dictionary<PlayableAdapter, int> m_Adapter2PortID = new();

    private List<EnAnimLayer> m_EnterLayerList = new();
    private List<EnAnimLayer> m_ExistLayerList = new();


    public override void OnDestroy()
    {
        m_Adapter2PortID.Clear();
        m_EnterLayerList.Clear();
        m_ExistLayerList.Clear();
        UpdateMgr.Instance.Unregistener(this);
        m_Layer2unusePortDic.Clear();
        m_LayerMixer.Destroy();
        //m_Adapter.Destroy();
        base.OnDestroy();
        m_GraphAdapter = null;
    }

    public override ScriptPlayable<AdapterPlayable> GetPlayable()
    {
        return default;
    }
    public AnimationLayerMixerPlayable GetLayerMixerPlayable()
    {
        return m_LayerMixer;
    }

    public override void Initialization(int entityID, PlayableGraphAdapter graph)
    {
        base.Initialization(entityID, graph);
        m_GraphAdapter = graph;
        m_LayerMixer = AnimationLayerMixerPlayable.Create(graph.GetGraph(), (int)EnAnimLayer.EnumCount);
        //m_Adapter.AddInput(m_LayerMixer, 0, 1);
        UpdateMgr.Instance.Registener(this);
    }
    private LayerMixerInfo CreateLayerMixerInfo(EnAnimLayer layer)
    {
        var layerAdapter = ScriptPlayable<AdapterPlayable>.Create(m_LayerMixer.GetGraph());
        var info = GameClassPoolMgr.Instance.Pull<LayerMixerInfo>();
        m_Layer2unusePortDic.Add(layer, info);
        info.InitInfo(layer, layerAdapter);
        m_LayerMixer.ConnectInput((int)layer, layerAdapter, GlobalConfig.Int0, GlobalConfig.Int0);
        //m_LayerMixer.SetLayerAdditive((uint)layer, layer > EnAnimLayer.Base);
        m_LayerMixer.SetLayerAdditive((uint)layer, false);
        return info;
    }
    private void DestroyLayerMixerInfo(EnAnimLayer layer)
    {
        if (!TryGetLayerMixerInfo(layer, out var layerInfo))
            return;
        m_LayerMixer.DisconnectInput((int)layer);
        m_Layer2unusePortDic.Remove(layer);
        GameClassPoolMgr.Instance.Push(layerInfo);
    }

    public int ConnectLayerInput(PlayableAdapter playableAdapter, EnAnimLayer layer)
    {
        if (!TryGetLayerMixerInfo(layer, out var layerInfo))
        {
            layerInfo = CreateLayerMixerInfo(layer);
            AddMixerLayer(layer);
        }
        else if (layerInfo.IsStatus(EnAnimLayerStatus.Exiting))
        {
            AddMixerLayer(layer);
        }

        int portID = -1;
        if (layerInfo.GetConnectCount() > 0)
        {
            var from = layerInfo.DisconnectNoDestroy(GlobalConfig.Int0);
            var mainAdapter2 = from.GetMainPlayableAdapter();
            m_Adapter2PortID.Remove(mainAdapter2);
            from.Stop();
            var mixerAdapter = PlayableMixerAdapter.Create(m_EntityID, m_GraphAdapter, layer, from, playableAdapter, GlobalConfig.Int1);
            mixerAdapter.SetCompleteAction(MixerComplete);
            portID = layerInfo.Connect(mixerAdapter);
        }
        else
        {
            portID = layerInfo.Connect(playableAdapter);
        }
        var mainAdapter = playableAdapter.GetMainPlayableAdapter();
        m_Adapter2PortID.Add(mainAdapter, portID);
        return portID;
    }
    private void MixerComplete(EnAnimLayer layer, PlayableMixerAdapter mixerAdapter, PlayableAdapter from, PlayableAdapter to)
    {
        var mainAdapter = mixerAdapter.GetMainPlayableAdapter();
        if (!m_Adapter2PortID.TryGetValue(mainAdapter, out var portID))
            return;
        if (!TryGetLayerMixerInfo(layer, out var layerInfo))
            return;
        mixerAdapter.DisconnectAll();
        PlayableAdapter.Destroy(from);
        layerInfo.Disconnect(portID);
        layerInfo.Connect(to);
    }
    public void DisconnectLayerInput(EnAnimLayer layer, PlayableAdapter adapter)
    {
        if (!m_Adapter2PortID.TryGetValue(adapter, out var port))
            return;
        var mainAdapter = adapter.GetMainPlayableAdapter();
        m_Adapter2PortID.Remove(mainAdapter);
        if (!TryGetLayerMixerInfo(layer, out var layerInfo))
            return;
        if (!layerInfo.ContainsPortID(port))
            return;
        var playableAdapter = layerInfo.GetAdapter(port);
        playableAdapter.Stop();
        if (layerInfo.GetConnectCount() == GlobalConfig.Int1)
        {
            RemoveMixerLayer(layer);
            return;
        }
        layerInfo.Disconnect(port);
    }
    private void AddMixerLayer(EnAnimLayer layer)
    {
        if (!TryGetLayerMixerInfo(layer, out var info))
            return;
        m_EnterLayerList.Add(layer);
        if (info.IsStatus(EnAnimLayerStatus.Exiting))
        {
            m_ExistLayerList.Remove(layer);
        }
        info.SetStatus(EnAnimLayerStatus.Entering);
    }
    private void RemoveMixerLayer(EnAnimLayer layer)
    {
        if (!TryGetLayerMixerInfo(layer, out var info))
            return;
        m_ExistLayerList.Add(layer);
        if (info.IsStatus(EnAnimLayerStatus.Entering))
        {
            m_EnterLayerList.Remove(layer);
        }
        info.SetStatus(EnAnimLayerStatus.Exiting);
    }
    public void SetLayerWeight(EnAnimLayer layer, float weight)
    {
        m_LayerMixer.SetInputWeight((int)layer, weight);
    }
    public float GetLayerWeight(EnAnimLayer layer)
    {
        var weight = m_LayerMixer.GetInputWeight((int)layer);
        return weight;
    }
    private bool TryGetLayerMixerInfo(EnAnimLayer layer, out LayerMixerInfo info)
    {
        if (!m_Layer2unusePortDic.TryGetValue(layer, out info))
            return false;
        return true;
    }
    private void SetLayerStatus(EnAnimLayer layer, EnAnimLayerStatus status)
    {
        if (!TryGetLayerMixerInfo(layer, out var info))
            return;
        info.SetStatus(status);
    }
    public int GetConnectCount(EnAnimLayer layer)
    {
        if (!TryGetLayerMixerInfo(layer, out var info))
            return -1;
        var count = info.GetConnectCount();
        return count;
    }

    public void Update()
    {
        for (int i = 0; i < m_EnterLayerList.Count; i++)
        {
            var layer = m_EnterLayerList[i];
            var curWeight = GetLayerWeight(layer);
            if (curWeight == 1)
            {
                m_EnterLayerList[i] = m_EnterLayerList[^1];
                m_EnterLayerList.RemoveAt(m_EnterLayerList.Count - 1);
                i--;
                SetLayerStatus(layer, EnAnimLayerStatus.Playing);
                continue;
            }
            var toWeight = curWeight + ABBUtil.GetTimeDelta() * 5;
            var weight = Mathf.Clamp(toWeight, 0, 1);
            SetLayerWeight(layer, weight);
        }
        for (int i = 0; i < m_ExistLayerList.Count; i++)
        {
            var layer = m_ExistLayerList[i];
            var curWeight = GetLayerWeight(layer);
            if (curWeight == 0)
            {
                m_ExistLayerList[i] = m_ExistLayerList[^1];
                m_ExistLayerList.RemoveAt(m_ExistLayerList.Count - 1);
                i--;
                SetLayerStatus(layer, EnAnimLayerStatus.Nothing);
                if (!TryGetLayerMixerInfo(layer, out var layerInfo))
                    continue;
                layerInfo.DisconnectAll();
                DestroyLayerMixerInfo(layer);
                continue;
            }
            var toWeight = curWeight - ABBUtil.GetTimeDelta() * 5;
            var weight = Mathf.Clamp(toWeight, 0, 1);
            SetLayerWeight(layer, weight);
        }
    }
}
