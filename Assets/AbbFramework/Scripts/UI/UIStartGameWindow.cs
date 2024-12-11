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

    public override void OnUnload()
    {
        UIMgr.Instance.RemoveBtnListener(m_StartGameBtn, OnClick_StartGameBtn);
        base.OnUnload();
    }
    public override void LoadCompeletion()
    {
        base.LoadCompeletion();

        UIMgr.Instance.AddBtnListener(m_StartGameBtn, OnClick_StartGameBtn);

    }
    private void OnClick_StartGameBtn()
    {
        UIMgr.Instance.HideAllWindow();
        //UIMgr.Instance.ShowWindow<UILevelWindowData>();

        GameMgr.Instance.EnterLevel(1);
    }
}
