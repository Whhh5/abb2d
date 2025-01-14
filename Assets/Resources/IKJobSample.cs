using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public struct FootIKJob : IAnimationJob
{
    public TransformSceneHandle leftFootHandle;
    public float weight;
    public Transform tran;

    public void ProcessAnimation(AnimationStream stream)
    {
        //var pos = leftFootHandle.GetPosition(stream);
        //var rot = leftFootHandle.GetRotation(stream);
        //var human = stream.AsHuman();

        //var curPos = human.GetGoalPosition(AvatarIKGoal.LeftFoot);

        //var layer = (int)Mathf.Pow(2, (int)EnGameLayer.Terrain);
        //var hit = Physics.SphereCastAll(curPos, 1f, Vector3.one, 1f, layer);
        //Debug.Log(hit);

        //human.SetGoalPosition(AvatarIKGoal.LeftFoot, pos);
        //human.SetGoalRotation(AvatarIKGoal.LeftFoot, rot);
        //human.SetGoalWeightPosition(AvatarIKGoal.LeftFoot, weight);
        //human.SetGoalWeightRotation(AvatarIKGoal.LeftFoot, weight);
        //human.SolveIK();

    }

    public void ProcessRootMotion(AnimationStream stream)
    {
        //var human = stream.AsHuman();

        stream.rootMotionPosition.Set(0,0,0);
    }
}

public class IKJobSample : MonoBehaviour
{
    public Animator animator;
    public AnimationClip clip;
    public Transform leftFootEffector;
    [Range(0, 1)]
    public float weight;

    private PlayableGraph graph;
    private AnimationScriptPlayable jobPlayable;

    public float time;

    // Start is called before the first frame update
    void Start()
    {
        graph = PlayableGraph.Create();

        FootIKJob job = new();
        job.leftFootHandle = animator.BindSceneTransform(leftFootEffector);
        job.weight = weight;
        job.tran = leftFootEffector;
        jobPlayable = AnimationScriptPlayable.Create(graph, job);
        AnimationClipPlayable anim = AnimationClipPlayable.Create(graph, clip);
        anim.SetApplyFootIK(false);
        anim.SetApplyPlayableIK(false);

        jobPlayable.AddInput(anim, 0, 1f);

        AnimationPlayableOutput output = AnimationPlayableOutput.Create(graph, "_", animator);
        output.SetSourcePlayable(jobPlayable);
        //jobPlayable.SetPropagateSetTime(false);
        //jobPlayable.SetSpeed(0);
        //jobPlayable.SetSpeed(0);

        graph.Play();
    }

    // Update is called once per frame
    void Update()
    {
        jobPlayable.SetTime(time);
        FootIKJob job = jobPlayable.GetJobData<FootIKJob>();
        job.weight = weight;
        jobPlayable.SetJobData(job);
    }
}
