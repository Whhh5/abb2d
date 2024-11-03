using UnityEngine.Playables;
using UnityEngine.Animations;
using UnityEngine.PlayerLoop;

public class PlayableClipAdapter : PlayableAdapter
{
    public static PlayableClipAdapter Create(PlayableGraphAdapter graph, EnLoadTarget clipTarget)
    {
        var clipAdapter = GameUtil.PullClass<PlayableClipAdapter>();
        clipAdapter.InitClip(graph, clipTarget);
        return clipAdapter;
    }
    public override EnClassType ClassType => EnClassType.PlayableClipAdapter;
    private AnimationClipPlayable m_ClipPlayable;
    private ScriptPlayable<AdapterPlayable> m_Playable;
    public override void OnDestroy()
    {
        m_ClipPlayable.Destroy();
        m_Playable.Destroy();
        AnimMgr.Instance.RecycleClip(EnLoadTarget.Pre_TestPrefab);
        base.OnDestroy();
    }

    public override ScriptPlayable<AdapterPlayable> GetPlayable()
    {
        return m_Playable;
    }
    public override void Initialization(PlayableGraphAdapter graph)
    {
        base.Initialization(graph);
    }
    public void InitClip(PlayableGraphAdapter graph, EnLoadTarget clipTarget)
    {
        Initialization(graph);
        var clip = AnimMgr.Instance.GetClip(clipTarget);
        m_ClipPlayable = AnimationClipPlayable.Create(graph.GetGraph(), clip);
        m_Playable = ScriptPlayable<AdapterPlayable>.Create(graph.GetGraph(), 0);
        m_Playable.AddInput(m_ClipPlayable, 0, 1);
    }
    public override void ConnectInputTo(PlayableAdapter playableAdapter, int portID)
    {
        throw new System.NotImplementedException();
    }

    public override void ConnectOutputTo(int portID, PlayableAdapter playableAdapter)
    {
        playableAdapter.GetPlayable().ConnectInput(0, m_Playable, 0);
    }

}
