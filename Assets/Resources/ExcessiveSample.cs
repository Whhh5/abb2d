using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using Unity.Collections;

public class ExcessiveSample : MonoBehaviour
{
    private struct AnimaJob : IAnimationJob
    {
        public NativeArray<TransformStreamHandle> handles;
        public float weight;
        public void ProcessAnimation(AnimationStream stream)
        {
            var stream0 = stream.GetInputStream(0);
            var stream1 = stream.GetInputStream(1);
            var stream1IsVaild = stream1.isValid;
            foreach (var item in handles)
            {
                if (!stream1IsVaild)
                {
                    item.SetLocalPosition(stream, item.GetLocalPosition(stream0));
                    item.SetLocalRotation(stream, item.GetLocalRotation(stream0));
                    continue;
                }
                var pos = Vector3.Lerp(item.GetLocalPosition(stream0), item.GetLocalPosition(stream1), weight);
                item.SetLocalPosition(stream, pos);

                var rot = Quaternion.Slerp(item.GetLocalRotation(stream0), item.GetLocalRotation(stream1), weight);
                item.SetLocalRotation(stream, rot);

            }
        }

        public void ProcessRootMotion(AnimationStream stream)
        {
            var stream0 = stream.GetInputStream(0);
            var stream1 = stream.GetInputStream(1);
            if (!stream1.isValid)
            {
                stream.velocity = stream0.velocity;
                stream.angularVelocity = stream0.angularVelocity;
                return;
            }

            stream.velocity = Vector3.Lerp(stream0.velocity, stream1.velocity, weight);
            stream.angularVelocity = Vector3.Lerp(stream0.angularVelocity, stream1.angularVelocity, weight);
        }
    }
    private unsafe class AnimationExcessive
    {
        private PlayableGraph graph;
        private AnimationMixerPlayable mixer;
        public AnimationMixerPlayable fromPlayable;
        public AnimationMixerPlayable toPlayable;
        private AnimationMixerPlayable targetMixer;
        private int targetPort;
        public float durection;
        private float time;
        public bool isDone;
        private int fromPort;
        private int toPort;
        private float weight;
        private float startTime;
        public void Reset()
        {
            weight
                = durection
                = time
                = startTime
                = 0;
            fromPort
                = toPort
                = targetPort
                = 0;
            isDone = false;
            mixer
                = fromPlayable
                = toPlayable
                = targetMixer
                = AnimationMixerPlayable.Null;
            graph = default;

        }
        public bool IsVaild()
        {
            return mixer.IsValid();
        }
        public void Awake()
        {
            startTime = Time.time;
            targetPort = 0;
            targetMixer = (AnimationMixerPlayable)fromPlayable.GetOutput(targetPort);
            targetMixer.DisconnectInput(targetPort);
            graph = fromPlayable.GetGraph();
            mixer = AnimationMixerPlayable.Create(graph);
            fromPlayable.SetSpeed(0);
            toPort = mixer.AddInput(toPlayable, 0, 0);
            fromPort = mixer.AddInput(fromPlayable, 0, 1);
            targetMixer.ConnectInput(targetPort, mixer, 0, 1);
        }
        public void Finish()
        {
            isDone = true;
            //targetMixer.DisconnectInput(targetPort);
            if (mixer.IsValid())
            {
                mixer.DisconnectInput(fromPort);
                mixer.SetInputCount(1);
            }
            //mixer.DisconnectInput(toPort);
            //targetMixer.ConnectInput(targetPort, toPlayable, 0, 1.0f);
            //targetMixer.SetInputWeight(targetPort, 1);

            fromPlayable.Destroy();
        }
        public void Update(float timeDelta)
        {
            if (isDone)
                return;
            if (weight == 1 || !IsVaild())
            {
                Finish();
                return;
            }

            time += timeDelta;
            weight = Mathf.Min(1, time / durection);
            mixer.SetInputWeight(fromPort, 1 - weight);
            mixer.SetInputWeight(toPort, weight);
        }
    }

