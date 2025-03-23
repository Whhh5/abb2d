using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;


public class PhysicsResolveBox : IPhysicsResolve
{
    protected EnPhysicsBoxCenterType m_CenterType;
    protected EnPhysicsBoxType m_ExecuteType;
    protected float m_ExecuteTime;
    protected float m_UnitSizeZ;
    protected Vector3 m_BoxSize;
    protected Vector3 m_RotOffset;
    protected float m_PosOffsetZ;
    protected float m_PosOffsetX;
    protected float m_PosOffsetY;

    private int[] _TempEntityIDs = new int[20];
    public async void Execute(int entityID, int layer, PhysicsColliderCallback callback, IPhysicsColliderCallbackCustomData cusomData)
    {
        var entityPos = Entity3DMgr.Instance.GetEntityWorldPos(entityID);
        var forward = Entity3DMgr.Instance.GetEntityForward(entityID);
        var up = Entity3DMgr.Instance.GetEntityUp(entityID);
        var right = Entity3DMgr.Instance.GetEntityRight(entityID);
        var rot = Entity3DMgr.Instance.GetEntityRotation(entityID);

        var posOffset = m_PosOffsetZ + (m_CenterType == EnPhysicsBoxCenterType.Center ? 0 : m_BoxSize.z * 0.5f);
        var pos = entityPos + forward * posOffset + up * m_PosOffsetY + right * m_PosOffsetX;

        //var targetPos = pos + m_PosOffsetZ * forward;
        var targetRot = rot + m_RotOffset;
        var boxRotForward = Quaternion.Euler(targetRot) * Vector3.forward;
        var count = m_BoxSize.z / m_UnitSizeZ;
        var interval = Mathf.Max(0, m_ExecuteTime / count);
        //var unitHalfSizeZ = m_BoxSize.z / count;
        var unitHalfSize = m_BoxSize * 0.5f;
        unitHalfSize.z = m_UnitSizeZ * 0.5f;
        var startPos = pos - m_BoxSize.z * 0.5f * forward;


        for (int i = 0; i < count; i++)
        {
            var centre = startPos + (i + 0.5f) * m_UnitSizeZ * boxRotForward;

            var idCount = EntityUtil.PhysicsOverlapBox(ref _TempEntityIDs, centre, unitHalfSize, Quaternion.Euler(rot), layer);
            DebugDrawMgr.Instance.DrawBox(centre, unitHalfSize, targetRot, 0.5f);
            callback(ref _TempEntityIDs, ref idCount, cusomData);
            await UniTask.WaitForSeconds(interval);
        }
    }

    public void SetParams(ref int[] arrParams, int paramIndex)
    {

        m_PosOffsetX = arrParams[paramIndex++] * 0.01f;
        m_PosOffsetY = arrParams[paramIndex++] * 0.01f;
        m_PosOffsetZ = arrParams[paramIndex++] * 0.01f;

        m_CenterType = (EnPhysicsBoxCenterType)arrParams[paramIndex++];
        m_ExecuteType = (EnPhysicsBoxType)arrParams[paramIndex++];
        m_ExecuteTime = arrParams[paramIndex++] * 0.01f;
        m_UnitSizeZ = arrParams[paramIndex++] * 0.01f;
        m_BoxSize = new Vector3(arrParams[paramIndex++], arrParams[paramIndex++], arrParams[paramIndex++]) * 0.01f;
        var rotX = arrParams[paramIndex++];
        var rotY = arrParams[paramIndex++];
        var rotZ = arrParams[paramIndex++];
        m_RotOffset = new Vector3(rotX, rotY, rotZ) * 0.01f;
    }
}