using System;
using System.Collections.Generic;
using UnityEngine;

public enum EnEntityCmd
{
    None,
    Idle,
    Run,
    Jump,
    JumpDown,
    Attack,
    Skill1,
    Skill2,
    Skill3,
    Teleport,
    Injured, // 受伤

    Monster0Idle,
    Monster0Run,
    Monster0Skill1,
    Monster0Skill2,
    Monster0Die,
    Monster0Buff1,

    PlayerBuff,
    PlayerWalk,
    PlayerDown,//战士大地技能,
    PastorWaterBuff,
    PastorEcliBuff,
    PastorAttackBuff,
}
public enum EnEntityCmdLevel
{
    None,
    Level0,
    Level1,
    Level2,
    Level3,
    Level4,
    Level5,
    Level6,
    Level100,
    MaxLevel,
}
public interface IEntityAnimCom : IEntity3DCom
{
    public Animator GetAnimator();
}
public sealed class EntityAnimComData : Entity3DComDataGO<IEntityAnimCom>, IUpdate
{
    private PlayableGraphAdapter m_PlayableGraph = null;

    private Dictionary<PlayableAdapter, LayerMixerConnectInfo> m_Adapter2ConnectInfo = new();
    private List<PlayableAdapter> m_NoLoopPlayableList = new();
    private HashSet<EnEntityCmd> m_CreateAddList = new();
    private List<EnEntityCmd> _CurCmdList = new();
    private Dictionary<EnEntityCmd, List<SkillTypePlayableAdapter>> m_CmdAdapterDic = new();
    private Dictionary<EnEntityCmdLevel, List<EnEntityCmd>> m_Level2Cmd = new();

    #region life
    public override void OnEnable()
    {
        base.OnEnable();
        UpdateMgr.Instance.Registener(this);
    }
    public override void OnDisable()
    {
        UpdateMgr.Instance.Unregistener(this);
        base.OnDisable();
    }
    public override void OnDestroyGO()
    {
        base.OnDestroyGO();
        m_CreateAddList.Clear();
        var listKey = new List<EnEntityCmd>(m_CmdAdapterDic.Count);
        foreach (var item in m_CmdAdapterDic) listKey.Add(item.Key);
        foreach (var item in listKey) RemoveCmd(item);
        m_CmdAdapterDic.Clear();
        _CurCmdList.Clear();
        m_Level2Cmd.Clear();
        PlayableGraphAdapter.OnDestroy(m_PlayableGraph);
        m_PlayableGraph = null;
        m_Adapter2ConnectInfo.Clear();
        m_NoLoopPlayableList.Clear();
    }
    public override void OnCreateGO()
    {
        base.OnCreateGO();
        var animCom = _GoCom.GetAnimator();
        m_PlayableGraph = PlayableGraphAdapter.Create(_EntityID, animCom);
        foreach (var item in m_CreateAddList)
            AddCmd(item);
        m_CreateAddList.Clear();
    }

    #endregion


