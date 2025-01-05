using UnityEngine;
using UnityEngine.Playables;


public class TestPlayableAsset : PlayableAsset
{
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        return Playable.Create(graph);
    }
}
