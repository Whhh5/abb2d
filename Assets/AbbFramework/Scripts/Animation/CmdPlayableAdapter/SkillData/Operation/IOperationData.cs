using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOperationInfo: IClassPool<CommonSkillItemParamUserData>
{
    public int[] GetChildType();
}
