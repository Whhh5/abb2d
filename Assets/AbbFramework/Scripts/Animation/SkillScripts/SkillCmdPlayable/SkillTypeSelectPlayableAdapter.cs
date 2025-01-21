using System.Collections.Generic;
using UnityEngine;

public class SkillTypeSelectPlayableAdapter : SkillTypePlayableAdapter
{
    private PlayableClipAdapter _Clipadapter = null;


    private SkillTypeSelectData _SelectItem = null;
    private SkillItemInfo _AtkLinkItemData = null;
    protected override void OnDestroy()
    {
        PlayableAdapter.Destroy(_Clipadapter);
        ClassPoolMgr.Instance.Push(_SelectItem);
        base.OnDestroy();
        _Clipadapter = null;
    }

    public override void OnPoolInit(PlayableAdapterUserData userData)
    {
        base.OnPoolInit(userData);

        var data = userData.customData as SkillTypeLinkPlayableAdapterCustomData;

        var itemUserData = ClassPoolMgr.Instance.Pull<AttackLinkSkillDataUserData>();
        itemUserData.arrParams = data.arrParams;
        _SelectItem = ClassPoolMgr.Instance.Pull<SkillTypeSelectData>(itemUserData);
        ClassPoolMgr.Instance.Push(itemUserData);


        var velocity = Entity3DMgr.Instance.GetEntityVerticalVelocity(m_Graph);

        var value = Mathf.RoundToInt(velocity);
        _AtkLinkItemData = _SelectItem.CompareResult(value);

        var cipID = _AtkLinkItemData.GetClipID();
        _Clipadapter = m_Graph.CreateClipPlayableAdapter(cipID);
        AddConnectRootAdapter(_Clipadapter);
    }
    public override void ExecuteCmd()
    {
        base.ExecuteCmd();
        _AtkLinkItemData.OnEnable(m_Graph);
    }
    public override void RemoveCmd()
    {
        _AtkLinkItemData.OnDisable(m_Graph);
        base.RemoveCmd();
    }
    public override void ReExecuteCmd()
    {
        base.ReExecuteCmd();
    }
    public override bool NextAnimLevelComdition()
    {
        var curSchedule = GetPlaySchedule01();
        if (!_AtkLinkItemData.IsCanNextAction(curSchedule))
            return false;
        return true;
    }

    public override float GetUnitTime()
    {
        return _Clipadapter.GetUnitTime();
    }
    public override float GetPlayTime()
    {
        return _Clipadapter.GetPlayTime();
    }
}
