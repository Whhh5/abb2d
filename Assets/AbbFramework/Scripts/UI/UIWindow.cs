using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIWindowData : UIEntityData
{
    public abstract EnUIWindowType WindowType { get; }
    
    public override void OnPoolRecycle()
    {
        base.OnPoolRecycle();
    }
    public override void OnGOCreate()
    {
        base.OnGOCreate();
    }
    public override void OnPoolGet()
    {
        base.OnPoolGet();
    }
    public override void OnGODestroy()
    {
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


}
