using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public struct FootIKJob: IAnimationJob
{
    public TransformSceneHandle leftFootHandle;
    public float weight;

    public void ProcessAnimation(AnimationStream stream)
    {

        var human = stream.AsHuman();
        human.SetGoalLocalPosition(AvatarIKGoal.LeftFoot, leftFootHandle.GetPosition(stream));
        human.SetGoalLocalRotation(AvatarIKGoal.LeftFoot, leftFootHandle.GetRotation(stream));
        human.SetGoalWeightPosition(AvatarIKGoal.LeftFoot, weight);
        human.SetGoalWeightRotation(AvatarIKGoal.LeftFoot, weight);
        human.SolveIK();
    }

    public void ProcessRootMotion(AnimationStream stream)
    {
        
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
        jobPlayable = AnimationScriptPlayable.Create(graph, job);
        AnimationClipPlayable anim = AnimationClipPlayable.Create(graph, clip);
        anim.SetApplyFootIK(false);
        anim.SetApplyPlayableIK(false);

        jobPlayable.AddInput(anim, 0, 1f);

        AnimationPlayableOutput output = AnimationPlayableOutput.Create(graph, "_", animator);
        output.SetSourcePlayable(jobPlayable);
        //jobPlayable.SetPropagateSetTime(false);
        jobPlayable.SetSpeed(0);
        jobPlayable.SetSpeed(0);

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
