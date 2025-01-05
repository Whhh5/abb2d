using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


[TrackColor(1,0,0)]
[TrackClipType(typeof(TestPlayableAsset))]
[TrackBindingType(typeof(MonoBehaviour))]
public class TestTimeLineAsset : TrackAsset
{
    public string m_TestStr;

    public override bool CanCreateTrackMixer()
    {
        return base.CanCreateTrackMixer();
    }

    protected override Playable CreatePlayable(PlayableGraph graph, GameObject gameObject, TimelineClip clip)
    {
        var playable = ScriptPlayable<TestTineLinePlayable>.Create(graph);
        return playable;
    }

}
