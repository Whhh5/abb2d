using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class SampleAnimTest : MonoBehaviour
{
    //[SerializeField]
    //private Animator m_Anim = null;
    //[SerializeField]
    //private AnimationClip m_Clip = null;
    //PlayableGraphAdapter graph2 = null;
    private void Awake()
    {
        //graph2 = PlayableGraphAdapter.Create(, m_Anim);

        //var graph = graph2.GetGraph();
        //var graph = PlayableGraph.Create($"111");
        //graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
        //var output = AnimationPlayableOutput.Create(graph, $"-output", m_Anim);
        //var layerMixer = AnimationLayerMixerPlayable.Create(graph);
        //output.SetSourcePlayable(layerMixer);
        //graph.Play();
        //var clipPlayable = AnimationClipPlayable.Create(graph, m_Clip);
        //graph2.layerMixer.AddInput(clipPlayable, 0, 1);

        //var clipAdapter = GameClassPoolMgr.Instance.Pull<PlayableClipAdapter>();
        //clipAdapter.InitClip(graph2, m_Clip);
        //graph2.Connect(EnAnimLayer.Base, clipAdapter);
    }

}
