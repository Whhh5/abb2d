using UnityEngine;



public class BuffMgr : Singleton<BuffMgr>
{
    public EnBuffType GetBuffType(EnBuff buff)
    {
        switch (buff)
        {
            case EnBuff.NoJump:
                return EnBuffType.Persistence;
            case EnBuff.NoMove:
                return EnBuffType.Persistence;
            case EnBuff.MoveDown:
                return EnBuffType.Persistence;
            case EnBuff.NoRotation:
                return EnBuffType.Persistence;
            default:
                return EnBuffType.Persistence;
        }
        return EnBuffType.None;
    }
    public EntityBuffData CreateBuffData(EnBuff buff, int entityID)
    {
        EntityBuffData buffData = null;
        var data = new EntityBuffDataUserData()
        {
            entityID = entityID,
            buff = buff,
        };

        switch (buff)
        {
            case EnBuff.NoMove:
                buffData = ClassPoolMgr.Instance.Pull<EntityNoMoveBuffData, EntityBuffDataUserData>(ref data);
                break;
            case EnBuff.NoJump:
                buffData = ClassPoolMgr.Instance.Pull<EntityNoJumpBuffData, EntityBuffDataUserData>(ref data);
                break;
            case EnBuff.MoveDown:
                buffData = ClassPoolMgr.Instance.Pull<EntityMoveDownBuffData, EntityBuffDataUserData>(ref data);
                break;
            case EnBuff.NoRotation:
                buffData = ClassPoolMgr.Instance.Pull<EntityNoRotationBuffData, EntityBuffDataUserData>(ref data);
                break;
            case EnBuff.NoGravity:
                buffData = ClassPoolMgr.Instance.Pull<EntityNoGravityBuffData, EntityBuffDataUserData>(ref data);
                break;
            default:
                break;
        }
        return buffData;
    }
    public void DestroyBuffData(EntityBuffData buffData)
    {

    }
    public IEntityBuffParams GetBuffData(EnBuff buff, int[] arrParams)
    {
        switch (buff)
        {
            case EnBuff.MoveDown:
                {
                    var data = new EntityMoveDownBuffParamsUserData()
                    {
                        value = arrParams[0] / 100f,
                    };
                    var param = ClassPoolMgr.Instance.Pull<EntityMoveDownBuffParams, EntityMoveDownBuffParamsUserData>(ref data);
                    return param;
                }
            default:
                return null;
        }
    }
}
