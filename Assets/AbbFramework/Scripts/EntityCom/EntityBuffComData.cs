using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public sealed class EntityBuffComData : Entity3DComData
{
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
    }
    public override void OnPoolInit(Entity3DComDataUserData userData)
    {
        base.OnPoolInit(userData);
    }

    public void AddBuff(int addKey)
    {
        
    }
    public void RemoveBuff(int addKey)
    {

    }
}
