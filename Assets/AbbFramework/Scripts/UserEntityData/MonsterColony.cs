using UnityEngine;


public abstract class MonsterColonyData : Entity3DData<MonsterColony>
{
    private int _MonsterColonyID = -1;
    public sealed override EnLoadTarget LoadTarget
    {
        get
        {
            var colonyCfg = GameSchedule.Instance.GetMonsterColonyCfg0(_MonsterColonyID);
            return (EnLoadTarget)colonyCfg.nAssetID;
        }
    }

    public override void OnPoolInit(IClassPoolUserData userData)
    {
        base.OnPoolInit(userData);

        var data = userData as GeneralIntUserData;
        _MonsterColonyID = data.intValue;
    }
    public int GetMonsterColonyID()
    {
        return _MonsterColonyID;
    }
}
public abstract class MonsterColony : Entity3D
{

}
