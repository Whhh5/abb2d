using UnityEngine;

//public class Monster0KeyCodeController : EntityKeyCodeController
//{
//    public override void OnDisable()
//    {
//        ABBInputMgr.Instance.RemoveListanerDown(KeyCode.J, OnClick_KeyCodeDownJ);
//        ABBInputMgr.Instance.RemoveListanerDown(KeyCode.U, OnClick_KeyCodeDownU);
//        ABBInputMgr.Instance.RemoveListaner(KeyCode.I, OnClick_KeyCodeI);
//        base.OnDisable();
//    }
//    public override void OnEnable(int entityID)
//    {
//        base.OnEnable(entityID);

//        ABBInputMgr.Instance.AddListanerDown(KeyCode.J, OnClick_KeyCodeDownJ);
//        ABBInputMgr.Instance.AddListanerDown(KeyCode.U, OnClick_KeyCodeDownU);
//        ABBInputMgr.Instance.AddListaner(KeyCode.I, OnClick_KeyCodeI);
//    }
//    private void OnClick_KeyCodeDownJ()
//    {
//        if (!EntityUtil.IsValid(_EntityID))
//            return;
//        Entity3DMgr.Instance.AddEntityCmd(_EntityID, EnEntityCmd.Monster0Skill1);
//    }

//    private void OnClick_KeyCodeDownU()
//    {
//        if (!EntityUtil.IsValid(_EntityID))
//            return;
//        Entity3DMgr.Instance.AddEntityCmd(_EntityID, EnEntityCmd.Monster0Skill2);
//    }
//    private void OnClick_KeyCodeI()
//    {
//        if (!EntityUtil.IsValid(_EntityID))
//            return;
//        Entity3DMgr.Instance.AddEntityCmd(_EntityID, EnEntityCmd.Monster0Buff1);
//    }


//}