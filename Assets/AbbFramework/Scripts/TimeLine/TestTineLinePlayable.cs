using UnityEngine;
using UnityEngine.Playables;

public class TestTineLinePlayable : PlayableBehaviour
{
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        //Debug.Log("PrepareFrame");
    }
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        //Debug.Log("ProcessFrame");
    }
}
