using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class PhysicsUtil
{
    public static IPhysicsResolve CreatePhysicsResolve<T>(T data)
        where T : IPhysicsParams
    {
        IPhysicsResolve result = null;

        switch (data.GetPhysicsType())
        {
            case EnPhysicsType.Sphere:
                result = new PhysicsResolveSphere();
                break;
            case EnPhysicsType.Box:
                result = new PhysicsResolveBox();
                break;
            default:
                break;
        }
        var arrParams = data.GetPhysicsParams();
        result.SetParams(ref arrParams, 0);
        return result;
    }
}