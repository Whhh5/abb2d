using UnityEngine;



public class BuffMgr : Singleton<BuffMgr>
{
    public EnBuffType GetBuffType(EnBuff buff)
    {
        switch (buff)
        {
            case EnBuff.NoJumping:
                return EnBuffType.Persistence;
            case EnBuff.NoMovement:
                return EnBuffType.Persistence;
            case EnBuff.MovingChanges:
                return EnBuffType.Persistence;
            case EnBuff.NoRotation:
                return EnBuffType.Persistence;
            default:
                return EnBuffType.Persistence;
        }
        //return EnBuffType.None;
    }
    public EntityBuffData CreateBuffData(EnBuff buff, int entityID)
    {
        EntityBuffData buffData = null;
        var data = ClassPoolMgr.Instance.Pull<EntityBuffDataUserData>();
        data.entityID = entityID;
        data.buff = buff;

        switch (buff)
        {
            case EnBuff.NoMovement:
                buffData = ClassPoolMgr.Instance.Pull<EntityNoMoveBuffData>(data);
                break;
            case EnBuff.NoJumping:
                buffData = ClassPoolMgr.Instance.Pull<EntityNoJumpBuffData>(data);
                break;
            case EnBuff.MovingChanges:
                buffData = ClassPoolMgr.Instance.Pull<EntityMoveDownBuffData>(data);
                break;
            case EnBuff.NoRotation:
                buffData = ClassPoolMgr.Instance.Pull<EntityNoRotationBuffData>(data);
                break;
            case EnBuff.NoGravity:
                buffData = ClassPoolMgr.Instance.Pull<EntityNoGravityBuffData>(data);
                break;
            default:
                break;
        }
        ClassPoolMgr.Instance.Push(data);
        return buffData;
    }
    public void DestroyBuffData(EntityBuffData buffData)
    {

    }
    public IEntityBuffParams GetBuffData(EnBuff buff, int[] arrParams)
    {
        switch (buff)
        {
            case EnBuff.MovingChanges:
                {
                    var data = ClassPoolMgr.Instance.Pull<EntityMoveDownBuffParamsUserData>();
                    data.value = arrParams[0] / 100f;
                    var param = ClassPoolMgr.Instance.Pull<EntityMoveDownBuffParams>(data);
                    ClassPoolMgr.Instance.Push(data);
                    return param;
                }
            default:
                return null;
        }
    }
}
