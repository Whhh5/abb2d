using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class EntityPropertyInfoEditor : EntityPropertyInfo
{
    public abstract void GetStr(ref List<int> result);
}