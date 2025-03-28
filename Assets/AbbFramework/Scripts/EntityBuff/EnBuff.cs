public enum EnBuff
{
	None = 0,
	[EditorFieldNameAttribute("禁止移动")]
	NoMovement = 1,
	[EditorFieldNameAttribute("移动修改")]
	MovingChanges = 2,
	[EditorFieldNameAttribute("禁止跳跃")]
	NoJumping = 3,
	[EditorFieldNameAttribute("禁止旋转")]
	NoRotation = 4,
	[EditorFieldNameAttribute("忽略质量")]
	NoGravity = 5,
	[EditorFieldNameAttribute("玩家增强buff")]
	PlayerBuff = 6,
	[EditorFieldNameAttribute("玩家增强buff2")]
	PlayerBuff_1 = 7,
	[EditorFieldNameAttribute("毒气")]
	Poison = 8,
	[EditorFieldNameAttribute("中毒")]
	PoisonSub = 9,
	[EditorFieldNameAttribute("旋转技能")]
	PlayerSkill2 = 10,
	[EditorFieldNameAttribute("持续爆炸")]
	Expiosion = 11,
	[EditorFieldNameAttribute("大型爆炸")]
	Expiosion2 = 12,
}
