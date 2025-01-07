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
        var data = GameClassPoolMgr.Instance.Pull<EntityBuffDataUserData>();
        data.entityID = entityID;
        data.buff = buff;

        switch (buff)
        {
            case EnBuff.NoMove:
                buffData = GameClassPoolMgr.Instance.Pull<EntityNoMoveBuffData>(data);
                break;
            case EnBuff.NoJump:
                buffData = GameClassPoolMgr.Instance.Pull<EntityNoJumpBuffData>(data);
                break;
            case EnBuff.MoveDown:
                buffData = GameClassPoolMgr.Instance.Pull<EntityMoveDownBuffData>(data);
                break;
            case EnBuff.NoRotation:
                buffData = GameClassPoolMgr.Instance.Pull<EntityNoRotationBuffData>(data);
                break;
            case EnBuff.NoGravity:
                buffData = GameClassPoolMgr.Instance.Pull<EntityNoGravityBuffData>(data);
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
        IEntityBuffParams data = buff switch
        {
            EnBuff.MoveDown => new EntityMoveDownBuffParams() { value = arrParams[0] / 100f },
            _ => null,
        };
        return data;
    }       
}
