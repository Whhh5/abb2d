
public interface ICompareInfo: IOperationInfo
{
    public EnOperationCompareType GetCompareType();
    public bool Compare(int target);
}