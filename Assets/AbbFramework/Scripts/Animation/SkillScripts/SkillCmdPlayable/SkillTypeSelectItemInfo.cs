using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CommonSkillItemParamUserData : IClassPoolUserData
{
    public int startIndex;
    public int paramCount;
    public int[] arrParams;
    public void OnPoolDestroy()
    {
        startIndex = -1;
        paramCount = -1;
        arrParams = null;
    }
}

public class SkillTypeSelectItemInfo : IClassPool<CommonSkillItemParamUserData>
{
    public EnOperationType operationType = EnOperationType.None;

    public IOperationInfo operationInfo = null;

    public SkillItemInfo atkItemData = null;
    public void OnPoolDestroy()
    {
        ClassPoolMgr.Instance.Push(operationInfo);
        ClassPoolMgr.Instance.Push(atkItemData);
        atkItemData = null;
        operationInfo = null;
        operationType = EnOperationType.None;
    }

    public void OnPoolInit(CommonSkillItemParamUserData userData)
    {
        var arrParams = userData.arrParams;
        var startIndex = userData.startIndex;
        var endIndex = startIndex + userData.paramCount;

        var gCount = startIndex >= endIndex ? default : arrParams[startIndex++];
        operationType = gCount < 1 ? default : (EnOperationType)arrParams[startIndex++];
        //operationChildType = gCount < 2 ? default : arrParams[startIndex++];
        var childTypeCount = gCount < 2 ? default : arrParams[startIndex++];

        var childType = arrParams.Copy(startIndex, childTypeCount);
        startIndex += childTypeCount;

        //var operationParamsCount = startIndex >= endIndex ? default : arrParams[startIndex++];
        //operationTypeParam = arrParams.Copy(startIndex, operationParamsCount);
        //startIndex += operationParamsCount;


        var operationInfoParamsCount = startIndex >= endIndex ? default : arrParams[startIndex++];
        var para = ClassPoolMgr.Instance.Pull<CommonSkillItemParamUserData>();
        para.arrParams = arrParams;
        para.startIndex = startIndex;
        para.paramCount = operationInfoParamsCount;
        operationInfo = OperationFactory.Instance.CreateOperationData(operationType, para, childType);
        ClassPoolMgr.Instance.Push(para);
        startIndex += operationInfoParamsCount;

        var atkItemParamCount = startIndex >= endIndex ? default : arrParams[startIndex++];
        atkItemData = ClassPoolMgr.Instance.Pull<SkillItemInfo>();
        atkItemData.Init(arrParams, atkItemParamCount, ref startIndex);

    }

    public void OnPoolEnable()
    {
    }


    public void PoolConstructor()
    {
    }

    public void PoolRelease()
    {
    }
}