    public Animator animator = null;
    public Transform rootBone = null;
    public float startTime;
    public AnimationClip clipIdle;
    public AnimationClip clip1;
    public AnimationClip clip2;
    public AnimationClip clip3;
    public AnimationClip clip4;
    public AnimationClip clip5;
    public AnimationClip clip6;
    [Range(0, 1)]
    public float weight;

    private PlayableGraph graph;
    private AnimationClipPlayable clipPlayable1;
    private AnimationClipPlayable clipPlayable2;
    private AnimationMixerPlayable mixerPlayable;
    private AnimationMixerPlayable graphMixer;
    private HashSet<AnimationExcessive> m_ExcessiveList = new();
    private HashSet<AnimationExcessive> m_DesList = new();
    private NativeArray<TransformStreamHandle> handles;
    private bool isIdle = false;
    private void OnDestroy()
    {
        //handles.Dispose();
        graph.Destroy();
        clipPlayable1.Destroy();
    }
    // Start is called before the first frame update
    void Start()
    {
        var trans = rootBone.GetComponentsInChildren<Transform>();
        handles = new(trans.Length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
        for (int i = 0; i < trans.Length; i++)
        {
            handles[i] = animator.BindStreamTransform(trans[i]);
        }
        graph = PlayableGraph.Create();
        graphMixer = AnimationMixerPlayable.Create(graph);
        mixerPlayable = AnimationMixerPlayable.Create(graph);
        mixerPlayable.SetPropagateSetTime(true);
        clipPlayable1 = AnimationClipPlayable.Create(graph, clipIdle);
        graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

        var output = AnimationPlayableOutput.Create(graph, "_", animator);
        output.SetSourcePlayable(graphMixer);
        graphMixer.AddInput(mixerPlayable, 0, 1f);
        mixerPlayable.AddInput(clipPlayable1, 0, 1);

        graph.Play();
    }

    private float finishTime = 0;
    // Update is called once per frame
    void Update()
    {
        foreach (var item in m_DesList)
        {
            item.Reset();
            m_ExcessiveList.Remove(item);
        }
        m_DesList.Clear();
        foreach (var item in m_ExcessiveList)
        {
            item.Update(Time.deltaTime);
            if (item.isDone)
                m_DesList.Add(item);
        }
        if (!isIdle && finishTime < Time.time)
        {
            PlayAnim(clipIdle, 2);
            isIdle = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            var time = mixerPlayable.GetTime();
            time += Time.deltaTime;
            mixerPlayable.SetTime(time);
            //graph.Evaluate();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (mixerPlayable.GetSpeed() == 1)
            {
                mixerPlayable.SetSpeed(0);
            }
            else
            {
                mixerPlayable.SetSpeed(1);
            }

        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlayAnim(clip1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PlayAnim(clip2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            PlayAnim(clip3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            PlayAnim(clip4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            PlayAnim(clip5);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            PlayAnim(clip6);
        }
        //weight = Mathf.Min(1, weight + Time.deltaTime);
        //mixerPlayable.SetInputWeight(0, weight);
        //mixerPlayable.SetInputWeight(1, 1 - weight);


    }
    private void PlayAnim(AnimationClip f_TargetClip, float f_ExcessiveTime = 0.8f)
    {
        isIdle = false;
        finishTime = Time.time + f_TargetClip.length;
        var newMixer = AnimationMixerPlayable.Create(graph);
        newMixer.SetPropagateSetTime(true);

        var clipPlayable = AnimationClipPlayable.Create(graph, f_TargetClip);
        newMixer.AddInput(clipPlayable, 0, 1.0f);

        //graphMixer.DisconnectInput(0);

        var curMixer = graphMixer.GetInput(0);

        var excessive = new AnimationExcessive();
        excessive.fromPlayable = (AnimationMixerPlayable)curMixer;
        excessive.toPlayable = newMixer;
        excessive.durection = Random.Range(f_ExcessiveTime * 0.5f, f_ExcessiveTime * 1.5f);
        excessive.Awake();
        m_ExcessiveList.Add(excessive);
    }
}
