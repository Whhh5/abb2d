using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  static class SkillFactroyEditor
{

    public static ISkillTypeEditor GetSkillTypeEditor(EnSkillBoxType type)
    {
        return type switch
        {
            EnSkillBoxType.Link => new SkillTypeLinkDataEditor(),
            EnSkillBoxType.Random => new SkillTypeRandomDataEditor(),
            EnSkillBoxType.Singleton => new SkillTypeSingletonDataEditor(),
            EnSkillBoxType.Loop => new SkillTypeLoopDataEditor(),
            EnSkillBoxType.Select => new SkillTypeSelectDataEditor(),
            _ => null,
        };
    }

    public static Type GetOperationDataType(EnOperationType oprationType, params int[] childType)
    {
        return oprationType switch
        {
            EnOperationType.Compare => GetOperationComperaDataType((EnOperationCompareType)childType[0]),
            _ => null,
        };
    }

    private static Type GetOperationComperaDataType(EnOperationCompareType comperaType)
    {
        return comperaType switch
        {
            EnOperationCompareType.Less => typeof(CompareLessInfoEditor),
            EnOperationCompareType.Equal => typeof(CompareEqualInfoEditor),
            EnOperationCompareType.Greater => typeof(CompareGreaterInfoEditor),
            _ => null,
        };
    }

    public static IBuffDaraEditor GetBuffDataEditor(EnBuff buff)
    {
        IBuffDaraEditor buffData = buff switch
        {
            EnBuff.MovingChanges => new EntityMoveDownBuffDataEditor(),
            EnBuff.NoGravity => new EntityNoGravityBuffDataEditor(),
            EnBuff.NoJumping => new EntityNoJumpBuffDataEditor(),
            EnBuff.NoMovement => new EntityNoMoveBuffDataEditor(),
            EnBuff.NoRotation => new EntityNoRotationBuffDataEditor(),
            _ => null,
        };
        return buffData;
    }
}
