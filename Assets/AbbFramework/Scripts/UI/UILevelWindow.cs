using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILevelWindowData : UIWindowData
{
    public override EnUIWindowType WindowType => EnUIWindowType.Window;

    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_UILevelWindow;
}
public class UILevelWindow : UIWindow
{
    
}
