




public class CompareLessInfo : CompareInfo
{
    public override EnOperationCompareType GetCompareType() => EnOperationCompareType.Less;

    public override bool CompareResult(int target)
    {
        return target < value;
    }
}
public class CompareEqualInfo : CompareInfo
{
    public override EnOperationCompareType GetCompareType() => EnOperationCompareType.Equal;

    public override bool CompareResult(int target)
    {
        return target == value;
    }
}
public class CompareGreaterInfo : CompareInfo
{
    public override EnOperationCompareType GetCompareType() => EnOperationCompareType.Greater;

    public override bool CompareResult(int target)
    {
        return target > value;
    }
}



public abstract class CompareInfo : ICompareInfo
{
    public abstract EnOperationCompareType GetCompareType();
    public int value;

    public abstract bool CompareResult(int target);

    public void OnPoolDestroy()
    {
        value = -1;
    }

    public void OnPoolEnable()
    {
    }

    public void OnPoolInit(CommonSkillItemParamUserData userData)
    {
        var startIndex = userData.startIndex;
        var endIndex = startIndex + userData.paramCount;
        var arrParams = userData.arrParams;
        var headCount = startIndex >= endIndex ? default : arrParams[startIndex++];
        value = headCount < 1 ? default : arrParams[startIndex++];
    }

    public void PoolConstructor()
    {
    }

    public void PoolRelease()
    {
    }

    public int[] GetChildType()
    {
        return new int[] { (int)GetCompareType() };
    }
}