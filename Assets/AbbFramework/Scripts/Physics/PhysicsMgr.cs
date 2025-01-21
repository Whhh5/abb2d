using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;


public delegate void PhysicsColliderCallback(Collider[] colliders, IPhysicsColliderCallbackCustomData cusomData);

public class PhysicsMgr : Singleton<PhysicsMgr>
{
    public void PhysicsOverlap<T>(T data, int entityID, int layer, PhysicsColliderCallback callback, IPhysicsColliderCallbackCustomData cusomData)
        where T : IPhysicsResolve
    {
        data.Execute(entityID, layer, callback, cusomData);
    }
}
