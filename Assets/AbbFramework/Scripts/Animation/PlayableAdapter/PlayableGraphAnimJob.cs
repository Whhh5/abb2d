using System.IO;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using UnityEngine;
using UnityEngine.Animations;


public struct FootIKInfo
{
    public TransformStreamHandle lowerLegHandle;
    public float weight;

    public Vector3 worldPos;
    public Quaternion quaternion;
    public Vector3 direction;
    public Vector3 lastWorldPos;
    public Quaternion lastQuaternion;


    public float curWeight;
}
public struct PlayableGraphAnimJob : IAnimationJob
{
    public bool isActivity;
    public bool applyRootMotion;
    public float lastOffsetY;

    public FootIKInfo leftFootIKInfo;
    public FootIKInfo rightFootIKInfo;
    public void ProcessAnimation(AnimationStream stream)
    {
        isActivity = true;
        var human = stream.AsHuman();

        UpdateFootData(ref stream, ref human, ref leftFootIKInfo, AvatarIKGoal.LeftFoot);
        UpdateFootData(ref stream, ref human, ref rightFootIKInfo, AvatarIKGoal.RightFoot);

        var curBodyPos = human.bodyPosition;

        var temp1 = human.GetGoalPosition(AvatarIKGoal.LeftFoot);
        var temp2 = human.GetGoalPosition(AvatarIKGoal.RightFoot);


        var dis1 = Mathf.Abs(leftFootIKInfo.worldPos.y - leftFootIKInfo.lastWorldPos.y) * leftFootIKInfo.curWeight;
        var dis2 = Mathf.Abs(rightFootIKInfo.worldPos.y - rightFootIKInfo.lastWorldPos.y) * rightFootIKInfo.curWeight;

        var maxDis = Mathf.Max(dis1, dis2);
        var minY = Mathf.Min(temp1.y, temp2.y);


        var bodyWeight = Mathf.Abs(curBodyPos.y - minY) > 1f ? 1 : 0;

        var offsetY = Mathf.Max(maxDis, 0);
        lastOffsetY = Mathf.Lerp(lastOffsetY, offsetY * bodyWeight, 0.01f);

        var bodyPos = curBodyPos - Vector3.up * lastOffsetY;
        human.bodyPosition = bodyPos;
        human.SolveIK();
    }

    private void UpdateFootData(ref AnimationStream stream, ref AnimationHumanStream human, ref FootIKInfo info, AvatarIKGoal iKGoal)
    {
        info.direction = info.lowerLegHandle.GetRotation(stream) * -Vector3.right;

        info.lastWorldPos = human.GetGoalPosition(iKGoal);
        info.lastQuaternion = human.GetGoalRotation(iKGoal);



        var lerpSpeed = 0f;
        var line = 0.3f;
        if (info.weight > line && isActivity)
        {
            lerpSpeed = Mathf.Lerp(0, 0.01f, (info.weight - line) / (1 - line));
        }
        else
        {
            lerpSpeed = Mathf.Lerp(0, 0.2f, (line - info.weight) / line);
        }

        var toWeight = isActivity ? Mathf.Clamp01(Mathf.Ceil(info.weight - line)) : 0;
        //var t = (!isActivity || toWeight == 0) ? 0.1f : 0.01f;
        var weight = Mathf.Lerp(info.curWeight, toWeight, lerpSpeed);
        human.SetGoalPosition(iKGoal, info.worldPos);
        human.SetGoalRotation(iKGoal, info.quaternion);
        human.SetGoalWeightPosition(iKGoal, weight);
        human.SetGoalWeightRotation(iKGoal, weight);

        info.curWeight = weight;
    }

    public void ProcessRootMotion(AnimationStream stream)
    {

        if (!applyRootMotion)
        {
            stream.velocity = Vector3.zero;
            return;
        }
    }
}