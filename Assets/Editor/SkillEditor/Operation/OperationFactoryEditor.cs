using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IClassPoolUserDataEditor : IClassPoolUserData
{

}
public interface IOperationInfoEditor : IOperationInfo, IEditorItem
{

}


public static class OperationFactoryEditor
{
    public static Type GetOperationDataType(EnOperationType oprationType, params int[] childType)
    {
        return oprationType switch
        {
            EnOperationType.Compare => GetOperationComperaDataType((EnOperationCompareType)childType[0]),
            _ => null,
        };
    }

    private static Type GetOperationComperaDataType(EnOperationCompareType comperaType)
    {
        return comperaType switch
        {
            EnOperationCompareType.Less => typeof(CompareLessInfoEditor),
            _ => null,
        };
    }
    public static void GetOperationChild()
    {

    }
}
