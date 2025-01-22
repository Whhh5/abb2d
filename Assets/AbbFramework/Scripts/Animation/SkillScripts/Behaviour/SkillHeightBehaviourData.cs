using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillHeightBehaviourData : SkillBehaviour
{
    private float _Height = -1;
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        _Height = -1;
    }
    public override EnSkillBehaviourType SkillBehavioueType() => EnSkillBehaviourType.Height;

    public override void Execute(int entityID)
    {
        Entity3DMgr.Instance.SetEntityHeight(entityID, _Height, 1);
    }

    public override void Init(ref int[] arrValue)
    {
        _Height = arrValue?.Length > 0 ? arrValue[0] / 100f : 0;
    }
}
