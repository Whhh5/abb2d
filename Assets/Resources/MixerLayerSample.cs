using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Audio;
using UnityEngine.Playables;

public class MixerLayerSample : MonoBehaviour
{
    public Animator animator;
    public AnimationClip clipTop;
    public AnimationClip clipDown;
    public AvatarMask avatarMask;

    private PlayableGraph graph;
    private AnimationLayerMixerPlayable layerMixerTop;
    private AnimationLayerMixerPlayable layerMixerDown;
    private AnimationMixerPlayable mixer;
    [Range(0, 1)]
    public float weight;
    void Start()
    {
        graph = PlayableGraph.Create();


        var clipDownPlableable = AnimationClipPlayable.Create(graph, clipDown);
        mixer = AnimationMixerPlayable.Create(graph);
        //mixer.AddInput(layerMixerTop, 0, 1);
        mixer.AddInput(clipDownPlableable, 0, 1);

        layerMixerTop = AnimationLayerMixerPlayable.Create(graph, 0, true);
        var clipTopPlableable = AnimationClipPlayable.Create(graph, clipTop);
        layerMixerTop.AddInput(mixer, 0, 1);
        layerMixerTop.AddInput(clipTopPlableable, 0, 1);
        layerMixerTop.SetLayerAdditive(0, false);
        layerMixerTop.SetLayerAdditive(1, false);
        AvatarMask mask = new();
        mask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.LeftArm, true);
        mask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.RightArm, true);
        mask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.LeftFingers, true);
        mask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.RightFingers, true);

        // ���ڵ�
        mask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.Root, false);
        // ����
        mask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.Body, false);
        // ͷ
        mask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.Head, false);
        // �Ȳ�
        mask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.LeftLeg, false);
        mask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.RightLeg, false);
        // �첲
        //| AvatarMaskBodyPart.LeftArm
        //| AvatarMaskBodyPart.RightArm
        // ��ָ
        //| AvatarMaskBodyPart.LeftFingers
        //| AvatarMaskBodyPart.RightFingers
        // �� IK
        mask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.LeftFootIK, false);
        mask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.RightFootIK, false);
        // �� IK
        mask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.LeftHandIK, false);
        mask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.RightHandIK, false);
        // 
        mask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.LastBodyPart, false);

        layerMixerTop.SetLayerMaskFromAvatarMask(1, mask);
        //layerMixerTop.SetTraversalMode(PlayableTraversalMode.Passthrough);

        //layerMixerDown = AnimationLayerMixerPlayable.Create(graph, 0, true);
        //layerMixerDown.AddInput(clipDownPlableable, 0, 1);
        //layerMixerDown.SetLayerAdditive(0, false);


        var output = AnimationPlayableOutput.Create(graph, "__", animator);
        output.SetSourcePlayable(layerMixerTop);
        graph.Play();
    }


    void Update()
    {
        //mixer.SetInputWeight(0, weight);
        layerMixerTop.SetInputWeight(1, weight);
        layerMixerTop.SetInputWeight(0, 1 - weight);

        if (Input.GetKeyDown(KeyCode.D))
        {
            layerMixerTop.DisconnectInput(1);
        }
    }
}
