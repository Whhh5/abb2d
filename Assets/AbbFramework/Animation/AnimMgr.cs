using UnityEngine;
using System.Collections;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class AnimMgr : Singleton<AnimMgr>
{
    public override EnManagerFuncType FuncType => base.FuncType;
    public AnimationClip GetClip(EnLoadTarget clipTarget)
	{
		var clip = ABBLoadMgr.Instance.Load<AnimationClip>(clipTarget);

		return clip;
	}
	public void RecycleClip(EnLoadTarget clipTarget)
	{

	}

	public EnAnimLayer GetAnimLayer(EnLoadTarget clip)
	{
		if (clip == EnLoadTarget.Anim_Battle_idle)
		{
			return EnAnimLayer.Base;
		}
		return EnAnimLayer.Layer1;
	}
	public bool GetAnimIsLoop(EnLoadTarget clip)
    {
        if (clip == EnLoadTarget.Anim_Battle_idle)
        {
			return true;
        }
        return false;
    }
}

