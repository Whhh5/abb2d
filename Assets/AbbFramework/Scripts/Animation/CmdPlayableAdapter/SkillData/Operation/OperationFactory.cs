using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperationFactory : Singleton<OperationFactory>
{
    public IOperationInfo CreateOperationData(EnOperationType oprationType, IClassPoolUserData userData, params int[] childType)
    {
        return oprationType switch
        {
            EnOperationType.Compare => CreateOperationComperaData((EnOperationCompareType)childType[0], userData),
            _ => null,
        };
    }

    private ICompareInfo CreateOperationComperaData(EnOperationCompareType comperaType, IClassPoolUserData userData)
    {
        return comperaType switch
        {
            EnOperationCompareType.Less => ClassPoolMgr.Instance.Pull<CompareLessInfo>(),
            _ => null,
        };
    }
}
