using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class CustomPlable : PlayableBehaviour
{
    private AnimationMixerPlayable mixer;
    private int curIndex;
    private float curAnimaLength;
    private int maxIndex;
    private AnimationClip[] clips;
    private PlayableGraph graph;
    public void InitData(AnimationClip[] clips, PlayableGraph graph, Playable playable)
    {
        curIndex = 0;
        maxIndex = clips.Length;
        this.graph = graph;
        mixer = AnimationMixerPlayable.Create(graph, 1);
        this.clips = clips;
        var clipPlayable = AnimationClipPlayable.Create(graph, clips[0]);
        //clipPlayable.SetPropagateSetTime(true);
        mixer.ConnectInput(0, clipPlayable, 0, 1);
        playable.AddInput(mixer, 0, 1);
        curAnimaLength = clips[0].length;
    }
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        base.PrepareFrame(playable, info);

        if (mixer.GetInput(0).GetTime() < curAnimaLength)
            return;
        curIndex = (curIndex + 1) % maxIndex;
        var clipPlayable = AnimationClipPlayable.Create(graph, clips[curIndex]);
        mixer.DisconnectInput(0);
        mixer.ConnectInput(0, clipPlayable, 0, 1);
        curAnimaLength = clips[curIndex].length;
    }
}
public class CustomSample : MonoBehaviour
{
    public Animator animator;
    public AnimationClip[] clip;

    private PlayableGraph graph;
    // Start is called before the first frame update
    void Start()
    {
        graph = PlayableGraph.Create();
        var output = AnimationPlayableOutput.Create(graph, "_", animator);
        var custom = ScriptPlayable<CustomPlable>.Create(graph);
        custom.GetBehaviour().InitData(clip, graph, custom);
        output.SetSourcePlayable(custom);
        graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
        graph.Play();
    }
    private void OnDestroy()
    {
        graph.Destroy();
    }
    // Update is called once per frame
    void Update()
    {

    }
}
