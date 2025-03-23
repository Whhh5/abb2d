using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public enum EnKeyCodeControler
{
    None,
    Player,
    Monster0,
}
public class PlayerMgr : Singleton<PlayerMgr>
{
    public override EnManagerFuncType FuncType => base.FuncType | EnManagerFuncType.Update;

    private int _PlayerEntityID = -1;
    private IEntityKeyCodeController _KeyCodeController = null;

    private IEntityKeyCodeController GetController(int controllerID)
    {
        return (EnKeyCodeControler)controllerID switch
        {
            EnKeyCodeControler.Player => ClassPoolMgr.Instance.Pull<PlayerKeyCodeController>(),
            EnKeyCodeControler.Monster0 => ClassPoolMgr.Instance.Pull<Monster0KeyCodeController>(),
            _ => null
        };
    }

    public void SetControllerPlayerID(int entityID)
    {
        if (_PlayerEntityID == entityID)
            return;
        if (_PlayerEntityID > 0)
            ClearControllerPlayerID();

        _PlayerEntityID = entityID;

        Entity3DMgr.Instance.AddEntityCom<EntityCameraComData>(entityID);
        CameraMgr.Instance.SetLookAtTran(entityID);

        var monsterID = EntityUtil.EntityID2MonsterID(entityID);
        var monsterCfg = GameSchedule.Instance.GetMonsterCfg0(monsterID);
        _KeyCodeController = GetController(monsterCfg.nKeyCodeControllerID);
        _KeyCodeController.OnEnable(entityID);
    }
    public void ClearControllerPlayerID()
    {
        if (_PlayerEntityID <= 0)
            return;
        Entity3DMgr.Instance.RemoveEntityCom<EntityCameraComData>(_PlayerEntityID);
        _KeyCodeController.OnDisable();
        ClassPoolMgr.Instance.Push(_KeyCodeController);
        _KeyCodeController = null;
        _PlayerEntityID = -1;
    }


}
