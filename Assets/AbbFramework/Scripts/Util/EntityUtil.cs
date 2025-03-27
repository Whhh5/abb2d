using UnityEngine;


public struct EntityPhysicsInfo
{
    public Vector3 closestPoint;
    public int entityID;
}
static public class EntityUtil
{
    static public bool IsValid(int entityID)
    {
        if (!EntityMgr.Instance.IsValid(entityID))
            return false;
        if (Entity3DMgr.Instance.ContainsEntityCom<EntityLifeComData>(entityID))
        {
            var lifeCom = Entity3DMgr.Instance.GetEntityCom<EntityLifeComData>(entityID);
            if (lifeCom.IsDie())
                return false;
        }
        return true;
    }

    static readonly private Collider[] _TempCollider = new Collider[100];
    static private EntityPhysicsInfo[] _TempEntityID = new EntityPhysicsInfo[100];
    static public bool PhysicsOverlapSphere1(Vector3 worldPos, float radius, int layer, ref int entityID)
    {
        var count = Physics.OverlapSphereNonAlloc(worldPos, radius, _TempCollider, layer);
        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                var col = _TempCollider[i];
                if (!col.TryGetComponent<MonsterBase>(out var entityCom))
                    continue;
                if (!IsValid(entityCom))
                    continue;
                entityID = entityCom;
                return true;
            }
        }
        return false;
    }
    static public ref EntityPhysicsInfo[] PhysicsOverlapSphere(out int count, Vector3 worldPos, float radius, int layer)
    {
        var colliderCount = Physics.OverlapSphereNonAlloc(worldPos, radius, _TempCollider, layer);
        count = 0;
        for (int i = 0; i < colliderCount; i++)
        {
            var col = _TempCollider[i];
            if (!col.TryGetComponent<MonsterBase>(out var entityCom))
                continue;
            if (!IsValid(entityCom))
                continue;

            ref var element = ref _TempEntityID[count++];
            element.entityID = entityCom;
            element.closestPoint = col.ClosestPoint(worldPos);
        }
        return ref _TempEntityID;
    }
    //static public ref int[] PhysicsOverlapBox(out int count, Vector3 worldPos, Vector3 halfSize, Quaternion qua, int layer)
    //{
    //    count = 0;
    //    return ref _TempEntityID;
    //}
    static public int PhysicsOverlapBox(ref EntityPhysicsInfo[] entityIDs, Vector3 worldPos, Vector3 halfSize, Quaternion qua, int layer)
    {
        var count = Physics.OverlapBoxNonAlloc(worldPos, halfSize, _TempCollider, qua, layer);
        var idCount = 0;
        for (int i = 0; i < count; i++)
        {
            var col = _TempCollider[i];
            if (!col.TryGetComponent<MonsterBase>(out var entityCom))
                continue;
            if (!IsValid(entityCom))
                continue;

            ref var element = ref entityIDs[idCount++];
            element.entityID = entityCom;
            element.closestPoint = col.ClosestPoint(worldPos);
        }
        return idCount;
    }

    public static int EntityID2MonsterID(int entityID)
    {
        var monsterData = Entity3DMgr.Instance.GetMonsterEntityData(entityID);
        return monsterData.GetMonsterID();
    }
}
