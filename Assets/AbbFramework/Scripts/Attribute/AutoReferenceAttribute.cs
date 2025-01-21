using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoReferenceAttribute : Attribute
{
    public string objName;
    public AutoReferenceAttribute() { }
    public AutoReferenceAttribute(string name)
    {
        objName = name;
    }
}
