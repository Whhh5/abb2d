using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using Unity.Collections;

public struct AnimJob : IAnimationJob
{
    public NativeArray<TransformStreamHandle> handles;
    public float weight;
    public void ProcessAnimation(AnimationStream stream)
    {
        var hum = stream.AsHuman();
        var stream1 = stream.GetInputStream(0);
        var stream2 = stream.GetInputStream(1);

        foreach (var handle in handles)
        {
            var pos1 = handle.GetLocalPosition(stream1);
            var pos2 = handle.GetLocalPosition(stream2);
            var pos = Vector3.Lerp(pos1, pos2, weight);
            handle.SetLocalPosition(stream, pos);

            var angle1 = handle.GetLocalRotation(stream1);
            var angle2 = handle.GetLocalRotation(stream2);
            var angle = Quaternion.Slerp(angle1, angle2, weight);
            handle.SetLocalRotation(stream, angle);
        }

        //hum.SolveIK();
    }

    public void ProcessRootMotion(AnimationStream stream)
    {
        var stream1 = stream.GetInputStream(0);
        var stream2 = stream.GetInputStream(1);

        stream.velocity = Vector3.Lerp(stream1.rootMotionPosition, stream2.rootMotionPosition, weight);
        stream.angularVelocity = Vector3.Lerp(stream1.angularVelocity, stream2.angularVelocity, weight);
    }
}
public abstract class PlayableAdapter : IGamePool
{

    public static void Destroy(PlayableAdapter layerMaskAdapter)
    {
        layerMaskAdapter.OnDestroy();
        GameUtil.RecycleClass(layerMaskAdapter);
    }
    public virtual void Initialization(PlayableGraphAdapter graph)
    {
        //var job = new AnimJob();
        //var animPlayable = AnimationScriptPlayable.Create<AnimJob>(graph.GetGraph(), job, 0);
        //animPlayable.SetProcessInputs(false);

    }
    public virtual void OnDestroy()
    {

    }
    public abstract ScriptPlayable<AdapterPlayable> GetPlayable();
    public abstract void ConnectInputTo(PlayableAdapter playableAdapter, int portID);
    public abstract void ConnectOutputTo(int portID, PlayableAdapter playableAdapter);
    public virtual void ConnectTo(int outputPortID, ScriptPlayable<AdapterPlayable> toPlayable, int inputPortID)
    {
        
    }

    public virtual void PoolConstructor()
    {
    }

    public virtual void OnPoolGet()
    {
    }

    public virtual void OnPoolRecycle()
    {
    }

    public virtual void PoolRelease()
    {
    }
}