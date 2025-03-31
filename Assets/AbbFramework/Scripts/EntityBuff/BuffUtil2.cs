using UnityEngine;

static public partial class BuffUtil
{
    public static IEntityBuffParams ConvertBuffData(EnBuff buff, int[] arrParams)
    {
        var userData = ClassPoolMgr.Instance.Pull<CommonArrayUserData>();
        userData.value = arrParams;
        IEntityBuffParams result = buff switch
        {
            EnBuff.MovingChanges => ClassPoolMgr.Instance.Pull<EntityMoveDownBuffUserData>(userData),
            EnBuff.PlayerBuff => ClassPoolMgr.Instance.Pull<EntityBuffTimeDefaultInfo>(userData),
            EnBuff.PlayerBuff_1 => ClassPoolMgr.Instance.Pull<EntityBuffTimeDefaultInfo>(userData),
            EnBuff.Poison => ClassPoolMgr.Instance.Pull<EntityBuffTimeDefaultInfo>(userData),
            EnBuff.PoisonSub => ClassPoolMgr.Instance.Pull<EntityBuffTimeDefaultInfo>(userData),
            EnBuff.Expiosion => ClassPoolMgr.Instance.Pull<EntityBuffTimeDefaultInfo>(userData),
            EnBuff.Water => ClassPoolMgr.Instance.Pull<EntityBuffTimeDefaultInfo>(userData),
            EnBuff.WaterLoopHit => ClassPoolMgr.Instance.Pull<EntityBuffTimeDefaultInfo>(userData),
            EnBuff.Electrified => ClassPoolMgr.Instance.Pull<EntityBuffTimeDefaultInfo>(userData),
            EnBuff.ElectrifiedLoopHit => ClassPoolMgr.Instance.Pull<EntityBuffTimeDefaultInfo>(userData),
            EnBuff.AttackEffect => ClassPoolMgr.Instance.Pull<EntityBuffTimeDefaultInfo>(userData),
            EnBuff.AttackEffectLoop => ClassPoolMgr.Instance.Pull<EntityBuffTimeDefaultInfo>(userData),
            _ => null,
        };
        return result;
    }
    public static void PushConvertBuffData(IEntityBuffParams param)
    {
        if (param == null)
            return;
        ClassPoolMgr.Instance.Push(param);
    }
}
