using UnityEngine;
using UnityEngine.Animations;

public struct PlayableGraphAnimJob : IAnimationJob
{
    public bool applyRootMotion;
    public float leftFootIKWeight;
    public float rightFootIKWeight;
    public Vector3 leftFeatWorldPos;
    public Vector3 rightFeatWorldPos;
    public Quaternion leftFootQuaternion;
    public Vector3 lastLeftFootPos;
    public Quaternion lastLeftFootRot;
    public Vector3 lastRightFootPos;
    public Quaternion lastRightFootRot;
    public void ProcessAnimation(AnimationStream stream)
    {
        var human = stream.AsHuman();
        lastLeftFootPos = human.GetGoalPosition(AvatarIKGoal.LeftFoot);
        lastLeftFootRot = human.GetGoalRotation(AvatarIKGoal.LeftFoot);
        lastRightFootPos = human.GetGoalPosition(AvatarIKGoal.RightFoot);
        lastRightFootRot = human.GetGoalRotation(AvatarIKGoal.RightFoot);
        if (leftFootIKWeight != 0)
        {
            leftFeatWorldPos += (lastLeftFootPos - leftFeatWorldPos).normalized * human.leftFootHeight;
            human.SetGoalPosition(AvatarIKGoal.LeftFoot, leftFeatWorldPos);
            //human.SetGoalRotation(AvatarIKGoal.LeftFoot, leftFootQuaternion);

            human.SetGoalWeightPosition(AvatarIKGoal.LeftFoot, leftFootIKWeight);
            //human.SetGoalWeightRotation(AvatarIKGoal.LeftFoot, footIKWeight);
        }

        if (rightFootIKWeight != 0)
        {
            rightFeatWorldPos += (lastRightFootPos - rightFeatWorldPos).normalized * human.rightFootHeight;
            human.SetGoalPosition(AvatarIKGoal.RightFoot, rightFeatWorldPos);
            human.SetGoalWeightPosition(AvatarIKGoal.RightFoot, rightFootIKWeight);
        }


        human.SolveIK();

        var temp1 = human.GetGoalPosition(AvatarIKGoal.LeftFoot);
        var temp2 = human.GetGoalPosition(AvatarIKGoal.RightFoot);

        var dis1 = Vector3.Distance(temp1, lastLeftFootPos) * leftFootIKWeight;
        var dis2 = Vector3.Distance(temp2, lastRightFootPos) * rightFootIKWeight;

        var maxDis = Mathf.Max(dis1, dis2);

        var curBodyPos = human.bodyPosition;
        var bodyPos = curBodyPos - Vector3.up * Mathf.Max(maxDis - human.leftFootHeight, 0);
        human.bodyPosition = bodyPos;

        //human.
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