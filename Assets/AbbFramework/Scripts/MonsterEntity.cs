using UnityEngine;


public class MonsterEntityData : Entity3DData
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_MonsterEntity;
}
public class MonsterEntity : Entity3D, IEntity3DCCCom
{
    [SerializeField]
    private CharacterController m_CCCom = null;

    public override void LoadCompeletion()
    {
        base.LoadCompeletion();

        m_CCCom.Move(m_EntityData.WorldPos - transform.position);
    }
    public CharacterController GetCC()
    {
        return m_CCCom;
    }

    public bool IsGrounded()
    {
        return m_CCCom.isGrounded;
    }
    public override void SetPosition()
    {
        //base.SetPosition();
    }
}