    private SkillTypePlayableAdapter GetCmdPlayableAdapter(EnEntityCmd cmd)
    {
        var cmdAdapter = CmdMgr.Instance.GetPlayable(m_PlayableGraph, cmd);
        return cmdAdapter;
    }
    #region cmd
    public void AddCmd(EnEntityCmd cmd)
    {
        if (!m_CmdAdapterDic.TryGetValue(cmd, out var cmdAdapters))
        {
            if (!IsAddCmd(cmd))
                return;
            if (!Entity3DMgr.Instance.GetEntityIsLoadSuccess(_EntityID))
            {
                m_CreateAddList.Add(cmd);
                return;
            }
            var cmdAdapter = GetCmdPlayableAdapter(cmd);
            AddCmdData(cmdAdapter);
            ConnectLayerInput(cmd, cmdAdapter, cmdAdapter.GetOutputLayer());
            cmdAdapter.ExecuteCmd();

            var cmdCfg = GameSchedule.Instance.GetCmdCfg0((int)cmd);
            var applyRootMotion = cmdCfg.bApplyRootMotion;
            SetApplyRootMotion(applyRootMotion > 0);
        }
        else
        {
            foreach (var cmdAdapter in cmdAdapters)
                cmdAdapter.ReExecuteCmd();
        }

    }
    public EnEntityCmd GetCurCmd()
    {
        return _CurCmdList[^1];
    }
    public bool ContainsCmd(EnEntityCmd cmd)
    {
        var contains = m_CmdAdapterDic.ContainsKey(cmd);
        return contains;
    }
    public bool CmdIsEnd(EnEntityCmd cmd)
    {
        if (!m_CmdAdapterDic.TryGetValue(cmd, out var cmdAdapterList))
            return true;
        foreach (var adapter in cmdAdapterList)
        {
            //adapter.NextAnimLevelComdition
            if (!adapter.IsPlayEnd())
                return false;
        }
        return true;
    }
    public void RemoveCmd(EnEntityCmd cmd)
    {
        var adapters = RemoveCmdData(cmd);
        if (adapters == null)
            return;
        foreach (var adapter in adapters)
            DisconnectLayerInput(adapter);
    }
    public void CancelCmd(EnEntityCmd cmd)
    {
        if (!m_CmdAdapterDic.TryGetValue(cmd, out var cmdAdapter))
            return;
        foreach (var item in cmdAdapter)
            item.CancelCmd();
    }
    public bool IsAddCmd(EnEntityCmd cmd)
    {
        var level = AnimMgr.Instance.GetCmdLevel(cmd);
        var cmdCfg = GameSchedule.Instance.GetCmdCfg0((int)cmd);
        foreach (var item in m_CmdAdapterDic)
        {
            var itemCmd = item.Key;
            var level2 = AnimMgr.Instance.GetCmdLevel(itemCmd);
            if (level2 >= level)
            {
                foreach (var cmdAdapter in item.Value)
                {
                    if (!cmdAdapter.NextAnimLevelComdition())
                    {
                        if (cmdCfg.bIdleLayerPlay <= 0)
                            return false;

                        var cmdCfg2 = GameSchedule.Instance.GetCmdCfg0((int)itemCmd);
                        if (cmdCfg2.arrLayer != null)
                        {
                            for (int i = 0; i < cmdCfg2.arrLayer.Length; i++)
                            {
                                var itemLayer = cmdCfg2.arrLayer[i];
                                var value = Array.FindIndex(cmdCfg.arrLayer, item => item == itemLayer);
                                if (value >= 0)
                                    return false;
                            }
                        }
                    }
                }
            }
        }
        return true;
    }
    private void AddCmdData(SkillTypePlayableAdapter cmdAdapter)
    {
        var cmd = cmdAdapter.GetEntityCmd();
        var layer = cmdAdapter.GetOutputLayer();
        if (!m_CmdAdapterDic.TryGetValue(cmd, out var layerList))
        {
            layerList = new();
            m_CmdAdapterDic.Add(cmd, layerList);
        }
        layerList.Add(cmdAdapter);
        _CurCmdList.Add(cmd);
    }
    private List<SkillTypePlayableAdapter> RemoveCmdData(EnEntityCmd cmd)
    {
        if (!m_CmdAdapterDic.TryGetValue(cmd, out var adapters))
            return null;
        m_CmdAdapterDic.Remove(cmd);
        var index = _CurCmdList.IndexOf(cmd);
        foreach (var adapter in adapters)
            adapter.RemoveCmd();

        if (index == _CurCmdList.Count - 1)
        {
            if (_CurCmdList.Count <= 1)
            {
                SetApplyRootMotion(false);
            }
            else
            {
                var nextCmd = _CurCmdList[^2];
                var cmdCfg = GameSchedule.Instance.GetCmdCfg0((int)nextCmd);
                SetApplyRootMotion(cmdCfg.bApplyRootMotion > 0);
            }
        }
        _CurCmdList.RemoveAt(index);

        return adapters;
    }
    #endregion

