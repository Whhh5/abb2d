using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : Singleton<GameMgr>
{

    private EnGameStatus m_GameSatus = EnGameStatus.Playing;
    public bool IsStatus(EnGameStatus gameStatus)
    {
        return m_GameSatus == gameStatus;
    }

    public void EnterLevel(int level)
    {
        var worldPos = new Vector3(-82.4530029f, 1.64600003f, 0);
        PlayerMgr.Instance.CreatePlayerEntity(Vector3.up * 3);
    }
}
