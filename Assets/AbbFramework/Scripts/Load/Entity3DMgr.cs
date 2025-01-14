using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;


public class Entity3DMgr : Singleton<Entity3DMgr>
{
    public override EnManagerFuncType FuncType => base.FuncType | EnManagerFuncType.Update;
    private Dictionary<int, Entity3DData> m_ID2Entity3DData = new();
    public Entity3DData GetEntity3DData(int entityID)
    {
        if (!m_ID2Entity3DData.TryGetValue(entityID, out var entityData))
            return null;
        return entityData;
    }
    public T GetEntity3DData<T>(int entityID)
        where T : Entity3DData
    {
        var entityData = GetEntity3DData(entityID);
        if (entityData is not T tData)
            return null;
        return tData;
    }

    public int CreateEntityData<T>()
        where T : Entity3DData, new()
    {
        var entityID = EntityMgr.Instance.CreateEntityData<T>();
        var entityData = EntityMgr.Instance.GetEntityData<T>(entityID);
        m_ID2Entity3DData.Add(entityID, entityData);
        return entityID;
    }
    public void RecycleEntityData(int entityID)
    {
        m_ID2Entity3DData.Remove(entityID);
        EntityMgr.Instance.RecycleEntityData(entityID);
    }
    public void LoadEntity(int entityID)
    {
        EntityMgr.Instance.LoadEntity(entityID);
    }
    public void UnloadEntity(int entityID)
    {
        EntityMgr.Instance.UnloadEntity(entityID);
    }
    public int EntityID2RoleID(int m_EntityID)
    {
        return 1;
    }



    public Vector3 GetEntityWorldPos(int entityID)
    {
        var entity = GetEntity3DData(entityID);
        return entity.WorldPos;
    }
    public Vector3 GetEntityForward(int entityID)
    {
        var entity = GetEntity3DData(entityID);
        return entity.Forward;

    }
    public Vector3 GetEntityUp(int entityID)
    {
        var entity = GetEntity3DData(entityID);
        return entity.Up;

    }
    public Vector3 GetEntityRight(int entityID)
    {
        var entity = GetEntity3DData(entityID);
        return entity.Right;

    }
    public Vector3 GetEntityRotation(int entityID)
    {
        var entity = GetEntity3DData(entityID);
        return entity.LocalRotation;
    }
    public bool GetEntityIsCanJump(int entityID)
    {
        var entity = GetEntity3DData(entityID);
        return entity.IsCanJump();
    }
    public bool GetEntityIsCanMove(int entityID)
    {
        var entity = GetEntity3DData(entityID);
        return entity.IsCanMove();
    }
    public void AddEntityBuff(int entityID, EnBuff buff, IEntityBuffParams buffParams = null)
    {
        var entity = GetEntity3DData(entityID);
        var animCom = entity.GetEntityCom<EntityBuffComData>();
        animCom.AddBuff(buff, buffParams);
    }
    public bool ContainsBuff(int entityID, EnBuff buff)
    {
        var entity = GetEntity3DData(entityID);
        var animCom = entity.GetEntityCom<EntityBuffComData>();
        var contains = animCom.ContainsBuff(buff);
        return contains;
    }

    public void RemoveEntityBuff(int entityID, EnBuff buff)
    {
        var entity = GetEntity3DData(entityID);
        var animCom = entity.GetEntityCom<EntityBuffComData>();
        animCom.RemoveBuff(buff);
    }
    public void AddEntityCmd(int entityID, EnEntityCmd target)
    {
        var entity = GetEntity3DData(entityID);
        var animCom = entity.GetEntityCom<EntityAnimComData>();
        animCom.AddCmd(target);
    }

