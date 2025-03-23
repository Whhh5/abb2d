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
    [SerializeField]
    private Button m_StartGameBtn = null;

    private void OnClick_StartGameBtn()
    {
        UIMgr.Instance.HideAllWindow();
        //UIMgr.Instance.ShowWindow<UILevelWindowData>();

        GameMgr.Instance.EnterLevel(1);

        UIMgr.Instance.ShowWindow<UIBattleWindowData>();
    }

    public override void OnShow()
    {
        UIMgr.Instance.AddBtnListener(m_StartGameBtn, OnClick_StartGameBtn);
    }

    public override void OnHide()
    {
        UIMgr.Instance.RemoveBtnListener(m_StartGameBtn, OnClick_StartGameBtn);
    }
}
