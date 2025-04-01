

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.Progress;

public interface IEntityKeyCodeController : IClassPoolNone
{
    public void OnEnable(int entityID);
    public void OnDisable();
}

public sealed class EntityKeyCodeController : IEntityKeyCodeController
{
    private int _EntityID = -1;

    private Dictionary<KeyCode, UnityAction> _KeyCodeCallback = new();
    public void OnDisable()
    {
        foreach (var item in _KeyCodeCallback)
        {
            if(item.Key == KeyCode.O)
                ABBInputMgr.Instance.RemoveListaner(item.Key, item.Value);
            else
                ABBInputMgr.Instance.RemoveListanerDown(item.Key, item.Value);
        }
        ABBInputMgr.Instance.RemoveListanerDown(KeyCode.K, OnClick_KeyCodeDownK);
        ABBInputMgr.Instance.RemoveListaner(KeyCode.A, OnClick_KeyCodeA);
        ABBInputMgr.Instance.RemoveListaner(KeyCode.D, OnClick_KeyCodeD);
        ABBInputMgr.Instance.RemoveListaner(KeyCode.W, OnClick_KeyCodeW);
        ABBInputMgr.Instance.RemoveListaner(KeyCode.S, OnClick_KeyCodeS);
        _EntityID = -1;
        _KeyCodeCallback.Clear();
    }

    public void OnEnable(int entityID)
    {
        _EntityID = entityID;
        ABBInputMgr.Instance.AddListanerDown(KeyCode.K, OnClick_KeyCodeDownK);
        ABBInputMgr.Instance.AddListaner(KeyCode.A, OnClick_KeyCodeA);
        ABBInputMgr.Instance.AddListaner(KeyCode.D, OnClick_KeyCodeD);
        ABBInputMgr.Instance.AddListaner(KeyCode.W, OnClick_KeyCodeW);
        ABBInputMgr.Instance.AddListaner(KeyCode.S, OnClick_KeyCodeS);

        var monsterID = EntityUtil.EntityID2MonsterID(entityID);
        var monsterCfg = GameSchedule.Instance.GetMonsterCfg0(monsterID);
        var monsterControllerCfg = GameSchedule.Instance.GetMonsterControllerCfg0(monsterCfg.nKeyCodeControllerID);

        for (int i = 0; i < monsterControllerCfg.arrParams.Length; i += 2)
        {
            var keyCode = (KeyCode)monsterControllerCfg.arrParams[i];
            var cmdID = monsterControllerCfg.arrParams[i + 1];

            _KeyCodeCallback.Add(keyCode, () =>
            {
                if (!EntityUtil.IsValid(entityID))
                    return;
                Entity3DMgr.Instance.AddEntityCmd(_EntityID, (EnEntityCmd)cmdID);
            });

            if (keyCode == KeyCode.O)
                ABBInputMgr.Instance.AddListaner(keyCode, _KeyCodeCallback[keyCode]);
            else
                ABBInputMgr.Instance.AddListanerDown(keyCode, _KeyCodeCallback[keyCode]);
        }
    }

    private void OnClick_KeyCodeA()
    {
        if (!EntityUtil.IsValid(_EntityID))
            return;
        var pos = CameraMgr.Instance.GetCameraRight();
        var targetDir = -pos.normalized;

        Entity3DMgr.Instance.IncrementSetEntityMoveDirection(_EntityID, targetDir);
        Entity3DMgr.Instance.SetEntityLookAtDirection(_EntityID, targetDir);
    }
    private void OnClick_KeyCodeD()
    {
        if (!EntityUtil.IsValid(_EntityID))
            return;
        var pos = CameraMgr.Instance.GetCameraRight();
        Entity3DMgr.Instance.IncrementSetEntityMoveDirection(_EntityID, pos.normalized);
        Entity3DMgr.Instance.SetEntityLookAtDirection(_EntityID, pos.normalized);
    }
    private void OnClick_KeyCodeW()
    {
        if (!EntityUtil.IsValid(_EntityID))
            return;
        var playerWorldPos = Entity3DMgr.Instance.GetEntityWorldPos(_EntityID);
        var pos = playerWorldPos - CameraMgr.Instance.GetCameraWorldPos();
        pos.y = 0;
        Entity3DMgr.Instance.IncrementSetEntityMoveDirection(_EntityID, pos.normalized);
        Entity3DMgr.Instance.SetEntityLookAtDirection(_EntityID, pos.normalized);
    }
    private void OnClick_KeyCodeS()
    {
        if (!EntityUtil.IsValid(_EntityID))
            return;
        var playerWorldPos = Entity3DMgr.Instance.GetEntityWorldPos(_EntityID);
        var pos = CameraMgr.Instance.GetCameraWorldPos() - playerWorldPos;
        pos.y = 0;
        Entity3DMgr.Instance.IncrementSetEntityMoveDirection(_EntityID, pos.normalized);
        Entity3DMgr.Instance.SetEntityLookAtDirection(_EntityID, pos.normalized);
    }
    private void OnClick_KeyCodeDownK()
    {
        if (!EntityUtil.IsValid(_EntityID))
            return;
        Entity3DMgr.Instance.ExecuteEntityJump(_EntityID);
    }
}