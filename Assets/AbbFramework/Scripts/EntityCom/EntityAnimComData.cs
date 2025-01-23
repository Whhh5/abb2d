using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using static Unity.Burst.Intrinsics.X86.Avx;

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
    LayerMixer,
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
public class EntityAnimComData : IEntity3DComData<Entity3DComDataUserData>, IUpdate
{
    private int m_EntityID = -1;
    private Entity3DData m_Entity3D = null;
    private PlayableGraphAdapter m_PlayableGraph = null;

    private Dictionary<PlayableAdapter, LayerMixerConnectInfo> m_Adapter2ConnectInfo = new();
    private List<PlayableAdapter> m_NoLoopPlayableList = new();
    private HashSet<EnEntityCmd> m_CreateAddList = new();
    private Dictionary<EnEntityCmd, SkillTypePlayableAdapter> m_CmdAdapterDic = new();
    private Dictionary<EnEntityCmdLevel, List<EnEntityCmd>> m_Level2Cmd = new();

    #region life
    public void OnPoolDestroy()
    {
        m_Entity3D = null;
        m_EntityID = -1;
    }
    public void OnPoolInit(Entity3DComDataUserData userData)
    {
        m_EntityID = userData.entity3DData.EntityID;
        m_Entity3D = userData.entity3DData;

    }

    public void PoolConstructor()
    {
    }

    public void OnPoolEnable()
    {
    }

    public void PoolRelease()
    {
    }

    public void OnDestroyGO(int entityID)
    {
        UpdateMgr.Instance.Unregistener(this);
        m_CreateAddList.Clear();
        var listKey = new List<EnEntityCmd>(m_CmdAdapterDic.Count);
        foreach (var item in m_CmdAdapterDic) listKey.Add(item.Key);
        foreach (var item in listKey) RemoveCmd(item);
        m_CmdAdapterDic.Clear();
        m_Level2Cmd.Clear();
        PlayableGraphAdapter.OnDestroy(m_PlayableGraph);
        m_PlayableGraph = null;
        m_Adapter2ConnectInfo.Clear();
        m_NoLoopPlayableList.Clear();
    }
    public void OnCreateGO(int entityID)
    {
        var entity3DData = Entity3DMgr.Instance.GetEntity3DData(entityID);
        var entity3D = entity3DData.GetEntity<Entity3D>();
        var animCom = entity3D.GetAnimator();
        m_PlayableGraph = PlayableGraphAdapter.Create(m_EntityID, animCom);
        foreach (var item in m_CreateAddList)
            AddCmd(item);
        m_CreateAddList.Clear();
        UpdateMgr.Instance.Registener(this);
    }

    #endregion


    private SkillTypePlayableAdapter GetCmdPlayableAdapter(EnEntityCmd cmd)
    {
        var cmdAdapter = CmdMgr.Instance.GetPlayable(m_PlayableGraph, cmd);
        cmdAdapter.SetEntityCmd(cmd);
        return cmdAdapter;
    }
    #region cmd
    public void AddCmd(EnEntityCmd cmd)
    {
        if (!m_CmdAdapterDic.TryGetValue(cmd, out var cmdAdapter))
        {
            if (!IsAddCmd(cmd))
                return;
            if (!m_Entity3D.IsLoadSuccess)
            {
                m_CreateAddList.Add(cmd);
                return;
            }
            cmdAdapter = GetCmdPlayableAdapter(cmd);
            AddCmdData(cmdAdapter);
            ConnectLayerInput(cmd, cmdAdapter, cmdAdapter.GetOutputLayer());
            cmdAdapter.ExecuteCmd();
        }
        else
        {
            cmdAdapter.ReExecuteCmd();
        }

    }
    
    public void RemoveCmd(EnEntityCmd cmd)
    {
        var adapter = RemoveCmdData(cmd);
        if (adapter == null)
            return;
        DisconnectLayerInput(adapter);
    }
    public void CancelCmd(EnEntityCmd cmd)
    {
        if (!m_CmdAdapterDic.TryGetValue(cmd, out var cmdAdapter))
            return;
        cmdAdapter.CancelCmd();
    }
    public bool IsAddCmd(EnEntityCmd cmd)
    {
        var level = AnimMgr.Instance.GetCmdLevel(cmd);
        foreach (var item in m_CmdAdapterDic)
        {
            var level2 = AnimMgr.Instance.GetCmdLevel(item.Key);
            if (level2 >= level)
                if (!item.Value.NextAnimLevelComdition())
                    return false;
        }
        return true;
    }
    private void AddCmdData(SkillTypePlayableAdapter cmdAdapter)
    {
        var cmd = cmdAdapter.GetEntityCmd();
        m_CmdAdapterDic.Add(cmd, cmdAdapter);
    }
    private PlayableAdapter RemoveCmdData(EnEntityCmd cmd)
    {
        if (!m_CmdAdapterDic.TryGetValue(cmd, out var adapter))
            return null;
        m_CmdAdapterDic.Remove(cmd);
        adapter.RemoveCmd();
        return adapter;
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
            DisconnectLayerInput(item);
        }
    }
}
