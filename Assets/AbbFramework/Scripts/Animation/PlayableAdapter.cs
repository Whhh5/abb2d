using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using Unity.Collections;
using System;

public interface IPlayableAdapterCustomData: IGamePool
{
    //public void OnPoolRecycle();
}
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
    public static T Create<T>(PlayableGraphAdapter graph)
        where T : PlayableAdapter, new()
    {
        var adapter = Create<T>(graph, null);
        return adapter;
    }
    public static T Create<T>(PlayableGraphAdapter graph, IPlayableAdapterCustomData customData)
        where T : PlayableAdapter, new()
    {
        var adapter = GameClassPoolMgr.Instance.Pull<T>();
        adapter.Initialization(graph, customData);
        return adapter;
    }
    public static void Destroy(PlayableAdapter adapter)
    {
        adapter.OnDestroy();
        GameClassPoolMgr.Instance.Push(adapter);
    }
    public static implicit operator Playable(PlayableAdapter playable)
    {
        return playable.GetPlayable();
    }
    private bool m_IsValid = false;
    private ScriptPlayable<AdapterPlayable> m_MainPlayable;
    protected PlayableGraphAdapter m_Graph = null;
    protected virtual void OnDestroy()
    {
        m_MainPlayable.Destroy();
        m_IsValid = false;
        m_Graph = null;
    }
    protected virtual void Initialization(PlayableGraphAdapter graph, IPlayableAdapterCustomData customData)
    {
        m_IsValid = true;
        m_Graph = graph;
        m_MainPlayable = ScriptPlayable<AdapterPlayable>.Create(graph.GetGraph(), GlobalConfig.Int0);
        m_MainPlayable.GetBehaviour().Initialization(this);
        //var job = new AnimJob();
        //var animPlayable = AnimationScriptPlayable.Create<AnimJob>(graph.GetGraph(), job, 0);
        //animPlayable.SetProcessInputs(false);

    }
    public virtual PlayableAdapter GetMainPlayableAdapter()
    {
        return this;
    }
    protected void DisconnectRootAdapter(int inputPort = GlobalConfig.Int0)
    {
        m_MainPlayable.DisconnectInput(inputPort);
    }
    protected void ConnectRootAdapter(Playable playable)
    {
        ConnectRootAdapter(GlobalConfig.Int0, playable);
    }
    protected void ConnectRootAdapter(int inputPort, Playable playable, int sourceOutput = GlobalConfig.Int0, float weight = GlobalConfig.Float1)
    {
        m_MainPlayable.ConnectInput(inputPort, playable, sourceOutput, weight);
    }
    protected int AddConnectRootAdapter(Playable playable, int sourceOutput = GlobalConfig.Int0, float weight = GlobalConfig.Float1)
    {
        var port = m_MainPlayable.AddInput(playable, sourceOutput, weight);
        return port;
    }
    public bool GetIsValid()
    {
        return m_IsValid;
    }
    public virtual ScriptPlayable<AdapterPlayable> GetPlayable()
    {
        return m_MainPlayable;
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
    public virtual void Complete()
    {
        SetSpeed(0);
    }
    // 是否循环
    public virtual bool IsLoop()
    {
        return false;
    }
    // 循环一次需要的时间
    public abstract float GetUnitTime();
    public abstract float GetPlayTime();
    public float GetPlaySchedule()
    {
        var schedule = GetPlayTime() / GetUnitTime();
        return schedule;
    }
    public float GetPlaySchedule01()
    {
        var schedule = GetPlaySchedule();
        return Mathf.Clamp01(schedule);
    }
    public virtual int GetPlayCount()
    {
        var schedule = GetPlaySchedule();
        return Mathf.FloorToInt(schedule);
    }
    public virtual EnAnimLayer GetOutputLayer()
    {
        return EnAnimLayer.Layer1;
    }

    /// <summary>
    ///   <para>This function is called when the Playable that owns the PlayableBehaviour is created.</para>
    /// </summary>
    /// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
    public virtual void OnPlayableCreate()
    {
    }

    /// <summary>
	///   <para>This function is called when the PlayableGraph that owns this PlayableBehaviour starts.</para>
	/// </summary>
	/// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
	public virtual void OnGraphStart(Playable playable)
    {
    }

    /// <summary>
    ///   <para>This function is called when the PlayableGraph that owns this PlayableBehaviour stops.</para>
    /// </summary>
    /// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
    public virtual void OnGraphStop(Playable playable)
    {
    }


    /// <summary>
    ///   <para>This function is called when the Playable that owns the PlayableBehaviour is destroyed.</para>
    /// </summary>
    /// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
    public virtual void OnPlayableDestroy(Playable playable)
    {
    }


    /// <summary>
    ///   <para>This function is called when the Playable play state is changed to Playables.PlayState.Playing.</para>
    /// </summary>
    /// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
    /// <param name="info">A FrameData structure that contains information about the current frame context.</param>
    public virtual void OnBehaviourPlay(Playable playable, FrameData info)
    {
    }

    /// <summary>
    ///   <para>This method is invoked when one of the following situations occurs:
    /// &lt;br&gt;&lt;br&gt;
    ///      The effective play state during traversal is changed to Playables.PlayState.Paused. This state is indicated by FrameData.effectivePlayState.&lt;br&gt;&lt;br&gt;
    ///      The PlayableGraph is stopped while the playable play state is Playing. This state is indicated by PlayableGraph.IsPlaying returning true.</para>
    /// </summary>
    /// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
    /// <param name="info">A FrameData structure that contains information about the current frame context.</param>
    public virtual void OnBehaviourPause(Playable playable, FrameData info)
    {
    }

    /// <summary>
    ///   <para>This function is called during the PrepareData phase of the PlayableGraph.</para>
    /// </summary>
    /// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
    /// <param name="info">A FrameData structure that contains information about the current frame context.</param>
    public virtual void PrepareData(Playable playable, FrameData info)
    {
    }

    /// <summary>
    ///   <para>This function is called during the PrepareFrame phase of the PlayableGraph.</para>
    /// </summary>
    /// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
    /// <param name="info">A FrameData structure that contains information about the current frame context.</param>
    public void PrepareFrame(Playable playable, FrameData info)
    {
        OnPrepareFrame(playable, info);
    }
    public virtual bool OnPrepareFrame(Playable playable, FrameData info)
    {
        return true;
    }

    /// <summary>
    ///   <para>This function is called during the ProcessFrame phase of the PlayableGraph.</para>
    /// </summary>
    /// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
    /// <param name="info">A FrameData structure that contains information about the current frame context.</param>
    /// <param name="playerData">The user data of the ScriptPlayableOutput that initiated the process pass.</param>
    public virtual void ProcessFrame(Playable playable, FrameData info, object playerData)
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
    // 显式转换从double到Fahrenheit    
    //public static explicit operator Playable(PlayableAdapter playable)
    //{
    //    Playable p = playable;
    //    return new Fahrenheit(d);
    //}
}