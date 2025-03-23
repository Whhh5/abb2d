using System.Collections;
using System.Collections.Generic;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using UnityEngine;


public interface IEntity3DCCCom : IEntity3DCom
{
    public CharacterController GetCC();
    public bool IsGrounded();
}


public sealed class EntityCCComData : Entity3DComDataGO<IEntity3DCCCom>, IUpdate
{
    private CharacterController m_CC = null;
    private Transform m_Tran = null;
    // 方向
    private Vector3 m_MoveDirection;
    // 移动
    private bool m_IsCanMove = true;
    private float m_MoveSpeed = 5;
    private float m_MoveSpeedIncrements = 1;
    // 跳跃
    private bool m_IsCanJump = true;
    private float m_JumpSpeed = 1;
    private float m_JumpSpeedIncrements = 1;
    private float m_JumpHeight = 7;
    // 重力
    private bool m_IsGravity = true;
    private float m_VerticalVelocity = -2;
    private float m_Gravity = 20;
    private bool m_IsJumping = false;
    private int m_JumpCount = 0;
    private int m_JumpMaxCount = 3;
    // 旋转
    private float m_RotationSpeed = 1f;
    private bool m_IsCanRotation = true;

    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
    }
    public override void OnDestroyGO()
    {
        base.OnDestroyGO();
        m_CC = null;
        m_Tran = null;

        m_MoveDirection = Vector3.zero;
        m_IsCanMove = true;
        m_MoveSpeed = 5;
        m_MoveSpeedIncrements = 1;

        m_IsCanJump = true;
        m_JumpSpeed = 1;
        m_JumpSpeedIncrements = 1;
        m_JumpHeight = 7;
        
        m_IsGravity = true;
        m_VerticalVelocity = -2;
        m_Gravity = 20;
        m_IsJumping = false;
        m_JumpCount = 0;
        m_JumpMaxCount = 3;
        
        m_RotationSpeed = 1f;
        m_IsCanRotation = true;
    }

    public override void OnPoolInit(Entity3DComDataUserData userData)
    {
        base.OnPoolInit(userData);

    }

    public override void OnEnable()
    {
        base.OnEnable();
        UpdateMgr.Instance.Registener(this);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        UpdateMgr.Instance.Unregistener(this);
    }
    public override void OnCreateGO()
    {
        base.OnCreateGO();
        var entityData = Entity3DMgr.Instance.GetEntity3DData(_EntityID);
        var entity = entityData.GetEntityComponent<Entity3D>();
        m_CC = _GoCom.GetCC();
        m_Tran = entity.transform;
    }

    public void IncrementMove(Vector3 motion)
    {
        if (!Entity3DMgr.Instance.GetEntityIsLoadSuccess(_EntityID))
            return;
        m_CC.Move(motion);
    }
    public void Jump(float height)
    {
        if (!Entity3DMgr.Instance.GetEntityIsLoadSuccess(_EntityID))
            return;
        m_CC.Move(Vector3.up * height);
        //var curPosition = m_Entity3DData.WorldPos;
        //m_Entity3DData.SetPosition(curPosition + Vector3.up * height);
    }
    public bool IsGrounded()
    {
        if (!Entity3DMgr.Instance.GetEntityIsLoadSuccess(_EntityID))
            return true;
        return _GoCom.IsGrounded();
    }

    public void Update()
    {
        if (!Entity3DMgr.Instance.GetEntityIsLoadSuccess(_EntityID))
            return;
        Entity3DMgr.Instance.SetEntityWorldPos(_EntityID, m_Tran.position);
    }

    public void SetMoveDirection(Vector3 direction)
    {
        m_MoveDirection = direction;
    }
    public Vector3 GetMoveDirection()
    {
        return m_MoveDirection;
    }
    public void SetMoveSpeed(float speed)
    {
        m_MoveSpeed = speed;
    }
    public void SetMoveSpeedIncrements(float increments)
    {
        m_MoveSpeedIncrements = increments;
    }
    public float GetMoveSpeedIncrements()
    {
        return m_MoveSpeedIncrements;
    }
    public void AddMoveSpeedIncrements(float increments)
    {
        m_MoveSpeedIncrements += increments;
    }
    public void SetMoveStatus(bool isCanMove)
    {
        m_IsCanMove = isCanMove;
    }
    public bool GetMoveStatus()
    {
        return m_IsCanMove;
    }
    public float GetMoveSpeed()
    {
        var increments = Mathf.Max(m_MoveSpeedIncrements, 0);
        return m_IsCanMove ? m_MoveSpeed * increments : 0;
    }
    public void SetJumpStatus(bool isCanJump)
    {
        m_IsCanJump = isCanJump;
    }
    public bool GetJumpStatus()
    {
        return m_IsCanJump;
    }
    public void SetJumpSpeedIncrements(float increments)
    {
        m_JumpSpeedIncrements = increments;
    }
    public void SetJumpSpeed(float jumpSpeed)
    {
        m_JumpSpeed = jumpSpeed;
    }
    public float GetJumpSpeed()
    {
        return m_IsCanJump ? m_JumpSpeed * m_JumpSpeedIncrements : 0;
    }
    public void SetJumpHeight(float jumpHeight)
    {
        m_JumpHeight = jumpHeight;
    }
    public float GetJumpHeight()
    {
        return m_JumpHeight;
    }
    public void SetVerticalVelocity(float verticalVelocity)
    {
        m_VerticalVelocity = verticalVelocity;
    }
    public float GetVerticalVelocity()
    {
        return m_VerticalVelocity;
    }
    public void SetGravity(float gravity)
    {
        m_Gravity = gravity;
    }
    public float GetGravity()
    {
        return m_IsGravity ? m_Gravity : 0;
    }
    public void SetIsGravity(bool isGravity)
    {
        m_IsGravity = isGravity;
    }
    public bool GetIsGravity()
    {
        return m_IsGravity;
    }
    public void SetIsJumping(bool isJumping)
    {
        m_IsJumping = isJumping;
    }
    public bool IsJumping()
    {
        return m_IsJumping;
    }
    public void SetJumpCount(int jumpCount)
    {
        m_JumpCount = jumpCount;
    }
    public int GetJumpCount()
    {
        return m_JumpCount;
    }
    public void SetIsJumping(int jumpMaxCount)
    {
        m_JumpMaxCount = jumpMaxCount;
    }
    public int GetJumpMaxCount()
    {
        return m_JumpMaxCount;
    }
    public void SetIsCanRotation(bool isCanRotation)
    {
        m_IsCanRotation = isCanRotation;
    }
    public bool GetIsCanRotation()
    {
        return m_IsCanRotation;
    }
    public void SetRotationSpeed(float rotationSpeed)
    {
        m_RotationSpeed = rotationSpeed;
    }
    public float GetRotationSpeed()
    {
        return m_IsCanRotation ? m_RotationSpeed : 0;
    }

}