//using System.IO;
//using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
//using UnityEngine;
//using UnityEngine.Animations;


//public struct FootIKInfo
//{
//    public TransformStreamHandle lowerLegHandle;
//    public float weight;

//    public Vector3 worldPos;
//    public Quaternion quaternion;
//    public Vector3 direction;
//    public Vector3 lastWorldPos;
//    public Quaternion lastQuaternion;


//    public float curWeight;
//}
//public struct PlayableGraphAnimJob : IAnimationJob
//{
//    public bool isActivity;
//    public bool applyRootMotion;
//    public float lastOffsetY;

//    public FootIKInfo leftFootIKInfo;
//    public FootIKInfo rightFootIKInfo;
//    public void ProcessAnimation(AnimationStream stream)
//    {
//        isActivity = true;
//        var human = stream.AsHuman();

//        //leftFootIKInfo.direction = leftFootIKInfo.lowerLegHandle.GetRotation(stream) * -Vector3.right;
//        //rightFootIKInfo.direction = rightFootIKInfo.lowerLegHandle.GetRotation(stream) * -Vector3.right;

//        //leftFootIKInfo.lastWorldPos = human.GetGoalPosition(AvatarIKGoal.LeftFoot);
//        //leftFootIKInfo.lastQuaternion = human.GetGoalRotation(AvatarIKGoal.LeftFoot);
//        //rightFootIKInfo.lastWorldPos = human.GetGoalPosition(AvatarIKGoal.RightFoot);
//        //rightFootIKInfo.lastQuaternion = human.GetGoalRotation(AvatarIKGoal.RightFoot);

//        //UpdateFootData(ref stream, ref human, ref leftFootIKInfo, AvatarIKGoal.LeftFoot);
//        //UpdateFootData(ref stream, ref human, ref rightFootIKInfo, AvatarIKGoal.RightFoot);


//        human.SetGoalPosition(AvatarIKGoal.LeftFoot, Vector3.zero);
//        human.SetGoalRotation(AvatarIKGoal.LeftFoot, Quaternion.identity);
//        human.SetGoalWeightPosition(AvatarIKGoal.LeftFoot, 0.1f);
//        human.SetGoalWeightRotation(AvatarIKGoal.LeftFoot, 0.1f);


//        //leftFootIKInfo.lowerLegHandle.Resolve(stream);

//        human.SolveIK();

//        //if (leftFootIKInfo.lowerLegHandle.IsResolved(stream))
//        //{

//        //}

//        var curBodyPos = human.bodyPosition;

//        var temp1 = human.GetGoalPosition(AvatarIKGoal.LeftFoot);
//        var temp2 = human.GetGoalPosition(AvatarIKGoal.RightFoot);


//        var dis1 = Vector3.Distance(temp1, leftFootIKInfo.lastWorldPos) * leftFootIKInfo.curWeight;
//        var dis2 = Vector3.Distance(temp2, rightFootIKInfo.lastWorldPos) * rightFootIKInfo.curWeight;

//        var maxDis = Mathf.Max(dis1, dis2);
//        var minY = Mathf.Min(temp1.y, temp2.y);


//        var bodyWeight = Mathf.Abs(curBodyPos.y - minY) > 1f ? 1 : 0;

//        var offsetY = Mathf.Max(maxDis - human.leftFootHeight, 0);
//        lastOffsetY = Mathf.Lerp(lastOffsetY, offsetY * bodyWeight, 0.01f);

//        var bodyPos = curBodyPos - Vector3.up * lastOffsetY;
//        human.bodyPosition = bodyPos;
//    }

//    private void UpdateFootData(ref AnimationStream stream, ref AnimationHumanStream human,ref FootIKInfo info, AvatarIKGoal iKGoal)
//    {
//        info.direction = info.lowerLegHandle.GetRotation(stream) * -Vector3.right;

//        info.lastWorldPos = human.GetGoalPosition(iKGoal);
//        info.lastQuaternion = human.GetGoalRotation(iKGoal);

        

//        var toWeight = isActivity ? Mathf.Clamp01(Mathf.Ceil(info.weight - 0.3f)) : 0;
//        var t = (!isActivity || toWeight == 0) ? 0.02f: 0.01f;
//        var weight = Mathf.Lerp(info.curWeight, toWeight, t);
//        human.SetGoalPosition(iKGoal, info.worldPos);
//        human.SetGoalRotation(iKGoal, info.quaternion);
//        human.SetGoalWeightPosition(iKGoal, 0f);
//        human.SetGoalWeightRotation(iKGoal, 0f);

//        info.lowerLegHandle.Resolve(stream);

//        info.curWeight = weight;
//    }

//    public void ProcessRootMotion(AnimationStream stream)
//    {

//        if (!applyRootMotion)
//        {
//            stream.velocity = Vector3.zero;
//            return;
//        }
//    }
//}