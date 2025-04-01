using UnityEngine;


public enum EnAIRangeType
{
    None,
    Sphere,
    Box,
}
public class AIRandomMoveModule : AIModule, IUpdate
{
    private EnAIRangeType _AIRangeType = EnAIRangeType.None;
    private int[] _RangeParams = null;
    private Vector3 _CenterPos = Vector3.zero;


    private Vector3 _NextPoint = Vector3.zero;
    private bool _IsFinish = false;
    private float _LastSpeedIncrement = 0;
    private Vector3 _LastPos = Vector3.zero;
    private int _BugCount = 0;

    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        _CenterPos
            = _NextPoint
            = Vector3.zero;
        _AIRangeType = EnAIRangeType.None;
        _RangeParams = null;
        _IsFinish = false;
        _LastSpeedIncrement = 0;
        _LastPos = Vector3.zero;
        _BugCount = 0;
    }
    public override void OnPoolInit(AIModuleUserData userData)
    {
        base.OnPoolInit(userData);
        var data = userData as AIRandomMoveModuleUserData;
        _AIRangeType = data.rangeType;
        _RangeParams = data.typeParams;
        _CenterPos = data.centerPos;
    }

    public override void PreExecute()
    {
        _NextPoint = CreatePoint();

        _LastSpeedIncrement = Random.Range(-0.7f, 0f);
        Entity3DMgr.Instance.SetEntityMoveSpeedIncrements(GetEntityID(), _LastSpeedIncrement);
    }
    public override void Reexecute()
    {
        _LastPos = Vector3.zero;
        _BugCount = 0;
        _IsFinish = false;
        Entity3DMgr.Instance.SetEntityMoveSpeedIncrements(GetEntityID(), -_LastSpeedIncrement);
        _NextPoint = CreatePoint();
        _LastSpeedIncrement = Random.Range(-0.7f, 0f);
        Entity3DMgr.Instance.SetEntityMoveSpeedIncrements(GetEntityID(), _LastSpeedIncrement);
    }
    public override void Execute()
    {
        UpdateMgr.Instance.Registener(this);
    }

    public override void Finish()
    {
        Entity3DMgr.Instance.SetEntityMoveSpeedIncrements(GetEntityID(), -_LastSpeedIncrement);
        UpdateMgr.Instance.Unregistener(this);
        _IsFinish = false;
        _NextPoint = Vector3.zero;
        _LastPos = Vector3.zero;
        _BugCount = 0;
    }

    public override bool IsBreak()
    {
        return true;
    }



    public override bool IsNextModule()
    {
        return _IsFinish;
    }

    public override bool IsExecute()
    {
        return true;
    }

    public Vector3 CreatePoint()
    {
        var radius = _RangeParams[0] / 100f;
        var angle = Random.Range(0, Mathf.PI * 2);
        var x = Mathf.Cos(angle);
        var z = Mathf.Sin(angle);

        var localPos = new Vector3(x, 0, z);
        var dis = Random.Range(0, radius);
        var point = _CenterPos + localPos * dis;
        return point;
    }

    public void Update()
    {
        if (_IsFinish)
            return;

        var entityID = GetEntityID();
        var curPos = Entity3DMgr.Instance.GetEntityWorldPos(entityID);
        if (Vector3.SqrMagnitude(curPos - _NextPoint) < 0.1f)
        {
            _IsFinish = true;
            return;
        }
        if (Vector3.SqrMagnitude(curPos - _LastPos) < 0.00001f)
        {
            if (_BugCount++ > 10)
            {
                _IsFinish = true;
            }
        }
        else
        {
            _BugCount = 0;
        }
        _LastPos = curPos;
        var dir = _NextPoint - curPos;
        Entity3DMgr.Instance.IncrementSetEntityMoveDirection(entityID, dir.normalized);
        Entity3DMgr.Instance.SetEntityLookAtDirection(entityID, dir.normalized);
    }
}
