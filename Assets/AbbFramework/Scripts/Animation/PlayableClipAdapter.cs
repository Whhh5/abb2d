using UnityEngine.Playables;
using UnityEngine.Animations;
using UnityEngine.PlayerLoop;
using UnityEngine;

public class PlayableClipAdapterData : IPlayableAdapterCustomData
{
    public int clipID = -1;
    public void OnPoolRecycle()
    {
        clipID = -1;
    }
}
public class PlayableClipAdapter : PlayableAdapter
{
    private AnimationClipPlayable m_ClipPlayable;
    private int m_ClipID = -1;
    private float m_ClipLength = -1;
    protected override void OnDestroy()
    {
        m_ClipPlayable.Destroy();
        AnimMgr.Instance.RecycleClip(m_ClipID);
        base.OnDestroy();
        m_ClipID = -1;
        m_ClipLength = -1;
    }
    protected override void Initialization(PlayableGraphAdapter graph, IPlayableAdapterCustomData customData)
    {
        base.Initialization(graph, customData);
        var clipData = (PlayableClipAdapterData)customData;

        m_ClipID = clipData.clipID;
        var clip = AnimMgr.Instance.GetClip(m_ClipID);
        m_ClipLength = clip.length;
        m_ClipPlayable = AnimationClipPlayable.Create(graph.GetGraph(), clip);
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
}
