using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStartGameWindowData : UIWindowData
{
    public override EnUIWindowType WindowType => EnUIWindowType.Window;
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_UIStartGameWindow;

}
public class UIStartGameWindow : UIWindow
{
    [SerializeField, AutoReference]
    private Button _StartGameBtn = null;
    [SerializeField, AutoReference]
    private RectTransform _CharacterItemRootRect = null;

    private List<int> _CharacterItemList = new();
    private Dictionary<int, int> _CharacterID2Index = new();
    private int _CurSelectID = -1;
    public override void OnHide()
    {
        UIMgr.Instance.RemoveBtnListener(_StartGameBtn, OnClick_StartGameBtn);
        for (int i = 0; i < _CharacterItemList.Count; i++)
            UIMgr.Instance.DestroyWindowItem(_CharacterItemList[i]);
        _CurSelectID = -1;
        _CharacterID2Index.Clear();
        _CharacterItemList.Clear();
    }
    public override void OnShow()
    {
        UIMgr.Instance.AddBtnListener(_StartGameBtn, OnClick_StartGameBtn);

        var count = GameSchedule.Instance.GetCharacterCfgCount();
        for (int i = 0; i < count; i++)
        {
            var characterCfg = GameSchedule.Instance.GetCharacterCfg(i);
            var itemDataID = UIMgr.Instance.CreateWindowItem<UICharacterItemData>(_CharacterItemRootRect, null);
            var itemData = EntityMgr.Instance.GetEntityData<UICharacterItemData>(itemDataID);
            itemData.SetCharacterID(characterCfg.nCharacterID);
            itemData.SetClickEvent(OnClickCharacterItem);
            _CharacterItemList.Add(itemDataID);
            _CharacterID2Index.Add(characterCfg.nCharacterID, _CharacterItemList.Count - 1);
        }
        UpdateEnterBtnActive();
    }
    private void OnClickCharacterItem(int characterID)
    {
        var lastID = _CurSelectID;
        if (lastID > 0)
        {
            var index = _CharacterID2Index[lastID];
            var entityID = _CharacterItemList[index];
            var itemData = EntityMgr.Instance.GetEntityData<UICharacterItemData>(entityID);
            itemData.SetSelectStatus(false);
            _CurSelectID = -1;
        }
        if (characterID == lastID)
        {
            UpdateEnterBtnActive();
            return;
        }

        _CurSelectID = characterID;

        var index2 = _CharacterID2Index[characterID];
        var entityID2 = _CharacterItemList[index2];
        var itemData2 = EntityMgr.Instance.GetEntityData<UICharacterItemData>(entityID2);
        itemData2.SetSelectStatus(true);
        UpdateEnterBtnActive();
    }
    private void OnClick_StartGameBtn()
    {
        //UIMgr.Instance.ShowWindow<UILevelWindowData>();

        GameMgr.Instance.EnterLevel(1, _CurSelectID);

        UIMgr.Instance.HideAllWindow();

        UIMgr.Instance.ShowWindow<UIBattleWindowData>();
    }
    private void UpdateEnterBtnActive()
    {
        _StartGameBtn.gameObject.SetActive(_CurSelectID > 0);
    }

}
