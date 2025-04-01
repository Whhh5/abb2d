using UnityEngine;
public class EntityBattleElectrificationBuffData : EntityBuffData, IABBEventExecute, IUpdate
{
    private IEntityBuffParams _BuffParams = null;
    private readonly int[] _EffectEntityIDs = new int[2];
    public override void OnPoolDestroy()
    {
        UpdateMgr.Instance.Unregistener(this);
        for (int i = 0; i < _EffectEntityIDs.Length; i++)
            EffectMgr.Instance.DestroyEffect(_EffectEntityIDs[i]);

        BuffUtil.PushConvertBuffData(_BuffParams);
        ABBEventMgr.Instance.Unregister(EnABBEvent.EVENT_BATTLE_INFO, (int)EnAttackEventSourceType.Other, _TargetEntityID, this);
        base.OnPoolDestroy();
        _BuffParams = null;
    }
    public override void OnEnable(int addKey, IEntityBuffParams buffParams)
    {
        base.OnEnable(addKey, buffParams);

        _BuffParams = BuffUtil.ConvertBuffData(EnBuff.BattleElectrificationTrace, new int[] { 1, 100 });

        ABBEventMgr.Instance.Register(EnABBEvent.EVENT_BATTLE_INFO, (int)EnAttackEventSourceType.Other, _TargetEntityID, this);
        UpdateMgr.Instance.Registener(this);


        _EffectEntityIDs[0] = EffectMgr.Instance.PlayEffect(29);
        _EffectEntityIDs[1] = EffectMgr.Instance.PlayEffect(30);
    }

    #region Event
    public void EventExecute(EnABBEvent enEvent, int sourceID, int type, IClassPool userData)
    {
        switch (enEvent)
        {
            case EnABBEvent.EVENT_BATTLE_INFO:
                EVENT_BATTLE_INFO(userData as EventBattleInfo);
                break;
            default:
                break;
        }
    }
    private void EVENT_BATTLE_INFO(EventBattleInfo userData)
    {
        BuffMgr.Instance.AddEntityBuff(_TargetEntityID, userData.entityID2, EnBuff.BattleElectrificationTrace, _BuffParams);
    }
    #endregion

    public void Update()
    {
        var pos = Entity3DMgr.Instance.GetEntityWorldPos(_TargetEntityID);
        var entityData = EntityMgr.Instance.GetEntityData(_EffectEntityIDs[0]);
        entityData.SetPosition(pos);

        var weaponPos = Entity3DMgr.Instance.GetEntityWeaponPos1(_TargetEntityID);
        var entityData2 = EntityMgr.Instance.GetEntityData(_EffectEntityIDs[1]);
        entityData2.SetPosition(weaponPos);
    }
}
