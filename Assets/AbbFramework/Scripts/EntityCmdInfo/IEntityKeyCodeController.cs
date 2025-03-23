

using UnityEngine;

public interface IEntityKeyCodeController: IClassPoolNone
{
    public void OnEnable(int entityID);
    public void OnDisable();
}

public abstract class EntityKeyCodeController : IEntityKeyCodeController
{
    protected int _EntityID { get; private set; }
    public virtual void OnDisable()
    {
        ABBInputMgr.Instance.RemoveListaner(KeyCode.A, OnClick_KeyCodeA);
        ABBInputMgr.Instance.RemoveListaner(KeyCode.D, OnClick_KeyCodeD);
        ABBInputMgr.Instance.RemoveListaner(KeyCode.W, OnClick_KeyCodeW);
        ABBInputMgr.Instance.RemoveListaner(KeyCode.S, OnClick_KeyCodeS);
        _EntityID = -1;
    }

    public virtual void OnEnable(int entityID)
    {
        _EntityID = entityID;
        ABBInputMgr.Instance.AddListaner(KeyCode.A, OnClick_KeyCodeA);
        ABBInputMgr.Instance.AddListaner(KeyCode.D, OnClick_KeyCodeD);
        ABBInputMgr.Instance.AddListaner(KeyCode.W, OnClick_KeyCodeW);
        ABBInputMgr.Instance.AddListaner(KeyCode.S, OnClick_KeyCodeS);
    }

    private void OnClick_KeyCodeA()
    {
        if (!EntityUtil.IsValid(_EntityID))
            return;
        var pos = CameraMgr.Instance.GetCameraRight();
        var targetDir = -pos.normalized;

        Entity3DMgr.Instance.IncrementSetEntityMoveDirection(_EntityID, targetDir);
        Entity3DMgr.Instance.SetEntityMoveDirection(_EntityID, targetDir);
    }
    private void OnClick_KeyCodeD()
    {
        if (!EntityUtil.IsValid(_EntityID))
            return;
        var pos = CameraMgr.Instance.GetCameraRight();
        //pos.y = -0.2f;
        Entity3DMgr.Instance.IncrementSetEntityMoveDirection(_EntityID, pos.normalized);
        Entity3DMgr.Instance.SetEntityMoveDirection(_EntityID, pos.normalized);
    }
    private void OnClick_KeyCodeW()
    {
        if (!EntityUtil.IsValid(_EntityID))
            return;
        var playerWorldPos = Entity3DMgr.Instance.GetEntityWorldPos(_EntityID);
        var pos = playerWorldPos - CameraMgr.Instance.GetCameraWorldPos();
        pos.y = 0;
        Entity3DMgr.Instance.IncrementSetEntityMoveDirection(_EntityID, pos.normalized);
        Entity3DMgr.Instance.SetEntityMoveDirection(_EntityID, pos.normalized);
    }
    private void OnClick_KeyCodeS()
    {
        if (!EntityUtil.IsValid(_EntityID))
            return;
        var playerWorldPos = Entity3DMgr.Instance.GetEntityWorldPos(_EntityID);
        var pos = CameraMgr.Instance.GetCameraWorldPos() - playerWorldPos;
        pos.y = 0;
        Entity3DMgr.Instance.IncrementSetEntityMoveDirection(_EntityID, pos.normalized);
        Entity3DMgr.Instance.SetEntityMoveDirection(_EntityID, pos.normalized);
    }
}