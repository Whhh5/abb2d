using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameMgr : Singleton<GameMgr>
{

    private EnGameStatus m_GameSatus = EnGameStatus.Playing;
    public bool IsStatus(EnGameStatus gameStatus)
    {
        return m_GameSatus == gameStatus;
    }

    public async void EnterLevel(int level, int characterID)
    {
        //var worldPos = new Vector3(-82.4530029f, 1.64600003f, 0);
        var playerEntityID = CreatePlayerEntity(characterID, Vector3.up * 3);
        Entity3DMgr.Instance.SetEntityControllerType(playerEntityID, EnEntityControllerType.Manual);

         await CreateMonsterColony();
    }


    public int CreatePlayerEntity(int monsterID, Vector3 startPos)
    {
        var playerID = Entity3DMgr.Instance.CreateMonsterEntityData<PlayerEntityData>(monsterID);
        var playerData = Entity3DMgr.Instance.GetEntity3DData<PlayerEntityData>(playerID);
        playerData.SetPosition(startPos);
        playerData.AddEntityCom<EntityAnimComData>();
        playerData.AddEntityCom<EntityCCComData>();
        playerData.AddEntityCom<EntityBuffComData>();
        Entity3DMgr.Instance.LoadEntity(playerID);

        var monsterCfg = GameSchedule.Instance.GetMonsterCfg0(monsterID);
        if (monsterCfg.nIdleCmdID > 0)
        {
            var animCom = playerData.GetEntityCom<EntityAnimComData>();
            animCom.AddCmd((EnEntityCmd)monsterCfg.nIdleCmdID);
        }
        return playerID;
    }

    private async UniTask CreateMonsterColony()
    {
        var worldPosList = new Vector3[]
        {
            new(-10, 0, 0),
            new(-10, 0, 50),
            new(30, 0, -20),
            new(30, 0, 50),
            new(50, 0, 20),
        };
        for (int i = 0; i < worldPosList.Length; i++)
        {
            var worldPos = worldPosList[i];
            MonsterColonyMgr.Instance.CreateMonsterColony(1, worldPos);
            await UniTask.Delay(5000);
        }
    }
}
