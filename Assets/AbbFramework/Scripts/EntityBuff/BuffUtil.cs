public partial class BuffUtil
{
	public static IEntityBuffData CreateBuffData(EnBuff buff, IClassPoolUserData data)
	{
		return buff switch
		{
			EnBuff.NoMovement => ClassPoolMgr.Instance.Pull<EntityNoMovementBuffData>(data),
			EnBuff.MovingChanges => ClassPoolMgr.Instance.Pull<EntityMovingChangesBuffData>(data),
			EnBuff.NoJumping => ClassPoolMgr.Instance.Pull<EntityNoJumpingBuffData>(data),
			EnBuff.NoRotation => ClassPoolMgr.Instance.Pull<EntityNoRotationBuffData>(data),
			EnBuff.NoGravity => ClassPoolMgr.Instance.Pull<EntityNoGravityBuffData>(data),
			EnBuff.PlayerBuff => ClassPoolMgr.Instance.Pull<EntityPlayerBuffBuffData>(data),
			EnBuff.PlayerBuff_1 => ClassPoolMgr.Instance.Pull<EntityPlayerBuff_1BuffData>(data),
			EnBuff.Poison => ClassPoolMgr.Instance.Pull<EntityPoisonBuffData>(data),
			EnBuff.PoisonSub => ClassPoolMgr.Instance.Pull<EntityPoisonSubBuffData>(data),
			EnBuff.PlayerSkill2 => ClassPoolMgr.Instance.Pull<EntityPlayerSkill2BuffData>(data),
			EnBuff.Expiosion => ClassPoolMgr.Instance.Pull<EntityExpiosionBuffData>(data),
			EnBuff.Expiosion2 => ClassPoolMgr.Instance.Pull<EntityExpiosion2BuffData>(data),
			EnBuff.AttackEffect => ClassPoolMgr.Instance.Pull<EntityAttackEffectBuffData>(data),
			EnBuff.Water => ClassPoolMgr.Instance.Pull<EntityWaterBuffData>(data),
			EnBuff.Electrified => ClassPoolMgr.Instance.Pull<EntityElectrifiedBuffData>(data),
			EnBuff.WaterLoopHit => ClassPoolMgr.Instance.Pull<EntityWaterLoopHitBuffData>(data),
			EnBuff.ElectrifiedLoopHit => ClassPoolMgr.Instance.Pull<EntityElectrifiedLoopHitBuffData>(data),
			EnBuff.AttackEffectLoop => ClassPoolMgr.Instance.Pull<EntityAttackEffectLoopBuffData>(data),
			EnBuff.BattleElectrification => ClassPoolMgr.Instance.Pull<EntityBattleElectrificationBuffData>(data),
			EnBuff.BattleElectrificationTrace => ClassPoolMgr.Instance.Pull<EntityBattleElectrificationTraceBuffData>(data),
			_ => default,
		};
	}
}
