using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public abstract class PlayableAdapter
{
    public abstract void Initialization(PlayableGraphAdapter graph);
    public abstract IPlayable GetPlayable();
}