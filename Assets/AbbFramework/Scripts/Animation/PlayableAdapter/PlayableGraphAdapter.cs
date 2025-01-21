using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public enum EnAnimLayer
{
    None = -1,
    Base = 0,
    Layer1,
    Layer2,
    Layer3,
    EnumCount,
}
public class PlayableGraphAdapter : IClassPool<PlayableGraphAdapterUserData>, IUpdate
{

    public static PlayableGraphAdapter Create(int entityID, Animator animator)
    {
        var data = ClassPoolMgr.Instance.Pull<PlayableGraphAdapterUserData>();
        data.entityID = entityID;
        data.anim = animator;
        var playable = ClassPoolMgr.Instance.Pull<PlayableGraphAdapter>(data);
        ClassPoolMgr.Instance.Push(data);
        return playable;
    }
    public static void OnDestroy(PlayableGraphAdapter graph)
    {
        ClassPoolMgr.Instance.Push(graph);
    }
    public static implicit operator int(PlayableGraphAdapter adapter)
    {
        return adapter.GetEntityID();
    }

    private PlayableGraph m_Graph = default;
    private int m_EntityID = -1;
    private AnimationLayerMixerPlayable m_LayerMixerPlayable;
    private Dictionary<EnAnimLayer, LayerMixerInfo> m_Layer2unusePortDic = new();
    private List<EnAnimLayer> m_EnterLayerList = new();
    private List<EnAnimLayer> m_ExistLayerList = new();
    private AnimationScriptPlayable m_PlayableJob;


    public void PoolConstructor()
    {
    }

    public void OnPoolInit(PlayableGraphAdapterUserData userData)
    {
        m_EntityID = userData.entityID;
        m_Graph = PlayableGraph.Create($"custom-{userData.anim.name}");
        m_Graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
        var output = AnimationPlayableOutput.Create(m_Graph, $"{userData.anim.name}-output", userData.anim);
        var jobData = new PlayableGraphAnimJob();
        m_PlayableJob = AnimationScriptPlayable.Create<PlayableGraphAnimJob>(m_Graph, jobData);
        m_LayerMixerPlayable = AnimationLayerMixerPlayable.Create(m_Graph, (int)EnAnimLayer.EnumCount);
        m_PlayableJob.AddInput(m_LayerMixerPlayable, 0, 1);
        output.SetSourcePlayable(m_PlayableJob);
        UpdateMgr.Instance.Registener(this);
    }

    public void OnPoolEnable()
    {
    }

    public void OnPoolDestroy()
    {
        UpdateMgr.Instance.Unregistener(this);
        m_LayerMixerPlayable.Destroy();
        m_Graph.Destroy();
        m_EntityID = -1;
        m_EnterLayerList.Clear();
        m_ExistLayerList.Clear();
        m_Layer2unusePortDic.Clear();
    }

    public void PoolRelease()
    {
    }
    private float GetLayerWeight(EnAnimLayer layer)
    {
        return m_LayerMixerPlayable.GetInputWeight((int)layer);
    }
    private void SetLayerWeight(EnAnimLayer layer, float weight)
    {
        m_LayerMixerPlayable.SetInputWeight((int)layer, weight);
    }
    private void SetLayerAdditive(EnAnimLayer layer, bool isAdditive)
    {
        m_LayerMixerPlayable.SetLayerAdditive((uint)layer, isAdditive);
    }
    public PlayableGraph GetGraph()
    {
        return m_Graph;
    }
    public int GetEntityID()
    {
        return m_EntityID;
    }
    public void UpdtaeGraphEvaluate()
    {
        m_Graph.Evaluate(ABBUtil.GetTimeDelta());
    }
    public bool IsPlaying()
    {
        return m_Graph.IsPlaying();
    }
    public void PlayGraph()
    {
        m_Graph.Play();
    }
    public void StopGraph()
    {
        m_Graph.Stop();
    }
    public void PauseGraph()
    {

    }

    #region layermixerinfo
    public bool TryGetLayerMixerInfo(EnAnimLayer layer, out LayerMixerInfo info)
    {
        if (!m_Layer2unusePortDic.TryGetValue(layer, out info))
            return false;
        return true;
    }
    #endregion

    #region layermixer

    public void AddMixerLayer(EnAnimLayer layer)
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
    public void RemoveMixerLayer(EnAnimLayer layer)
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

    private void DestroyLayerMixerInfo(EnAnimLayer layer)
    {
        if (!TryGetLayerMixerInfo(layer, out var layerInfo))
            return;
        m_LayerMixerPlayable.DisconnectInput((int)layer);
        m_Layer2unusePortDic.Remove(layer);
        ClassPoolMgr.Instance.Push(layerInfo);
    }

