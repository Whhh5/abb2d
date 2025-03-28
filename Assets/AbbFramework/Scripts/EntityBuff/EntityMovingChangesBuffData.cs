using System.Collections.Generic;
using UnityEngine;
public class EntityMovingChangesBuffData : EntityBuffData<EntityMoveDownBuffUserData>
{
    private readonly Dictionary<int, float> _Key2Value = new();

    public override bool OnDisable(int addKey)
    {
        if (!base.OnDisable(addKey))
            return false;

        Entity3DMgr.Instance.SetEntityMoveSpeedIncrements(_TargetEntityID, -_Key2Value[addKey]);
        _Key2Value.Remove(addKey);
        return true;
    }

    protected override void OnEnable(int addKey, EntityMoveDownBuffUserData buffParams)
    {
        base.OnEnable(addKey, buffParams);
        SetValue(ref addKey, ref buffParams.value);
    }
    protected override void ReOnEnable(int addKey, EntityMoveDownBuffUserData buffParams)
    {
        base.ReOnEnable(addKey, buffParams);
        SetValue(ref addKey, ref buffParams.value);
    }

    private void SetValue(ref int addKey, ref float value)
    {
        _Key2Value.Add(addKey, value);
        Entity3DMgr.Instance.SetEntityMoveSpeedIncrements(_TargetEntityID, value);
    }
}
