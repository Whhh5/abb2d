using System;
using UnityEngine;
using static AttackCmdPlayableAdapter;




public class PhysicsOverlapCallbackCustomData : IPhysicsColliderCallbackCustomData
{
    public int atkValue;
    public int entityID;
}
public class AttackLinkPhysicsItem : IPhysicsParams, IAttackLinkScheduleItem<PoolNaNUserData>
{

    public float atkSchedule;
    public int atkValue;
    public EnPhysicsType physicsType = EnPhysicsType.Sphere;
    public int[] physicsParams;
    public IPhysicsResolve physicsResolve;


    private EnAtkLinkScheculeType m_ScheduleType = EnAtkLinkScheculeType.None;

    public bool m_IsAtked = false;



    public void OnPoolDestroy()
    {
        m_IsAtked = false;
        atkSchedule = -1;
        atkValue = -1;
        physicsType = EnPhysicsType.Sphere;
        physicsParams = null;
        physicsResolve = null;
        m_ScheduleType = EnAtkLinkScheculeType.None;

    }
    public void PoolConstructor()
    {
    }

    public void OnPoolInit(PoolNaNUserData userData)
    {
    }

    public void OnPoolEnable()
    {
    }

    public void PoolRelease()
    {
    }

    public void SetScheduleType(EnAtkLinkScheculeType scheduleType)
    {
        m_ScheduleType = scheduleType;
    }

    public EnAtkLinkScheculeType GetScheduleType()
    {
        return m_ScheduleType;
    }
    public virtual void Init(int[] data, int arrCount, ref int startIndex)
    {
        var endIndex = startIndex + arrCount;
        var gCount = startIndex >= endIndex ? default : data[startIndex++];
        atkSchedule = gCount < 1 ? default : data[startIndex++] / 100f;
        atkValue = gCount < 2 ? default : data[startIndex++];
        physicsType = gCount < 3 ? default : (EnPhysicsType)data[startIndex++];

        var paramCount = startIndex >= endIndex ? default : data[startIndex++];
        physicsParams = new int[paramCount];
        data.CopyTo(startIndex, physicsParams, paramCount);
        startIndex += paramCount;

        if (physicsType > 0)
        {
            physicsResolve = PhysicsUtil.CreatePhysicsResolve(this);
        }
    }

    public void Reset()
    {
        m_IsAtked
            = false;
    }

    EnPhysicsType IPhysicsParams.GetPhysicsType()
    {
        return physicsType;
    }

    public int[] GetPhysicsParams()
    {
        return physicsParams;
    }

    public void Enter(int entityID)
    {
        var data = new PhysicsOverlapCallbackCustomData()
        {
            atkValue = atkValue,
            entityID = entityID,
        };
        PhysicsMgr.Instance.PhysicsOverlap(physicsResolve, entityID, 1 << (int)EnGameLayer.Monster, PhysicsOverlapCallback, data);
    }

    public void Exit()
    {

    }

    public bool GetIsEffect()
    {
        return m_IsAtked;
    }

    public void SetIsEffect(bool isEffect)
    {
        m_IsAtked = isEffect;
    }
    private void PhysicsOverlapCallback(Collider[] colliders, IPhysicsColliderCallbackCustomData customData)
    {
        var data = customData as PhysicsOverlapCallbackCustomData;
        var pos = Entity3DMgr.Instance.GetEntityWorldPos(data.entityID);
        foreach (var item in colliders)
        {
            var com = item.GetComponent<Entity3D>();
            AttackMgr.Instance.AttackEntity(data.entityID, com, data.atkValue);
            Debug.DrawLine(pos, item.ClosestPoint(pos), Color.red, 1);
        }
    }

    public float GetEnterSchedule()
    {
        return atkSchedule;
    }
}