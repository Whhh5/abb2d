using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMgr : Singleton<PlayerMgr>
{
    public override EnManagerFuncType FuncType => base.FuncType | EnManagerFuncType.Update;
    private int m_CurNum = 0;

    private int m_PlayerEntityID = -1;

    public void AddNum(int num)
    {
        m_CurNum += num;
        var playerEntityData = EntityMgr.Instance.GetEntityData<PlayerEntityData>(m_PlayerEntityID);
        playerEntityData.SetNum(m_CurNum);
    }
    public int GetNum()
    {
        return m_CurNum;
    }

    public void CreatePlayerEntity(MapDataSO mapDataSO)
    {
        var worldPos = mapDataSO.GetPlayerStartPos();
        m_PlayerEntityID = EntityMgr.Instance.CreateEntityData<PlayerEntityData>(EnLoadTarget.Pre_PlayerEntity);
        var playerEntityData = EntityMgr.Instance.GetEntityData<PlayerEntityData>(m_PlayerEntityID);
        playerEntityData.SetNum(10);
        playerEntityData.SetPosition(worldPos);
        EntityMgr.Instance.LoadEntity(m_PlayerEntityID);

    }
    public void IncrementMovePlayer(Vector3 value)
    {
        var playerEntityData = EntityMgr.Instance.GetEntityData<PlayerEntityData>(m_PlayerEntityID);
        playerEntityData.IncrementMove(value);
    }

    public override void Update()
    {
        base.Update();
        if (m_PlayerEntityID < 0)
            return;
        if (!GameMgr.Instance.IsStatus(EnGameStatus.Playing))
            return;
        IncrementMovePlayer(Vector3.forward * ABBUtil.GetTimeDelta());
    }
}
