using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillBehaviour : ISkillBehaviour
{
    protected int[] _ArrValue;
    public abstract void Execute(int entityID);
    public abstract void Init(ref int[] arrValue);
    public abstract EnSkillBehaviourType SkillBehavioueType();

    public virtual void OnPoolDestroy()
    {
    }

    public void OnPoolEnable()
    {
    }

    public void OnPoolInit(CommonSkillItemParamUserData userData)
    {
        _ArrValue = userData.arrParams.Copy(userData.startIndex, userData.paramCount);
        Init(ref _ArrValue);
    }

    public void PoolConstructor()
    {
    }

    public void PoolRelease()
    {
    }
}
