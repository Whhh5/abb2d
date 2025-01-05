using UnityEngine;
using System.Collections.Generic;

public enum EnAnimClip
{
    None,
    Battle_idle,

}
public class AnimMgr : Singleton<AnimMgr>
{
    public override EnManagerFuncType FuncType => base.FuncType;
    public AnimationClip GetClip(int clipID)
	{
        var clipCfg = GameSchedule.Instance.GetClipCfg0(clipID);
		var clip = ABBLoadMgr.Instance.Load<AnimationClip>(clipCfg.nAssetID);
		return clip;
	}
	public void RecycleClip(int clipID)
	{

	}
    public AvatarMask GetLayerAvatar(EnAnimLayer layerAvatarID)
    {
        var avatarCfg = GameSchedule.Instance.GetAnimLayerCfg0((int)layerAvatarID);
        var mask = ABBLoadMgr.Instance.Load<AvatarMask>(avatarCfg.nAssetID);
        return mask;
    }
    public EnEntityCmdLevel GetCmdLevel(EnEntityCmd cmd)
    {
        var cmdCfg = GameSchedule.Instance.GetCmdCfg0((int)cmd);
        return (EnEntityCmdLevel)cmdCfg.nLevel;
    }
	public EnAnimLayer GetAnimLayer(int clipID)
	{
        var clipCfg = GameSchedule.Instance.GetClipCfg0(clipID);
		return (EnAnimLayer)clipCfg.nLayer;
	}
	public bool GetAnimIsLoop(int clipID)
    {
        var clipCfg = GameSchedule.Instance.GetClipCfg0(clipID);
        return clipCfg.bIsLoop > 0;
    }
	public List<int> GetIdleAnimClipList(int m_RoleID)
	{
		return new()
		{
            (int)EnLoadTarget.Anim_Battle_idle,
            (int)EnLoadTarget.Anim_Rest_idle,
            (int)EnLoadTarget.Anim_Rest_relax_01,
            (int)EnLoadTarget.Anim_Rest_relax_02,
            //EnLoadTarget.Anim_Rest_relax_03,
        };
    }
    public List<int> GetAttackAnimClipList(int m_RoleID)
    {
        return new()
        {
            (int)EnLoadTarget.Anim_Attack_01,
            (int)EnLoadTarget.Anim_Attack_02,
            (int)EnLoadTarget.Anim_Attack_03,
            (int)EnLoadTarget.Anim_Attack_04,
            (int)EnLoadTarget.Anim_Attack_05,
            (int)EnLoadTarget.Anim_Attack_06,
        };
    }
    public List<int> GetRunAnimClipList(int m_RoleID)
    {
        return new()
        {
            (int)EnLoadTarget.Anim_Battle_run,
        };
    }
    public List<int> GetJumpAnimClipList(int m_RoleID)
    {
        return new()
        {
            (int)EnLoadTarget.Anim_Jump_inplace,
            (int)EnLoadTarget.Anim_Jump_fall_loop,
            (int)EnLoadTarget.Anim_Jump_fall_end_H_Battle,
        };
    }
    public List<int> GetJumpDownAnimClipList(int m_RoleID)
    {
        return new()
        {
            (int)EnLoadTarget.Anim_Jump_fall_end_L_Battle,
            (int)EnLoadTarget.Anim_Jump_fall_end_H_Battle,
        };
    }
    public List<int> GetSkill2AnimClipList(int roleID)
    {
        return new()
        {
            (int)EnLoadTarget.Anim_Magic_04_4,
            (int)EnLoadTarget.Anim_Magic_04_5_Loop,
            (int)EnLoadTarget.Anim_Magic_04_6,
        };
    }
    public List<int> GetSkill1AnimClipList(int roleID)
    {
        return new()
        {
            (int)EnLoadTarget.Anim_Magic_06_1,
            (int)EnLoadTarget.Anim_Magic_06_2,
            (int)EnLoadTarget.Anim_Magic_06_3,
            (int)EnLoadTarget.Anim_Magic_06_4,
            (int)EnLoadTarget.Anim_Magic_06_5,
            //EnLoadTarget.Anim_Magic_06_6,
        };
    }
    public List<int> GetTeleportAnimClipList(int roleID)
    {
        return new()
        {
            (int)EnLoadTarget.Anim_Accelerate,
        };
    }
    public List<int> GetInjuredAnimClipList(int roleID)
    {
        return new()
        {
            (int)EnLoadTarget.Anim_Hit_light,
        };
    }
}

