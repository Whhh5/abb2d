
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
    NoMovement,
    [EditorFieldName("移动修改")]
    MovingChanges,
    [EditorFieldName("禁止跳跃")]
    NoJumping,
    [EditorFieldName("禁止旋转")]
    NoRotation,
    [EditorFieldName("忽略质量")]
    NoGravity,
    [EditorFieldName("玩家增强buff")]
    PlayerBuff,
    [HideEditor]
    PlayerBuff_1,
    Poison,
    PoisonSub,
    [HideEditor]
    PlayerSkill2,
    EnumCount,
}