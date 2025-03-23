using UnityEngine;

public class Monster0KeyCodeController : EntityKeyCodeController
{
    public override void OnDisable()
    {
        ABBInputMgr.Instance.RemoveListanerDown(KeyCode.K, OnClick_KeyCodeDownK);
        ABBInputMgr.Instance.RemoveListanerDown(KeyCode.J, OnClick_KeyCodeDownJ);
        ABBInputMgr.Instance.RemoveListanerDown(KeyCode.U, OnClick_KeyCodeDownU);
        ABBInputMgr.Instance.RemoveListaner(KeyCode.I, OnClick_KeyCodeI);
        ABBInputMgr.Instance.RemoveListanerDown(KeyCode.L, OnClick_KeyCodeDownL);
        base.OnDisable();
    }
    public override void OnEnable(int entityID)
    {
        base.OnEnable(entityID);

        ABBInputMgr.Instance.AddListanerDown(KeyCode.K, OnClick_KeyCodeDownK);
        ABBInputMgr.Instance.AddListanerDown(KeyCode.J, OnClick_KeyCodeDownJ);
        ABBInputMgr.Instance.AddListanerDown(KeyCode.U, OnClick_KeyCodeDownU);
        ABBInputMgr.Instance.AddListaner(KeyCode.I, OnClick_KeyCodeI);
        ABBInputMgr.Instance.AddListanerDown(KeyCode.L, OnClick_KeyCodeDownL);
    }
    private void OnClick_KeyCodeDownJ()
    {
        if (!EntityUtil.IsValid(_EntityID))
            return;
        Entity3DMgr.Instance.AddEntityCmd(_EntityID, EnEntityCmd.Monster0Skill1);
    }
    private void OnClick_KeyCodeDownK()
    {
        if (!EntityUtil.IsValid(_EntityID))
            return;
        Entity3DMgr.Instance.ExecuteEntityJump(_EntityID);
    }


    private void OnClick_KeyCodeDownU()
    {
        if (!EntityUtil.IsValid(_EntityID))
            return;
        Entity3DMgr.Instance.AddEntityCmd(_EntityID, EnEntityCmd.Monster0Skill2);
    }
    private void OnClick_KeyCodeI()
    {
        if (!EntityUtil.IsValid(_EntityID))
            return;
        Entity3DMgr.Instance.AddEntityCmd(_EntityID, EnEntityCmd.Monster0Buff1);
    }

    private void OnClick_KeyCodeDownL()
    {
        //if (!EntityUtil.IsValid(_EntityID))
        //    return;
        //Entity3DMgr.Instance.AddEntityCmd(_EntityID, EnEntityCmd.Teleport);
    }
}