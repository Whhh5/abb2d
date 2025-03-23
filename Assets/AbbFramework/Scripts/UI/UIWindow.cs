using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIWindowData : UIEntityData
{
    public abstract EnUIWindowType WindowType { get; }
    private UIWindow _UIWindow = null;

    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
    }
    public override void OnGOCreate()
    {
        base.OnGOCreate();
        _UIWindow = m_Entity as UIWindow;
        _UIWindow.OnShow();
    }
    public override void OnPoolInit(IClassPoolUserData userData)
    {
        base.OnPoolInit(userData);
    }
    public override void OnGODestroy()
    {
        _UIWindow.OnHide();
        base.OnGODestroy();
    }
    public virtual void OnShow(object userData)
    {

    }
    public virtual void OnHide()
    {
    }
}
public abstract class UIWindow : UIEntity
{
    public abstract void OnShow();
    public abstract void OnHide();
}
