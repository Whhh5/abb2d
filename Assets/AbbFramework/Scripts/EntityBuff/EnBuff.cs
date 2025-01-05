
public enum EnBuffType
{
    None,
    Time, // 有时间
    Persistence, // 持续性
    Perpetual, // 永久的
}

public enum EnBuff
{
    None,
    [EditorFieldName("禁止移动")]
    NoMove,
    [EditorFieldName("移动修改")]
    MoveDown,
    [EditorFieldName("禁止跳跃")]
    NoJump,
    [EditorFieldName("禁止旋转")]
    NoRotation,
    [EditorFieldName("忽略质量")]
    NoGravity,
    EnumCount,
}