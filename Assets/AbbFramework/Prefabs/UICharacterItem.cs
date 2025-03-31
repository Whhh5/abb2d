using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UICharacterItemData : UIWindowItemData
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_UICharacterItem;

    private UICharacterItem _UICharacterItem = null;
    public int CharacterID { get; private set; }

    public bool SelectStatus { get; private set; }
    public UnityAction<int> _ClickEvent = null;
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        CharacterID = -1;
    }
    public override void OnGODestroy()
    {
        base.OnGODestroy();
        _UICharacterItem = null;
    }
    public override void OnGOCreate()
    {
        base.OnGOCreate();
        _UICharacterItem = m_Entity as UICharacterItem;
    }
    public void SetCharacterID(int id)
    {
        CharacterID = id;
        if (m_IsLoadSuccess)
            _UICharacterItem.SetCharacterID();
    }
    public void SetSelectStatus(bool selectStatus)
    {
        SelectStatus = selectStatus;
        if (m_IsLoadSuccess)
            _UICharacterItem.SetSelectStatus();
    }
    public void SetClickEvent(UnityAction<int> ClickEvent)
    {
        _ClickEvent = ClickEvent;
    }
    public void OnClick()
    {
        _ClickEvent.Invoke(CharacterID);
    }
}
public class UICharacterItem : UIWindowItem
{
    [SerializeField, AutoReference]
    private TextMeshProUGUI _NameTxt = null;
    [SerializeField, AutoReference]
    private Image _IconImg = null;
    [SerializeField, AutoReference]
    private GameObject _SelectStatusObj = null;
    [SerializeField, AutoReference]
    private Button _ClickBtn = null;

    private UICharacterItemData _UICharacterItemData = null;

    public override void OnUnload()
    {
        UIMgr.Instance.RemoveBtnListener(_ClickBtn, _UICharacterItemData.OnClick);
        base.OnUnload();
        _UICharacterItemData = null;
    }
    public override void LoadCompeletion()
    {
        base.LoadCompeletion();
        _UICharacterItemData = m_EntityData as UICharacterItemData;
        UIMgr.Instance.AddBtnListener(_ClickBtn, _UICharacterItemData.OnClick);

        SetCharacterID();
        SetSelectStatus();
    }

    public void SetCharacterID()
    {
        var characterCfg = GameSchedule.Instance.GetCharacterCfg0(_UICharacterItemData.CharacterID);
        var monsterCfg = GameSchedule.Instance.GetMonsterCfg0(characterCfg.nMonsterID);
        _NameTxt.text = monsterCfg.strName;
    }

    public void SetSelectStatus()
    {
        _SelectStatusObj.SetActive(_UICharacterItemData.SelectStatus);
    }
}
