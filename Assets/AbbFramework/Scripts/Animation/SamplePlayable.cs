using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class SamplePlayable: MonoBehaviour
{
    [SerializeField]
    private Animator m_Anim = null;
    private PlayableGraphAdapter m_Graph;
    private void OnDestroy()
    {
        PlayableGraphAdapter.OnDestroy(m_Graph);
        m_Graph = null;
    }
    private void Awake()
    {
    }
    private void Start()
    {
        
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //m_Graph = PlayableGraphAdapter.Create(m_Anim);
            //var clipAdapter = PlayableClipAdapter.Create(m_Graph, EnLoadTarget.Anim_Rest_walk);
            //m_Graph.Connect(clipAdapter);

        }
        if (Input.GetKey(KeyCode.A))
        {
            m_Graph.UpdtaeGraphEvaluate();
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log(m_Graph.IsPlaying());
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log(m_Graph.IsPlaying());
            m_Graph.PlayGraph();
            Debug.Log(m_Graph.IsPlaying());
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log(m_Graph.IsPlaying());
            m_Graph.StopGraph();
            Debug.Log(m_Graph.IsPlaying());
        }
        if(Input.GetKeyDown(KeyCode.G))
        {
            //var clipAdapter = ScriptPlayable<PlayableScriptBrhaviour>.Create(m_Graph.GetGraph());
            //m_Graph.layerMixer.AddInput(clipAdapter, 0, 1);
            //var clip = AnimMgr.Instance.GetClip(EnLoadTarget.Anim_Rest_walk);
            //var clipadapter = AnimationClipPlayable.Create(m_Graph.GetGraph(), clip);
            //clipAdapter.AddInput(clipadapter, 0, 1);
        }
    }
}


