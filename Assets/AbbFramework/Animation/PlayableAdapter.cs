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
    private bool m_IsValid = false;
    protected int m_EntityID = -1;
    public bool GetIsValid()
    {
        return m_IsValid;
    }
    public static void Destroy(PlayableAdapter layerMaskAdapter)
    {
        layerMaskAdapter.OnDestroy();
        GameClassPoolMgr.Instance.Push(layerMaskAdapter);
    }
    public virtual void Initialization(int entityID, PlayableGraphAdapter graph)
    {
        m_IsValid = true;
        m_EntityID = entityID;
        //var job = new AnimJob();
        //var animPlayable = AnimationScriptPlayable.Create<AnimJob>(graph.GetGraph(), job, 0);
        //animPlayable.SetProcessInputs(false);

    }
    public virtual void OnDestroy()
    {
        m_IsValid = false;
        m_EntityID = -1;
    }
    public abstract ScriptPlayable<AdapterPlayable> GetPlayable();
    public virtual PlayableAdapter GetMainPlayableAdapter()
    {
        return this;
    }
    public virtual int ConnectInputTo(PlayableAdapter playableAdapter, int portID)
    {
        return -1;
    }
    public virtual void DisconnectInput(int inputPortID)
    {

    }
    public void SetSpeed(float speed)
    {
        var playable = GetPlayable();
        playable.SetSpeed(speed);
    }
    public double GetSpeed()
    {
        var playable = GetPlayable();
        var speed = playable.GetSpeed();
        return speed;
    }
    public virtual void Stop()
    {
        SetSpeed(0);
    }
    // 是否循环
    public virtual bool IsLoop()
    {
        return true;
    }
    // 循环一次需要的时间
    public virtual float GetUnitTime()
    {
        return -1;
    }
    public virtual double GetPlayTime()
    {
        var playable = GetPlayable();
        var time = playable.GetTime();
        return time;
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