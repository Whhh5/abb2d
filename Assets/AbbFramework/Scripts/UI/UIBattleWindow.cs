using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;

public interface IUIBattleBtnItem
{

}
public class UIBattleWindowData : UIWindowData
{
    public override EnUIWindowType WindowType => EnUIWindowType.Window;

    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_UIBattleWindow;
}
public class UIBattleWindow : UIWindow, IABBEventExecute
{

    [SerializeField, AutoReference]
    private RectTransform _HealthRootRect = null;


    private Dictionary<int, int> _EntityID2DataIndex = new();
    private List<int> _HealthData = new();

    public override void OnHide()
    {
        ClearHealthInfo();
        ABBEventMgr.Instance.Unregister(EnABBEvent.EVENT_BATTLE_INFO, 0, 0, this);
    }

    public override void OnShow()
    {
        ABBEventMgr.Instance.Register(EnABBEvent.EVENT_BATTLE_INFO, 0, 0, this);
    }

    public void EventExecute(EnABBEvent enEvent, int sourceID, int type, IClassPool userData)
    {
        switch (enEvent)
        {
            case EnABBEvent.EVENT_BATTLE_INFO:
                {
                    var info = userData as EventBattleInfo;
                    if(_EntityID2DataIndex.TryGetValue(info.entityID2, out var index))
                    {
                        var entityData = EntityMgr.Instance.GetEntityData<UIEntityHealthInfoData>(_HealthData[index]);
                        entityData.UpdateValue(info.toValue);
                    }
                    else
                    {
                        var data = ClassPoolMgr.Instance.Pull<UIEntityHealthInfoDataUserData>();
                        data.entityID = info.entityID2;
                        data.fromValue = info.fromValue;
                        data.toValue = info.toValue;
                        data.time = 5;
                        var itemEntityID = UIMgr.Instance.CreateWindowItem<UIEntityHealthInfoData>(_HealthRootRect, data);
                        _EntityID2DataIndex.Add(info.entityID2, _HealthData.Count);
                        _HealthData.Add(itemEntityID);
                        ClassPoolMgr.Instance.Push(data);
                    }
                }
                break;
            default:
                break;
        }
    }

    private void ClearHealthInfo()
    {
        for (int i = 0; i < _HealthData.Count; i++)
        {
            UIMgr.Instance.DestroyWindowItem(_HealthData[i]);
        }

        _EntityID2DataIndex.Clear();
        _HealthData.Clear();
    }
    protected override void Update()
    {
        base.Update();

        if (_HealthData.Count > 0)
        {
            var entityData = EntityMgr.Instance.GetEntityData<UIEntityHealthInfoData>(_HealthData[^1]);
            if (entityData.IsEnd())
            {
                ClearHealthInfo();
            }
        }
    }
}
