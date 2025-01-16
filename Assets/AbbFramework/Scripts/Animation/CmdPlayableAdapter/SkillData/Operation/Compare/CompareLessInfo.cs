public class CompareLessInfo : ICompareInfo
{
    public EnOperationCompareType GetCompareType() => EnOperationCompareType.Less;
    public int value;

    public bool Compare(int target)
    {
        return target < value;
    }

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