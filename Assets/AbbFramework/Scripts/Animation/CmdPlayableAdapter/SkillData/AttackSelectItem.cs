using System;
using System.Collections.Generic;
using System.Reflection;


public class AttackSelectItemUserData : IClassPoolUserData
{
    public int[] arrParams = null;

    public void OnPoolDestroy()
    {
        arrParams = null;
    }
}
public class AttackSelectItem : ISkillData<AttackLinkSkillDataUserData>
{
    //public EntityPropertyInfo propertyInfo = null;
    public EnEntityProperty target = EnEntityProperty.None;
    public AttackSelectItemInfo[] itemInfo = null;

    public void OnPoolDestroy()
    {
        //ClassPoolMgr.Instance.Push(propertyInfo);
        foreach (var item in itemInfo)
            ClassPoolMgr.Instance.Push(item);

        itemInfo = null;
        //propertyInfo = null;
    }
    public void OnPoolInit(AttackLinkSkillDataUserData userData)
    {
        var arrParams = userData.arrParams;
        var startIndex = 0;
        var endIndex = arrParams?.Length ?? 0;

        var gCount = startIndex >= endIndex ? default : arrParams[startIndex++];
        target = gCount < 1 ? default : (EnEntityProperty)arrParams[startIndex++];
        //var gArrParamsCount = gCount < 2 ? default : arrParams[startIndex++];
        var clipItemCount = gCount < 2 ? default : arrParams[startIndex++];

        {
            
            //var targetArrParams = arrParams.Copy(startIndex, gArrParamsCount);
            //startIndex += gArrParamsCount;
            //propertyInfo = EntityPropertyMgr.Instance.CreatePropertyInfo(target, targetArrParams);
        }
        {

            itemInfo = new AttackSelectItemInfo[clipItemCount];
            for (int i = 0; i < clipItemCount; i++)
            {
                var itemElementCount = arrParams[startIndex++];
                var paramInfo = ClassPoolMgr.Instance.Pull<CommonSkillItemParamUserData>();
                paramInfo.startIndex = startIndex;
                paramInfo.arrParams = arrParams;
                paramInfo.paramCount = itemElementCount;
                var itemInfo = ClassPoolMgr.Instance.Pull<AttackSelectItemInfo>(paramInfo);
                ClassPoolMgr.Instance.Push(itemInfo);
                startIndex += itemElementCount;
                this.itemInfo[i] = itemInfo;
            }
        }
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

