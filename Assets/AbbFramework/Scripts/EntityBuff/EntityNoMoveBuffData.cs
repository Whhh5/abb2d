using UnityEngine;

public class EntityNoMoveBuffData : EntityBuffData
{
    public override void OnDisable()
    {
        m_CCCom.SetMoveStatus(true);
    }

    public override void OnEnable(IEntityBuffParams buffParams)
    {
        m_CCCom.SetMoveStatus(false);
    }
}
