using UnityEngine;

public class DebugDrawMgr : Singleton<DebugDrawMgr>
{
    public void DrawBox(Vector3 worldPos, Vector3 halfSize, Vector3 rot, float duration)
    {
        var entityID = Entity3DMgr.Instance.CreateEntityData<DebugDrawBoxData>();
        var entityData = Entity3DMgr.Instance.GetEntity3DData<DebugDrawBoxData>(entityID);
        entityData.SetDurationTime(duration);
        entityData.SetLocalRotation(rot);
        entityData.SetLocalScale(halfSize * 2);
        entityData.SetPosition(worldPos);
        Entity3DMgr.Instance.LoadEntity(entityID);
    }
    public void DrawSphere(Vector3 worldPos, float radius, float duration)
    {
        var entityID = Entity3DMgr.Instance.CreateEntityData<DebugDrawSphereData>();
        var entityData = Entity3DMgr.Instance.GetEntity3DData<DebugDrawSphereData>(entityID);
        entityData.SetLocalScale(Vector3.one * radius * 2);
        entityData.SetPosition(worldPos);
        entityData.SetDurationTime(duration);
        Entity3DMgr.Instance.LoadEntity(entityID);
    }
}
