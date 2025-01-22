using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkillBehaviour : IClassPool<CommonSkillItemParamUserData>
{
    public EnSkillBehaviourType SkillBehavioueType();
    public void Execute(int entityID);
}
