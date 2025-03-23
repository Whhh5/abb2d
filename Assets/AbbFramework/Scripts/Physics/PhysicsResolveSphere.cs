using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PhysicsResolveSphere : IPhysicsResolve
{
    protected float m_Radius;
    protected float m_PosOffsetZ;
    protected float m_PosOffsetX;
    protected float m_PosOffsetY;

    public void Execute(int entityID, int layer, PhysicsColliderCallback callback, IPhysicsColliderCallbackCustomData cusomData)
    {
        var entityPos = Entity3DMgr.Instance.GetEntityWorldPos(entityID);
        var forward = Entity3DMgr.Instance.GetEntityForward(entityID);
        var up = Entity3DMgr.Instance.GetEntityUp(entityID);
        var right = Entity3DMgr.Instance.GetEntityRight(entityID);

        var pos = entityPos + forward * m_PosOffsetZ + up * m_PosOffsetY + right * m_PosOffsetX;

        ref var enittyIDs = ref EntityUtil.PhysicsOverlapSphere(out var count, pos, m_Radius, layer);
        DebugDrawMgr.Instance.DrawSphere(pos, m_Radius, 0.5f);
        callback.Invoke(ref enittyIDs, ref count, cusomData);
    }

    public void SetParams(ref int[] arrParams, int startIndex)
    {
        m_PosOffsetX = arrParams[startIndex++] * 0.01f;
        m_PosOffsetY = arrParams[startIndex++] * 0.01f;
        m_PosOffsetZ = arrParams[startIndex++] * 0.01f;

        m_Radius = arrParams[startIndex++] * 0.01f;
    }
}