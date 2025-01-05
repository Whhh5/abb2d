using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using static UnityEditor.FilePathAttribute;
using static UnityEditor.PlayerSettings;
using static UnityEngine.Rendering.DebugUI.Table;

public interface IPhysicsParams
{
    public EnPhysicsType GetPhysicsType();
    public int[] GetPhysicsParams();
}
public interface IPhysicsColliderCallbackCustomData
{

}
public interface IPhysicsResolve
{
    public void SetParams(ref int[] arrParams, int startIndex);
    public void Execute(int entityID, int layer, PhysicsColliderCallback callback, IPhysicsColliderCallbackCustomData cusomData);
}
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

        var colliders = Physics.OverlapSphere(pos, m_Radius, layer);
        DebugDrawMgr.Instance.DrawSphere(pos, m_Radius, 0.5f);
        callback.Invoke(colliders, cusomData);
    }

    public void SetParams(ref int[] arrParams, int startIndex)
    {
        m_PosOffsetX = arrParams[startIndex++] * 0.01f;
        m_PosOffsetY = arrParams[startIndex++] * 0.01f;
        m_PosOffsetZ = arrParams[startIndex++] * 0.01f;

        m_Radius = arrParams[startIndex++] * 0.01f;
    }
}
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

            var colliders = Physics.OverlapBox(centre, unitHalfSize, Quaternion.Euler(targetRot), layer);
            DebugDrawMgr.Instance.DrawBox(centre, unitHalfSize, targetRot, 0.5f);
            callback(colliders, cusomData);
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
public delegate void PhysicsColliderCallback(Collider[] colliders, IPhysicsColliderCallbackCustomData cusomData);
public class PhysicsMgr : Singleton<PhysicsMgr>
{
    public void PhysicsOverlap<T>(T data, int entityID, int layer, PhysicsColliderCallback callback, IPhysicsColliderCallbackCustomData cusomData)
        where T : IPhysicsResolve
    {
        data.Execute(entityID, layer, callback, cusomData);
    }
}
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
