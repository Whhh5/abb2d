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
}
