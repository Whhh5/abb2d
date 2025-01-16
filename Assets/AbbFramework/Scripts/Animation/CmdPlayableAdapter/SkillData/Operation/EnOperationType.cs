public enum EnOperationType
{
    None,
    [EditorFieldName("算数")]
    Compare,
}
public enum EnOperationCompareType
{
    None,
    [EditorFieldName("<")]
    Less,
    [EditorFieldName("==")]
    Equal,
    [EditorFieldName(">")]
    Greater,
}