using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOperationInfo : IClassPool<CommonSkillItemParamUserData>
{
    public int[] GetChildType();
    // float -> value * 100, int -> value * 100
    public bool CompareResult(int target);
}
//public interface IOperationInfo<T> : IOperationInfo
//{
//    public bool CompareResult(T target);
//}
