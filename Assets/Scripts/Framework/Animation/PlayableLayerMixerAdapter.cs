using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class PlayableLayerMixerAdapter : PlayableAdapter
{
    private AnimationLayerMixerPlayable m_LayerMixer;
    public override void Initialization(PlayableGraphAdapter graph)
    {
        throw new System.NotImplementedException();
    }

    public override IPlayable GetPlayable()
    {
        throw new System.NotImplementedException();
    }
}
