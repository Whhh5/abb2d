using System;
using UnityEngine;
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
            m_Graph = PlayableGraphAdapter.Create(m_Anim);
            var clipAdapter = PlayableClipAdapter.Create(m_Graph, EnLoadTarget.Anim_Rest_idle);
            m_Graph.Connect(clipAdapter);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            
        }
    }
}


