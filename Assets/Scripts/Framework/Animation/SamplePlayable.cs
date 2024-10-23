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
        m_Graph.Initialization(m_Anim);
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

        }
        if (Input.GetKeyDown(KeyCode.W))
        {

        }
    }
}


