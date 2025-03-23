using UnityEngine;
using UnityEngine.AI;

public class MonsterEntityData : MonsterBaseData
{
    
}
public class MonsterEntity : MonsterBase, IEntity3DCCCom
{
    [SerializeField]
    private CharacterController m_CCCom = null;
    [SerializeField]
    private Trigger3D m_IsGroundTrigger = null;

    NavMeshPath _NavPath = null;
    protected override void Awake()
    {
        base.Awake();

        _NavPath = new();
    }

    public override void LoadCompeletion()
    {
        m_CCCom.enabled = false;
        base.LoadCompeletion();
        transform.position = m_EntityData.WorldPos;
        m_CCCom.enabled = true;
    }
    public CharacterController GetCC()
    {
        return m_CCCom;
    }

    public bool IsGrounded()
    {
        return m_IsGroundTrigger.IsEnter();
    }
}
