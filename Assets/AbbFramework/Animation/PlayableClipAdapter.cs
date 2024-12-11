using UnityEngine.Playables;
using UnityEngine.Animations;
using UnityEngine.PlayerLoop;
using UnityEngine;

public class PlayableClipAdapter : PlayableAdapter
{
    public static PlayableClipAdapter Create(int entityID, PlayableGraphAdapter graph, EnLoadTarget clipTarget)
    {
        var clipAdapter = GameClassPoolMgr.Instance.Pull<PlayableClipAdapter>();
        clipAdapter.Initialization(entityID, graph);
        clipAdapter.InitClip(graph, clipTarget);
        return clipAdapter;
    }
    private AnimationClipPlayable m_ClipPlayable;
    private ScriptPlayable<AdapterPlayable> m_Playable;
    private EnLoadTarget m_ClipTarget = EnLoadTarget.None;
    public override void OnDestroy()
    {
        AnimMgr.Instance.RecycleClip(m_ClipTarget);
        base.OnDestroy();
        m_ClipTarget = EnLoadTarget.None;
        m_Playable.Destroy();
        m_ClipPlayable.Destroy();
    }

    public override ScriptPlayable<AdapterPlayable> GetPlayable()
    {
        return m_Playable;
    }
    public override void Initialization(int entityID, PlayableGraphAdapter graph)
    {
        base.Initialization(entityID, graph);
    }
    public void InitClip(PlayableGraphAdapter graph, EnLoadTarget clipTarget)
    {
        var clip = AnimMgr.Instance.GetClip(clipTarget);
        m_ClipTarget = clipTarget;
        m_ClipPlayable = AnimationClipPlayable.Create(graph.GetGraph(), clip);
        m_Playable = ScriptPlayable<AdapterPlayable>.Create(graph.GetGraph(), 0);
        m_Playable.AddInput(m_ClipPlayable, 0, 1);
    }
    public override float GetUnitTime()
    {
        return m_ClipPlayable.GetAnimationClip().length;
    }
    public override bool IsLoop()
    {
        //return base.IsLoop();
        switch (m_ClipTarget)
        {
            case EnLoadTarget.Anim_Attack_01:
            case EnLoadTarget.Anim_Attack_02:
            case EnLoadTarget.Anim_Attack_03:
            case EnLoadTarget.Anim_Attack_04:
            case EnLoadTarget.Anim_Attack_05:
            case EnLoadTarget.Anim_Attack_06:
                return false;
            default:
                return true;
        }
    }
}
