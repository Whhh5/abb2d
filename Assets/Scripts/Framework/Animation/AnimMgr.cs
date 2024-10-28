using UnityEngine;
using System.Collections;
using Cysharp.Threading.Tasks;

public class AnimMgr : Singleton<AnimMgr>
{
	public AnimationClip GetClip(EnLoadTarget clipTarget)
	{
		var clip = LoadMgr.Instance.Load<AnimationClip>(clipTarget);

		return clip;
	}
	public void RecycleClip(EnLoadTarget clipTarget)
	{

	}
}

