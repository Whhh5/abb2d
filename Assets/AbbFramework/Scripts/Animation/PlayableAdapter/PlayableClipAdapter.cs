using UnityEngine.Playables;
using UnityEngine.Animations;
using UnityEngine.PlayerLoop;
using UnityEngine;

public class PlayableClipAdapterData : IPlayableAdapterCustomData
{
    public int clipID = -1;
    public void OnPoolDestroy()
    {
        clipID = -1;
    }
}
public class PlayableClipAdapter : PlayableAdapter
{
    private AnimationClipPlayable m_ClipPlayable;
    private int m_ClipID = -1;
    private float m_ClipLength = -1;
    private EnAnimLayer _AnimLayer = EnAnimLayer.None;
    protected override void OnDestroy()
    {
        m_ClipPlayable.Destroy();
        AnimMgr.Instance.RecycleClip(m_ClipID);
        base.OnDestroy();
        m_ClipID = -1;
        m_ClipLength = -1;
    }
    public override void OnPoolInit(PlayableAdapterUserData userData)
    {
        base.OnPoolInit(userData);
        if (userData is not PlayableAdapterUserData playableData)
            return;
        var clipData = (PlayableClipAdapterData)playableData.customData;

        m_ClipID = clipData.clipID;
        var clipCfg = GameSchedule.Instance.GetClipCfg0(m_ClipID);
        _AnimLayer = (EnAnimLayer)clipCfg.nLayer;
        var clip = AnimMgr.Instance.GetClip(m_ClipID);
        m_ClipLength = clip.length;
        m_ClipPlayable = AnimationClipPlayable.Create(m_Graph.GetGraph(), clip);
        m_ClipPlayable.SetApplyFootIK(false);
        m_ClipPlayable.SetApplyPlayableIK(false);
        AddConnectRootAdapter(m_ClipPlayable, GlobalConfig.Int0, GlobalConfig.Float1);
    }
    public override float GetPlayTime()
    {
        return (float)m_ClipPlayable.GetTime();
    }
    public override float GetUnitTime()
    {
        return m_ClipLength;
    }

    public override EnAnimLayer GetOutputLayer()
    {
        return _AnimLayer;
    }
}
