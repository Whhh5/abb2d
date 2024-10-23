using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

[RequireComponent(typeof(Animator))]
public unsafe class PlayAnimationSample : MonoBehaviour
{
    [SerializeField]
    private AnimationClip m_Clip = null;
    [SerializeField]
    private AnimationClip m_Clip2 = null;
    [SerializeField]
    private AnimationClip m_Clip3 = null;
    [SerializeField]
    private AnimationClip m_Clip4 = null;
    [SerializeField]
    private AnimationClip m_Clip5 = null;
    [SerializeField]
    private Animator m_Animator = null;
    [SerializeField]
    private float m_ModeTime = 0;

    private PlayableGraph m_PlayableGraph = default;
    [Range(0, 1)]
    public float m_Weight = 0.5f;
    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_PlayableGraph = PlayableGraph.Create();
        m_PlayableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
    }

    private void OnDestroy()
    {
        m_PlayableGraph.Destroy();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            CreateGraph(m_Clip);
        }
        if (Input.GetKey(KeyCode.S))
        {
            PlayMixtureAnima(m_Clip2, m_Clip3);
        }
        if (Input.GetKey(KeyCode.D))
        {
            CreateGraph(m_Clip3);
        }
        if (Input.GetKey(KeyCode.F))
        {
            CreateGraph(m_Clip4);
        }
        if (Input.GetKey(KeyCode.G))
        {
            CreateGraph(m_Clip5);
        }


        m_MixerPlayable.SetInputWeight(0, m_Weight);
        m_MixerPlayable.SetInputWeight(1, 1 - m_Weight);
    }

    private AnimationClipPlayable m_LastClipPlayable = default;
    private AnimationMixerPlayable m_MixerPlayable = default;
    private void CreateGraph(AnimationClip f_AnimaClip)
    {
        var output = AnimationPlayableOutput.Create(m_PlayableGraph, "anim--", m_Animator);
        m_LastClipPlayable = AnimationClipPlayable.Create(m_PlayableGraph, f_AnimaClip);
        output.SetSourcePlayable(m_LastClipPlayable);
        m_PlayableGraph.Play();
    }
    private void PlayMixtureAnima(AnimationClip f_AnimaClip, AnimationClip f_Anima2)
    {
        m_PlayableGraph.Destroy();
        m_PlayableGraph = PlayableGraph.Create();

        m_MixerPlayable = AnimationMixerPlayable.Create(m_PlayableGraph, 2);
        var clipPlayable = AnimationClipPlayable.Create(m_PlayableGraph, f_AnimaClip);
        var clipPlayable2 = AnimationClipPlayable.Create(m_PlayableGraph, f_Anima2);
        var output = AnimationPlayableOutput.Create(m_PlayableGraph, "anim--", m_Animator);

        m_PlayableGraph.Connect(clipPlayable2, 0, m_MixerPlayable, 0);
        m_PlayableGraph.Connect(clipPlayable, 0, m_MixerPlayable, 1);
        m_MixerPlayable.SetInputWeight(0, m_Weight);
        m_MixerPlayable.SetInputWeight(1, 1 - m_Weight);

        output.SetSourcePlayable(m_MixerPlayable);

        m_PlayableGraph.Play();
    }
}
