using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperationFactory : Singleton<OperationFactory>
{
    public IOperationInfo CreateOperationData(EnOperationType oprationType, CommonSkillItemParamUserData userData, params int[] childType)
    {
        return oprationType switch
        {
            EnOperationType.Compare => CreateOperationComperaData((EnOperationCompareType)childType[0], userData),
            _ => null,
        };
    }

    public ICompareInfo CreateOperationComperaData(EnOperationCompareType comperaType, CommonSkillItemParamUserData userData)
    {
        return comperaType switch
        {
            EnOperationCompareType.Less => ClassPoolMgr.Instance.Pull<CompareLessInfo>(userData),
            EnOperationCompareType.Equal => ClassPoolMgr.Instance.Pull<CompareEqualInfo>(userData),
            EnOperationCompareType.Greater => ClassPoolMgr.Instance.Pull<CompareGreaterInfo>(userData),
            _ => null,
        };
    }
}
