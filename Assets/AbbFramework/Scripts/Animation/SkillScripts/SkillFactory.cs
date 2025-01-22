using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SkillFactory
{
    public static ISkillBehaviour CreateSkillBehaviour(EnSkillBehaviourType type, CommonSkillItemParamUserData UserData)
    {
        return type switch
        {
            EnSkillBehaviourType.Height => ClassPoolMgr.Instance.Pull<SkillHeightBehaviourData>(UserData),
            _ => null,
        };
    }
    public static void DestroySkillBehaviour(ref ISkillBehaviour behaviour)
    {
        ClassPoolMgr.Instance.Push(behaviour);
        behaviour = null;
    }
}