    public LayerMixerInfo CreateLayerMixerInfo(EnAnimLayer layer)
    {
        var layerAdapter = ScriptPlayable<BridgePlayableAdapter>.Create(m_Graph);
        var infoUserData = ClassPoolMgr.Instance.Pull<LayerMixerInfoUserData>();
        infoUserData.layer = layer;
        infoUserData.layerAdapter = layerAdapter;
        var info = ClassPoolMgr.Instance.Pull<LayerMixerInfo>(infoUserData);
        ClassPoolMgr.Instance.Push(infoUserData);
        m_Layer2unusePortDic.Add(layer, info);
        m_LayerMixerPlayable.ConnectInput((int)layer, layerAdapter, GlobalConfig.Int0, GlobalConfig.Int0);
        var avatar = AnimMgr.Instance.GetLayerAvatar(layer);
        m_LayerMixerPlayable.SetLayerMaskFromAvatarMask((uint)layer, avatar);
        SetLayerAdditive(layer, false);
        return info;
    }
    public void DisconnectLayerInput(EnAnimLayer layer, int port)
    {
        if (!TryGetLayerMixerInfo(layer, out var layerInfo))
            return;
        if (!layerInfo.ContainsPortID(port))
            return;
        var playableAdapter = layerInfo.GetAdapter(port);
        playableAdapter.Complete();
        if (layerInfo.GetConnectCount() == GlobalConfig.Int1)
        {
            RemoveMixerLayer(layer);
            return;
        }
        layerInfo.Disconnect(port);
    }
    #endregion

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
    private RaycastHit[] m_ArrHit = new RaycastHit[1];
    public void Update()
    {
        UpdateFootIK();
        UpdtaeGraphEvaluate();
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
    public void SetApplyRootMotion(bool applyRootMotion)
    {
        var job = m_PlayableJob.GetJobData<PlayableGraphAnimJob>();
        job.applyRootMotion = applyRootMotion;
        m_PlayableJob.SetJobData(job);
    }


    private void UpdateLeftFootIK(ref PlayableGraphAnimJob job)
    {
        var leftPos = job.lastLeftFootPos;
        var startPos = leftPos + Vector3.up * 1;
        var dis = 1 + 0.5f;
        var count = Physics.RaycastNonAlloc(startPos, Vector3.down, m_ArrHit, dis);
        Debug.DrawLine(startPos, startPos + Vector3.down * dis, Color.red);
        if (count > 0)
        {
            var hit = m_ArrHit[0];
            var pos = hit.point;
            var ikDis = Vector3.Distance(pos, leftPos);

            //DebugDrawMgr.Instance.DrawSphere(pos, 0.1f, 0.01f);

            var weight2 = pos.y < leftPos.y
                ? Mathf.Lerp(0, 1, 1 - Mathf.Pow(Mathf.Clamp01((ikDis - 0.1f) / 0.4f), 5))
                : 1;
            job.leftFeatWorldPos = pos;
            job.leftFootIKWeight = weight2;
        }
        else
        {
            job.leftFootIKWeight = 0;
        }
    }
    private void UpdateRightFootIK(ref PlayableGraphAnimJob job)
    {
        var rightPos = job.lastRightFootPos;
        var startPos = rightPos + Vector3.up * 1;
        var dis = 1 + 0.5f;
        var count = Physics.RaycastNonAlloc(startPos, Vector3.down, m_ArrHit, dis);
        Debug.DrawLine(startPos, startPos + Vector3.down * dis, Color.red);
        if (count > 0)
        {
            var hit = m_ArrHit[0];
            var pos = hit.point;
            var ikDis = Vector3.Distance(pos, rightPos);

            //Debug.DrawRay(pos, dir, Color.gray, 1);
            //DebugDrawMgr.Instance.DrawSphere(pos, 0.1f, 0.01f);

            var weight2 = pos.y < rightPos.y
                ? Mathf.Lerp(0, 1, 1 - Mathf.Pow(Mathf.Clamp01((ikDis - 0.1f) / 0.4f), 5))
                : 1;
            job.rightFeatWorldPos = pos;
            job.rightFootIKWeight = weight2;
        }
        else
        {
            job.rightFootIKWeight = 0;
        }

    }
    private void UpdateFootIK()
    {
        var job = m_PlayableJob.GetJobData<PlayableGraphAnimJob>();

        UpdateLeftFootIK(ref job);
        UpdateRightFootIK(ref job);

        m_PlayableJob.SetJobData(job);
    }


    public PlayableClipAdapter CreateClipPlayableAdapter(int clipID)
    {
        var clipData = ClassPoolMgr.Instance.Pull<PlayableClipAdapterData>();
        clipData.clipID = clipID;
        var clipPlayable = Create<PlayableClipAdapter>(clipData);
        ClassPoolMgr.Instance.Push(clipData);
        return clipPlayable;
    }
    public void DestroyPlayableAdapter(PlayableAdapter playableAdapter)
    {
        PlayableAdapter.Destroy(playableAdapter);
    }
    public PlayableMixerAdapter CreateMixerPlayableAdapter(PlayableAdapter from, PlayableAdapter to, float time, PlayableMixerCompleteAction complete)
    {
        var clipData = ClassPoolMgr.Instance.Pull<PlayableMixerAdapterData>();
        clipData.from = from;
        clipData.to = to;
        clipData.time = time;
        clipData.complete = complete;
        var clipPlayable = Create<PlayableMixerAdapter>(clipData);
        ClassPoolMgr.Instance.Push(clipData);
        return clipPlayable;
    }
    public T Create<T>()
        where T : PlayableAdapter, new()
    {
        var playable = PlayableAdapter.Create<T>(this);
        return playable;
    }
    public T Create<T>(IPlayableAdapterCustomData customData)
        where T : PlayableAdapter, new()
    {
        var playable = PlayableAdapter.Create<T>(this, customData);
        return playable;
    }

}
