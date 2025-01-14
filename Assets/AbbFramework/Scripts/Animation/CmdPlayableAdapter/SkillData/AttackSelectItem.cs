using System;
using System.Collections.Generic;
using System.Reflection;

public enum EnOperationType
{
    None,
    Compare,
}
public enum EnOperationCompareType
{
    Less,
    Equal,
    Greater,
}
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

public class AttackSelectItemInfoEditor : AttackSelectItemInfo
{
    public void GetStr(ref List<int> result)
    {
        var gIndex = result.Count;
        result.Add((int)operationType);
        result.Add(operationChildType);
        result.Insert(gIndex, result.Count - gIndex);

        result.Add(operationTypeParam.Length);
        result.AddRange(operationTypeParam);
    }
}
public class AttackSelectItemInfo : IClassPool<CommonSkillItemParamUserData>
{
    public EnOperationType operationType = EnOperationType.None;
    public int operationChildType;

    public int[] operationTypeParam = null;
    public void OnPoolDestroy()
    {

    }

    public void OnPoolInit(CommonSkillItemParamUserData userData)
    {
        var arrParams = userData.arrParams;
        var startIndex = userData.startIndex;
        var endIndex = startIndex + userData.paramCount;

        var gCount = startIndex >= endIndex ? default : arrParams[startIndex++];
        operationType = gCount < 1 ? default : (EnOperationType)arrParams[startIndex++];
        operationChildType = gCount < 2 ? default : arrParams[startIndex++];

        var operationParamsCount = startIndex >= endIndex ? default : arrParams[startIndex++];
        operationTypeParam = arrParams.Copy(startIndex, operationParamsCount);
        startIndex += operationParamsCount;

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

public class AttackSelectItemUserData : IClassPoolUserData
{
    public int[] arrParams = null;

    public void OnPoolDestroy()
    {
        arrParams = null;
    }
}

public class AttackSelectItemEditor : AttackSelectItem
{
    //public EntityPropertyInfoEditor m_PropertyInfoEditor = null;
    public List<AttackSelectItemInfoEditor> m_ItemInfoEditorList = new();

    public void InitEditor()
    {
        //foreach (var item in itemInfo??new AttackSelectItemInfo[0])
        //{
        //    m_ItemInfoEditorList.Add();
        //}
    }
    public void GetStr(ref List<int> result)
    {
        var gIndex = result.Count;
        result.Add((int)target);
        result.Add(itemInfo.Length);
        result.Insert(gIndex, result.Count - gIndex);

        //m_PropertyInfoEditor.GetStr(ref result);


        for (int i = 0; i < m_ItemInfoEditorList.Count; i++)
        {
            var index = result.Count;
            var item = m_ItemInfoEditorList[i];
            item.GetStr(ref result);
            result.Insert(index, result.Count - index);
        }
    }
}

public class EntityPropertyInfoUserData : IClassPoolUserData
{
    public EnEntityProperty target = EnEntityProperty.None;
    public int[] arrParams = null;
    public void OnPoolDestroy()
    {
        target = EnEntityProperty.None;
        arrParams = null;
    }
}

public abstract class EntityPropertyInfoEditor : EntityPropertyInfo
{
    public abstract void GetStr(ref List<int> result);
}
public class EntityPropertyGravityInfoEditor: EntityPropertyGravityInfo
{
    private List<int> m_Params = new();
    public void InitEditor()
    {
        m_Params = new(arrParams);
    }
    public void Draw()
    {

    }
    public void GetStr(ref List<int> result)
    {
        var index = result.Count;
        result.Add((int)target);

        result.Insert(index, result.Count - index);
        result.Add(m_Params.Count);
        result.AddRange(m_Params);
    }
}
public class EntityPropertyGravityInfo : EntityPropertyInfo
{
    public int[] arrParams = null;
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        arrParams = null;
    }
    public override void OnPoolInit(EntityPropertyInfoUserData userData)
    {
        base.OnPoolInit(userData);
        arrParams = userData.arrParams;
    }

}
public abstract class EntityPropertyInfo : IClassPool<EntityPropertyInfoUserData>
{
    public EnEntityProperty target = EnEntityProperty.None;

    public virtual void OnPoolDestroy()
    {
        target = EnEntityProperty.None;
    }

    public virtual void OnPoolInit(EntityPropertyInfoUserData userData)
    {
        target = userData.target;
    }

    public void PoolConstructor()
    {
    }

    public void PoolRelease()
    {
    }

    public void OnPoolEnable()
    {
    }
}
public class EntityPropertyMgr : Singleton<EntityPropertyMgr>
{
    public EntityPropertyInfo CreatePropertyInfo(EnEntityProperty target, int[] targetParams)
    {
        var propertyUserData = ClassPoolMgr.Instance.Pull<EntityPropertyInfoUserData>();
        propertyUserData.target = target;
        propertyUserData.arrParams = targetParams;
        EntityPropertyInfo result = target switch
        {
            EnEntityProperty.Gravity => ClassPoolMgr.Instance.Pull<EntityPropertyGravityInfo>(propertyUserData),
            _ => null,
        };
        ClassPoolMgr.Instance.Push(propertyUserData);
        return result;
    }
}
public class AttackSelectItem : ISkillData<AttackSelectItemUserData>
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
    public void OnPoolInit(AttackSelectItemUserData userData)
    {
        var arrParams = userData.arrParams;
        var startIndex = 0;
        var endIndex = arrParams.Length;

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

