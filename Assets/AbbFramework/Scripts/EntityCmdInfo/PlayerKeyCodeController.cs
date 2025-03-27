using UnityEngine;


//public class PlayerKeyCodeController : EntityKeyCodeController
//{
//    public override void OnDisable()
//    {
//        ABBInputMgr.Instance.RemoveListanerDown(KeyCode.K, OnClick_KeyCodeDownK);
//        ABBInputMgr.Instance.RemoveListanerDown(KeyCode.J, OnClick_KeyCodeDownJ);
//        ABBInputMgr.Instance.RemoveListanerDown(KeyCode.U, OnClick_KeyCodeDownU);
//        ABBInputMgr.Instance.RemoveListanerDown(KeyCode.L, OnClick_KeyCodeDownL);
//        ABBInputMgr.Instance.RemoveListanerDown(KeyCode.F, OnClick_KeyCodeDownF);
//        ABBInputMgr.Instance.RemoveListanerDown(KeyCode.I, OnClick_KeyCodeDownI);

//        ABBInputMgr.Instance.RemoveListaner(KeyCode.O, OnClick_KeyCodeO);
//        base.OnDisable();
//    }
//    public override void OnEnable(int entityID)
//    {
//        base.OnEnable(entityID);

//        ABBInputMgr.Instance.AddListanerDown(KeyCode.K, OnClick_KeyCodeDownK);
//        ABBInputMgr.Instance.AddListanerDown(KeyCode.J, OnClick_KeyCodeDownJ);
//        ABBInputMgr.Instance.AddListanerDown(KeyCode.U, OnClick_KeyCodeDownU);
//        ABBInputMgr.Instance.AddListanerDown(KeyCode.L, OnClick_KeyCodeDownL);
//        ABBInputMgr.Instance.AddListanerDown(KeyCode.F, OnClick_KeyCodeDownF);
//        ABBInputMgr.Instance.AddListanerDown(KeyCode.I, OnClick_KeyCodeDownI);

//        ABBInputMgr.Instance.AddListaner(KeyCode.O, OnClick_KeyCodeO);
//    }
//    private void OnClick_KeyCodeDownJ()
//    {
//        if (!EntityUtil.IsValid(_EntityID))
//            return;
//        Entity3DMgr.Instance.AddEntityCmd(_EntityID, EnEntityCmd.Attack);
//    }
//    private void OnClick_KeyCodeDownK()
//    {
//        if (!EntityUtil.IsValid(_EntityID))
//            return;
//        Entity3DMgr.Instance.ExecuteEntityJump(_EntityID);
//    }


//    private void OnClick_KeyCodeDownU()
//    {
//        if (!EntityUtil.IsValid(_EntityID))
//            return;
//        Entity3DMgr.Instance.AddEntityCmd(_EntityID, EnEntityCmd.Skill1);
//    }

//    private void OnClick_KeyCodeDownL()
//    {
//        if (!EntityUtil.IsValid(_EntityID))
//            return;
//        Entity3DMgr.Instance.AddEntityCmd(_EntityID, EnEntityCmd.Teleport);
//    }

//    private void OnClick_KeyCodeDownF()
//    {
//        if (!EntityUtil.IsValid(_EntityID))
//            return;
//        //Entity3DMgr.Instance.AddEntityCmd(_EntityID, EnEntityCmd.LayerMixer);
//    }
//    private void OnClick_KeyCodeDownI()
//    {
//        if (!EntityUtil.IsValid(_EntityID))
//            return;
//        Entity3DMgr.Instance.AddEntityCmd(_EntityID, EnEntityCmd.PlayerBuff);
//    }


//    private void OnClick_KeyCodeO()
//    {
//        if (!EntityUtil.IsValid(_EntityID))
//            return;
//        Entity3DMgr.Instance.AddEntityCmd(_EntityID, EnEntityCmd.Skill2);
//    }
//}
