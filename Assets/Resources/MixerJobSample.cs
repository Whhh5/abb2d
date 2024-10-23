using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public struct MixerJob : IAnimationJob
{
    public NativeArray<TransformStreamHandle> handles;
    public float weight;
    public void ProcessAnimation(AnimationStream stream)
    {
        var stream0 = stream.GetInputStream(0);
        var stream1 = stream.GetInputStream(1);

        for (int i = 0; i < handles.Length; i++)
        {
            var handle = handles[i];
            var pos = Vector3.Lerp(handle.GetLocalPosition(stream0), handle.GetLocalPosition(stream1), weight);
            handle.SetLocalPosition(stream, pos);

            Quaternion rot = Quaternion.Slerp(handle.GetLocalRotation(stream0), handle.GetLocalRotation(stream1), weight);
            handle.SetLocalRotation(stream, rot);
        }
    }

    public void ProcessRootMotion(AnimationStream stream)
    {
        var stream0 = stream.GetInputStream(0);
        var stream1 = stream.GetInputStream(1);

        stream.velocity = Vector3.Lerp(stream0.velocity, stream1.velocity, weight);
        stream.angularVelocity = Vector3.Lerp(stream0.angularVelocity, stream1.angularVelocity, weight);
    }
}

public class MixerJobSample : MonoBehaviour
{
    public Animator animator = null;
    public AnimationClip clip1;
    public AnimationClip clip2;
    public Transform rootBone;
    [Range(0, 1)]
    public float weight;
    private PlayableGraph graph;
    private NativeArray<TransformStreamHandle> handles;
    private AnimationScriptPlayable jobPlayable;
    private void OnDestroy()
    {
        graph.Destroy();
        handles.Dispose();
        jobPlayable.Destroy();
    }
    // Start is called before the first frame update
    void Start()
    {
        graph = PlayableGraph.Create();

        var bones = rootBone.GetComponentsInChildren<Transform>();
        handles = new(bones.Length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
        for (int i = 0; i < bones.Length; i++)
        {
            handles[i] = animator.BindStreamTransform(bones[i]);
        }
        MixerJob job = new MixerJob();
        job.handles = handles;
        job.weight = weight;
        jobPlayable = AnimationScriptPlayable.Create(graph, job);
        jobPlayable.SetProcessInputs(false);

        jobPlayable.AddInput(AnimationClipPlayable.Create(graph, clip1), 0, 1f);
        jobPlayable.AddInput(AnimationClipPlayable.Create(graph, clip2), 0, 0f);
        AnimationPlayableOutput output = AnimationPlayableOutput.Create(graph, "__", animator);
        output.SetSourcePlayable(jobPlayable);
        //graph.Play();
        graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
    }

    // Update is called once per frame
    void Update()
    {
        var mixerJob = jobPlayable.GetJobData<MixerJob>();
        mixerJob.weight = weight;
        jobPlayable.SetJobData(mixerJob);
    }
}
