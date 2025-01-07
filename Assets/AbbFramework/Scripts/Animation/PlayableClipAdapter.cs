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

    public void OnPoolEnable()
    {
    }

    public void OnPoolInit(CustomPoolData userData)
    {
    }

    public void PoolConstructor()
    {
    }

    public void PoolRelease()
    {
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
    public override void OnPoolInit(CustomPoolData userData)
    {
        base.OnPoolInit(userData);
        var playableData = userData as PlayableAdapterData;
        var clipData = (PlayableClipAdapterData)playableData.customData;

        m_ClipID = clipData.clipID;
        var clip = AnimMgr.Instance.GetClip(m_ClipID);
        m_ClipLength = clip.length;
        m_ClipPlayable = AnimationClipPlayable.Create(m_Graph.GetGraph(), clip);
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
