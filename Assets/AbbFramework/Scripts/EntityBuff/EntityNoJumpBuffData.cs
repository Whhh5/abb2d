using UnityEngine;


public class EntityNoJumpBuffData : EntityBuffData
{
    public override void OnDisable()
    {
        m_CCCom.SetJumpStatus(true);
    }

    public override void OnEnable(IEntityBuffParams buffParams)
    {
        var curVelocity = m_CCCom.GetVerticalVelocity();
        m_CCCom.SetVerticalVelocity(Mathf.Min(curVelocity, 0));
        m_CCCom.SetJumpStatus(false);
    }
}