    public int ConnectLayerInput(EnEntityCmd cmd, SkillTypePlayableAdapter cmdAdapter, EnAnimLayer layer)
    {
        if (!m_PlayableGraph.TryGetLayerMixerInfo(layer, out var layerInfo))
        {
            layerInfo = m_PlayableGraph.CreateLayerMixerInfo(layer);
            m_PlayableGraph.AddMixerLayer(layer);
        }
        else if (layerInfo.IsStatus(EnAnimLayerStatus.Exiting))
        {
            m_PlayableGraph.AddMixerLayer(layer);
        }

        int portID = -1;
        if (layerInfo.GetConnectCount() > 0)
        {
            var from = layerInfo.DisconnectNoDestroy(GlobalConfig.Int0);
            var mainAdapter2 = from.GetMainPlayableAdapter();
            RemoveAdapter(mainAdapter2);
            from.Complete();
            
            //if (m_CmdAdapterDic.TryGetValue(from.GetEntityCmd(), out var lastAdapters))
            //{
            //    for (int i = 0; i < lastAdapters.Count; i++)
            //    {
            //        DisconnectLayerInput(lastAdapters[i]);
            //    }
            //}

            var mixerAdapter = m_PlayableGraph.CreateMixerPlayableAdapter(from, cmdAdapter, GlobalConfig.Float02, MixerComplete);
            portID = layerInfo.Connect(mixerAdapter);

        }
        else
        {
            portID = layerInfo.Connect(cmdAdapter);
        }
        var mainAdapter = cmdAdapter.GetMainPlayableAdapter();
        AddAdapter(mainAdapter, layer, portID, cmd);
        return portID;
    }
    private void MixerComplete(PlayableMixerAdapter mixerAdapter, PlayableAdapter from, PlayableAdapter to)
    {
        var mainAdapter = mixerAdapter.GetMainPlayableAdapter();
        if (!m_Adapter2ConnectInfo.TryGetValue(mainAdapter, out var connectInfo))
            return;
        if (!m_PlayableGraph.TryGetLayerMixerInfo(connectInfo.layer, out var layerInfo))
            return;
        mixerAdapter.DisconnectAll();
        layerInfo.Disconnect(connectInfo.port);
        PlayableAdapter.Destroy(from);
        layerInfo.Connect(to);
    }
    #region adapter
    private void AddAdapter(PlayableAdapter adapter, EnAnimLayer layer, int portID, EnEntityCmd cmd)
    {
        var connectInfo = ClassPoolMgr.Instance.Pull<LayerMixerConnectInfo>();
        connectInfo.layer = layer;
        connectInfo.port = portID;
        connectInfo.cmd = cmd;
        m_Adapter2ConnectInfo.Add(adapter, connectInfo);
        if (!adapter.IsLoop())
            m_NoLoopPlayableList.Add(adapter);
    }
    private void RemoveAdapter(PlayableAdapter adapter)
    {
        if (!m_Adapter2ConnectInfo.TryGetValue(adapter, out var connectInfo2))
            return;
        m_Adapter2ConnectInfo.Remove(adapter);
        m_NoLoopPlayableList.Remove(adapter);
        RemoveCmdData(connectInfo2.cmd);
        ClassPoolMgr.Instance.Push(connectInfo2);
    }

    public bool ContainsPlayableAdapter(PlayableAdapter adapter)
    {
        if (!m_Adapter2ConnectInfo.ContainsKey(adapter))
            return false;
        return true;
    }
    #endregion



    public void DisconnectLayerInput(PlayableAdapter adapter)
    {
        if (!m_Adapter2ConnectInfo.TryGetValue(adapter, out var connectInfo))
            return;
        var layer = connectInfo.layer;
        var port = connectInfo.port;
        var mainAdapter = adapter.GetMainPlayableAdapter();
        RemoveAdapter(mainAdapter);
        m_PlayableGraph.DisconnectLayerInput(layer, port);
    }

    public void SetApplyRootMotion(bool applyRootMotion)
    {
        m_PlayableGraph.SetApplyRootMotion(applyRootMotion);
    }


    public void Update()
    {
        for (int i = 0; i < m_NoLoopPlayableList.Count; i++)
        {
            var item = m_NoLoopPlayableList[i];
            //var time = item.GetUnitTime();
            //var playTime = item.GetPlayTime();
            if (!item.IsPlayEnd())
                continue;
            i--;

            //Debug.LogError(item.GetEntityCmd());
            
            DisconnectLayerInput(item);
        }
    }
}
