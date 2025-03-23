using UnityEngine;

public abstract class MonsterBaseData : Entity3DData
{
    private int _MonsterID = -1;
    private int _EnemyLayer = 0;
    private int _FriendLayer = 0;
    private EnEntityControllerType _EntityControllerType = EnEntityControllerType.None;
    public sealed override EnLoadTarget LoadTarget
    {
        get
        {
            var monsterCfg = GameSchedule.Instance.GetMonsterCfg0(_MonsterID);
            return (EnLoadTarget)monsterCfg.nAssetCfgID;
        }
    }
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        _MonsterID = -1;
        _EnemyLayer
            = _FriendLayer
            = 0;
    }
    public override void OnPoolInit(IClassPoolUserData userData)
    {
        base.OnPoolInit(userData);
        var monsterUserData = userData as MonsterBaseDataUserData;
        _MonsterID = monsterUserData.monsterID;
        var monsterCfg = GameSchedule.Instance.GetMonsterCfg0(_MonsterID);
        for (int i = 0; i < monsterCfg.arrEnemyLayer.Length; i++)
        {
            _EnemyLayer += 1 << monsterCfg.arrEnemyLayer[i];
        }
        for (int i = 0; i < monsterCfg.arrFriendLayer.Length; i++)
        {
            _FriendLayer += 1 << monsterCfg.arrFriendLayer[i];
        }
    }
    public override void OnGOCreate()
    {
        base.OnGOCreate();

        m_Entity.name += $"-{m_EntityID}";
    }
    public int GetMonsterID()
    {
        return _MonsterID;
    }
    public int GetEnemyLayer()
    {
        return _EnemyLayer;
    }
    public int GetFriendLayer()
    {
        return _FriendLayer;
    }
    public void SetEntityControllerType(EnEntityControllerType controllerType)
    {
        _EntityControllerType = controllerType;
    }
    public EnEntityControllerType GetEntityControllerType()
    {
        return _EntityControllerType;
    }
}
public class MonsterBase : Entity3D
{
    [SerializeField]
    private Vector3 _TargetPos;
    public override void LoadCompeletion()
    {
        base.LoadCompeletion();
        //_TargetPos = transform.position;
        //Debug.Log($"entity:{m_EntityData.EntityID}, {Time.frameCount}, {m_EntityData.WorldPos}, {transform.position}");
    }
    public override void SetPosition()
    {
        //base.SetPosition();
        //transform.position = m_EntityData.WorldPos;
    }
}