    public void RemoveEntityCmd(int entityID, EnEntityCmd target)
    {
        var entity = GetEntity3DData(entityID);
        var animCom = entity.GetEntityCom<EntityAnimComData>();
        animCom.RemoveCmd(target);
    }
    public void CancelEntityCmd(int entityID, EnEntityCmd target)
    {
        var entity = GetEntity3DData(entityID);
        var animCom = entity.GetEntityCom<EntityAnimComData>();
        animCom.CancelCmd(target);
    }
    public float GetEntityVerticalVelocity(int entityID)
    {
        var entityData = GetEntity3DData(entityID);
        var ccCom = entityData.GetEntityCom<EntityCCComData>();
        return ccCom.GetVerticalVelocity();
    }
    public void IncrementSetEntityMoveDirection(int entityID, Vector3 moveDirection)
    {
        var entityData = GetEntity3DData(entityID);
        var ccCom = entityData.GetEntityCom<EntityCCComData>();
        var moveDir = ccCom.GetMoveDirection();
        ccCom.SetMoveDirection(moveDir + moveDirection);
    }
    public Vector3 GetEntityMoveDirection(int entityID)
    {
        var entityData = GetEntity3DData(entityID);
        var ccCom = entityData.GetEntityCom<EntityCCComData>();
        var dir = ccCom.GetMoveDirection();
        return dir;
    }
    public void ExecuteEntityJump(int entityID)
    {
        var entityData = GetEntity3DData(entityID);
        if (!entityData.IsCanJump())
            return;
        var ccCom = entityData.GetEntityCom<EntityCCComData>();
        var jumpCount = ccCom.GetJumpCount();
        if (ccCom.GetJumpCount() < ccCom.GetJumpMaxCount())
        {
            var jumpHeight = ccCom.GetJumpHeight();
            ccCom.SetVerticalVelocity(jumpHeight);
            ccCom.SetJumpCount(jumpCount + 1);
            ccCom.SetJumpSpeed(GlobalConfig.Float1);
        }
    }
    public void SetEntityHeight(int entityID, float heilght, float time)
    {
        var entityData = GetEntity3DData(entityID);
        var ccCom = entityData.GetEntityCom<EntityCCComData>();
        //ccCom.SetVerticalVelocity(heilght);
        ccCom.IncrementMove(Vector3.up * heilght);
    }
    public void SetEntityIsGravity(int entityID, bool isGravity)
    {
        var entityData = GetEntity3DData(entityID);
        var ccCom = entityData.GetEntityCom<EntityCCComData>();
        ccCom.SetIsGravity(isGravity);
    }
    public void SetEntityVerticalVelocity(int entityID, int verticalVelocity)
    {
        var entityData = GetEntity3DData(entityID);
        var ccCom = entityData.GetEntityCom<EntityCCComData>();
        ccCom.SetVerticalVelocity(verticalVelocity);
    }
    public void SetEntityMoveSpeedIncrements(int entityID, float value)
    {
        var entityData = GetEntity3DData(entityID);
        var ccCom = entityData.GetEntityCom<EntityCCComData>();
        ccCom.AddMoveSpeedIncrements(value);
    }
    public void SetEntityRotationSpeed(int entityID, float value)
    {
        var entityData = GetEntity3DData(entityID);
        var ccCom = entityData.GetEntityCom<EntityCCComData>();
        ccCom.SetRotationSpeed(value);
    }
    public void SetEntityRotationStatus(int entityID, bool value)
    {
        var entityData = GetEntity3DData(entityID);
        var ccCom = entityData.GetEntityCom<EntityCCComData>();
        ccCom.SetIsCanRotation(value);
    }
    public void SetApplyRootMotion(int entityID, bool applyRootMotion)
    {
        var entityData = GetEntity3DData(entityID);
        var ccCom = entityData.GetEntityCom<EntityAnimComData>();
        //ccCom.SetVerticalVelocity(heilght);
        ccCom.SetApplyRootMotion(applyRootMotion);
    }
    public override void Update()
    {
        base.Update();

        var timeDelta = ABBUtil.GetTimeDelta();

        foreach (var item in m_ID2Entity3DData)
        {
            var ccCom = item.Value.GetEntityCom<EntityCCComData>();
            var animCom = item.Value.GetEntityCom<EntityAnimComData>();

            if (ccCom == null)
                continue;

            var moveDirection = ccCom.GetMoveDirection();
            var speed = ccCom.GetMoveSpeed();
            var jumpSpeed = ccCom.GetJumpSpeed();
            var verticalVelocity = ccCom.GetVerticalVelocity();


            Update_Jump(item.Value, ccCom, animCom, timeDelta);
            Updtae_Move(item.Value, ccCom, animCom, timeDelta);

            var pos = speed * timeDelta * moveDirection;
            var height = verticalVelocity * timeDelta;
            ccCom.IncrementMove(pos + Vector3.up * height);
            ccCom.SetMoveDirection(Vector3.zero);
        }
    }
    private void Update_Jump(Entity3DData entityData, EntityCCComData ccCom, EntityAnimComData animCom, float timeDelta)
    {
        if (ccCom.IsGrounded())
        {
            if (ccCom.IsJumping())
            {
                animCom.AddCmd(EnEntityCmd.JumpDown);
                ccCom.SetIsJumping(false);
                ccCom.SetJumpCount(GlobalConfig.Int0);
            }
            if (ccCom.GetVerticalVelocity() < -2)
            {
                ccCom.SetVerticalVelocity(-2);
            }
        }
        else
        {
            var jumpSpeed = ccCom.GetJumpSpeed();
            ccCom.SetIsJumping(true);
            var gravity = ccCom.GetGravity();
            var verticaVelocity = ccCom.GetVerticalVelocity();
            var velocity = verticaVelocity + -gravity * timeDelta;
            ccCom.SetVerticalVelocity(velocity);
            animCom.AddCmd(EnEntityCmd.Jump);
            var rotationSpeed = ccCom.GetRotationSpeed();
            entityData.UpdateRotation(timeDelta * rotationSpeed);
        }
    }
    private void Updtae_Move(Entity3DData entityData, EntityCCComData ccCom, EntityAnimComData animCom, float timeDelta)
    {
        var moveDirection = ccCom.GetMoveDirection();
        if (Mathf.Abs(moveDirection.x) + Mathf.Abs(moveDirection.z) == 0)
        {
            animCom.RemoveCmd(EnEntityCmd.Run);
        }
        else
        {
            if (ccCom.IsGrounded())
            {
                animCom.AddCmd(EnEntityCmd.Run);
                var rotationSpeed = ccCom.GetRotationSpeed();
                entityData.UpdateRotation(rotationSpeed * timeDelta);
            }
        }
    }
}
