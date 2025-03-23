using System.Collections.Generic;
using UnityEngine;

public enum EnEntityControllerType
{
    None,
    AI,
    Manual,
}
public class Entity3DMgr : Singleton<Entity3DMgr>
{
    #region struct
    public interface IEntityController
    {
        public void OnDisable(int entityID);
        public void OnEnable(int entityID);
    }
    public class EntityAIController : IEntityController
    {
        public void OnDisable(int entityID)
        {
            Entity3DMgr.Instance.RemoveEntityCom<EntityAIComData>(entityID);
        }

        public void OnEnable(int entityID)
        {
            Entity3DMgr.Instance.AddEntityCom<EntityAIComData>(entityID);
        }
    }
    public class EntityManualController : IEntityController
    {
        public void OnDisable(int entityID)
        {
            PlayerMgr.Instance.ClearControllerPlayerID();
            CameraMgr.Instance.ClearLookAt();
        }

        public void OnEnable(int entityID)
        {
            CameraMgr.Instance.SetLookAtTran(entityID);
            PlayerMgr.Instance.SetControllerPlayerID(entityID);
        }
    }
    public class EntityNoneController : IEntityController
    {
        public void OnDisable(int entityID)
        {

        }

        public void OnEnable(int entityID)
        {

        }
    }
    #endregion

    public override void Destroy()
    {
        base.Destroy();
        foreach (var item in m_ID2Entity3DData)
        {
            item.Value.RemoveEntityCom<EntityAnimComData>();
        }
    }
    public override EnManagerFuncType FuncType => base.FuncType | EnManagerFuncType.Update;
    private Dictionary<int, Entity3DData> m_ID2Entity3DData = new();

