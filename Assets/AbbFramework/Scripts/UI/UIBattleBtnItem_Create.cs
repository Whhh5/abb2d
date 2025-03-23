using UnityEngine;
using UnityEngine.UI;

public class UIBattleBtnItem_CreateData : UIWindowItemData, IUIBattleBtnItem
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_UIBattleBtnItem_Create;
}
public class UIBattleBtnItem_Create : UIWindowItem
{
    [SerializeField, AutoReference()]
    private Button _ClickBtn = null;
    [SerializeField, AutoReference]
    private GameObject _SelectEffectObj = null;


    private bool _IsSelect = false;

    public override void OnUnload()
    {
        UIMgr.Instance.RemoveBtnListener(_ClickBtn, OnClick_ClickBtn);
        base.OnUnload();
    }
    public override void LoadCompeletion()
    {
        base.LoadCompeletion();

        UIMgr.Instance.AddBtnListener(_ClickBtn, OnClick_ClickBtn);
    }

    private void OnClick_ClickBtn()
    {
        _IsSelect = !_IsSelect;
        UpdateSelectState();
    }
    private void UpdateSelectState()
    {
        _SelectEffectObj.SetActive(_IsSelect);
    }

    private void OnEvent_SelectBattleBtn()
    {

    }
}