    private Dictionary<EnEntityControllerType, IEntityController> _ControllerDic = new()
    {
        { EnEntityControllerType.None, new EntityNoneController() },
        { EnEntityControllerType.Manual, new EntityManualController() },
        { EnEntityControllerType.AI, new EntityAIController() },
    };
    public Entity3DData GetEntity3DData(int entityID)
    {
        if (!m_ID2Entity3DData.TryGetValue(entityID, out var entityData))
            return null;
        return entityData;
    }
    public T GetEntityGOComponent<T>(int entityID)
        where T : class
    {
        var entityData = Entity3DMgr.Instance.GetEntity3DData(entityID);
        var component = entityData.GetEntityComponent<T>();
        return component;
    }
    public T GetEntity3DData<T>(int entityID)
        where T : Entity3DData
    {
        var entityData = GetEntity3DData(entityID);
        if (entityData is not T tData)
            return null;
        return tData;
    }
    public MonsterBaseData GetMonsterEntityData(int entityID)
    {
        var monsterData = GetEntity3DData<MonsterBaseData>(entityID);
        return monsterData;
    }
    public T GetMonsterEntityData<T>(int entityID)
        where T : MonsterBaseData
    {
        var monsterData = GetEntity3DData<T>(entityID);
        return monsterData;
    }
    public int CreateMonsterEntityData<T>(int monsterID)
        where T : MonsterBaseData, new()
    {
        var userData = ClassPoolMgr.Instance.Pull<MonsterBaseDataUserData>();
        userData.monsterID = monsterID;
        var entityID = EntityMgr.Instance.CreateEntityData<T>(userData);
        var entityData = EntityMgr.Instance.GetEntityData<T>(entityID);
        entityData.AddEntityCom<EntityLifeComData>();
        var lifeCom = entityData.GetEntityCom<EntityLifeComData>();
        lifeCom.SetMaxHealthValue(1000);
        lifeCom.SetCurHealthValue(1000);
        m_ID2Entity3DData.Add(entityID, entityData);
        ClassPoolMgr.Instance.Push(userData);
        return entityID;
    }
    public int CreateEntityData<T>()
        where T : Entity3DData, new()
    {
        var entityID = CreateEntityData<T>(null);
        return entityID;
    }
    public int CreateEntityData<T>(IClassPoolUserData userData)
        where T : Entity3DData, new()
    {
        var entityID = EntityMgr.Instance.CreateEntityData<T>(userData);
        var entityData = EntityMgr.Instance.GetEntityData<T>(entityID);
        m_ID2Entity3DData.Add(entityID, entityData);
        return entityID;
    }
    public void RecycleEntityData(int entityID)
    {
        m_ID2Entity3DData.Remove(entityID);
        EntityMgr.Instance.RecycleEntityData(entityID);
    }
    private HashSet<int> _CorpseEntityID = new();
    public void DieEntity(int entityID)
    {
        _CorpseEntityID.Add(entityID);
        if (ContainsEntityCom<EntityLifeComData>(entityID))
        {
            var lifeCom = GetEntityCom<EntityLifeComData>(entityID);
            lifeCom.SetEntityStatus(EnEntityStatus.Die);
        }
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



    public void SetEntityWorldPos(int entityID, Vector3 worldPos)
    {
        var entity = GetEntity3DData(entityID);
        entity.SetPosition(worldPos);
    }
    public Vector3 GetEntityWorldPos(int entityID)
    {
        var entity = GetEntity3DData(entityID);
        return entity.WorldPos;
    }
    public Vector3 GetEntityForward(int entityID)
    {
        var entity = GetEntity3DData(entityID);
        return entity.GetForward();

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
    public bool ContainsEntityCom<T>(int entityID)
        where T : class, IEntity3DComData, new()
    {
        var entity = GetEntity3DData(entityID);
        if (!entity.ContainsEntityCom<T>())
            return false;
        return true;
    }
    public T GetEntityCom<T>(int entityID)
        where T : class, IEntity3DComData, new()
    {
        var entity = GetEntity3DData(entityID);
        var com = entity.GetEntityCom<T>();
        return com;
    }
    public T AddEntityCom<T>(int entityID)
        where T : class, IEntity3DComData, new()
    {
        var entity = GetEntity3DData(entityID);
        if (!entity.AddEntityCom<T>())
            return null;
        var com = entity.GetEntityCom<T>();
        return com;
    }
    public bool RemoveEntityCom<T>(int entityID)
        where T : class, IEntity3DComData, new()
    {
        var entity = GetEntity3DData(entityID);
        if (!entity.RemoveEntityCom<T>())
            return false;
        return true;
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
    public EnEntityCmd GetEntityCurCmd(int entityID)
    {
        var entity = GetEntity3DData(entityID);
        var animCom = entity.GetEntityCom<EntityAnimComData>();
        var cmd = animCom.GetCurCmd();
        return cmd;
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
    public void SetEntityMoveDirection(int entityID, Vector3 dir)
    {
        var entityData = GetEntity3DData(entityID);
        var ccCom = entityData.GetEntityCom<EntityCCComData>();

        if (!ccCom.GetIsCanRotation())
            return;

        ForceSetEntityMoveDirection(entityID, dir);
    }
    public void ForceSetEntityMoveDirection(int entityID, Vector3 dir)
    {
        var entityData = GetEntity3DData(entityID);

        var targetDir = dir.normalized;
        var curDir = entityData.GetForward();

        var dot = Vector3.Cross(curDir, targetDir);
        var addValue = (dot.y > 0 ? 1 : -1) * Mathf.PI * ABBUtil.GetTimeDelta() * Mathf.Rad2Deg * 5;

        var angleMax = Quaternion.Angle(Quaternion.Euler(curDir), Quaternion.Euler(dir)) * Mathf.Rad2Deg;

        var lerpValue = Mathf.Clamp(Mathf.Abs(angleMax / 45f), 0.5f, 1f);

        var angle = Vector3.up * Mathf.Min(angleMax, addValue * lerpValue);
        entityData.SetMoveDirection(angle);
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
                if (ccCom.GetIsGravity())
                {
                    animCom.AddCmd(EnEntityCmd.JumpDown);
                }
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
        var value = Mathf.Abs(moveDirection.x) + Mathf.Abs(moveDirection.z);
        if (value == 0)
        {
            animCom.RemoveCmd(EnEntityCmd.Run);
            animCom.RemoveCmd(EnEntityCmd.PlayerWalk);
        }
        else
        {
            if (ccCom.IsGrounded())
            {
                animCom.AddCmd(EnEntityCmd.Run);
            }
        }
        //var rotationSpeed = ccCom.GetRotationSpeed();
        //entityData.UpdateRotation(rotationSpeed * timeDelta);
    }

    public void SetEntityControllerType(int entityID, EnEntityControllerType controllerType)
    {
        var entity = GetMonsterEntityData(entityID);
        var curControllerType = entity.GetEntityControllerType();
        if (controllerType == curControllerType)
            return;
        if (!_ControllerDic.TryGetValue(curControllerType, out var controlller))
            return;
        if (!_ControllerDic.TryGetValue(controllerType, out var controlller2))
            return;
        controlller.OnDisable(entityID);
        controlller2.OnEnable(entityID);
        entity.SetEntityControllerType(controllerType);
    }

    public bool IsHashEntityCom<T>(int entityID)
        where T : IEntity3DCom
    {
        var entityData = GetEntity3DData(entityID);
        if (!entityData.ContainsEntityCom<EntityLifeComData>())
            return false;
        return true;
    }
    public int GetEntityHealthValue(int entityID)
    {
        var entityData = GetEntity3DData(entityID);
        if (!entityData.ContainsEntityCom<EntityLifeComData>())
            return -1;
        var healthCom = entityData.GetEntityCom<EntityLifeComData>();
        return healthCom.GetCurHealthValue();
    }
    public void SetEntityHealthValue(int entityID, int value)
    {
        var entityData = GetEntity3DData(entityID);
        if (!entityData.ContainsEntityCom<EntityLifeComData>())
            return;
        var healthCom = entityData.GetEntityCom<EntityLifeComData>();
        var v = Mathf.Clamp(value, 0, healthCom.GetMaxHealthValue());
        healthCom.SetCurHealthValue(v);
    }
    public int GetEntityMaxHealthValue(int entityID)
    {
        var entityData = GetEntity3DData(entityID);
        if (!entityData.ContainsEntityCom<EntityLifeComData>())
            return -1;
        var healthCom = entityData.GetEntityCom<EntityLifeComData>();
        return healthCom.GetMaxHealthValue();
    }
    public bool GetEntityIsLoadSuccess(int entityID)
    {
        var entityData = GetEntity3DData(entityID);
        return entityData.IsLoadSuccess;
    }
}
