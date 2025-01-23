using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EntityAICfg : ICfg
{
    public int nAiID;
    public int[] arrAIParams;
}
public class EntityAIInfo
{

}
public enum EnAIStatus
{
    Idle,
    Battle,

}
public enum EnAIBehaviourType
{
    Random,
    Attack,
    Skill,
}

[EditorFieldName("ai行为")]
public interface IAIBehaviour : IClassPool<CommonSkillItemParamUserData>
{
    public EnAIBehaviourType GetAIBehaviourType();
    public int[] ExecuteBehaviour(int entityID);

    void IClassPool.PoolConstructor() { }
    void IClassPool.PoolRelease() { }
    void IClassPool.OnPoolEnable() { }
}

public class T: AIRandomBehaviour
{
    private int _pRandomRadius;
    private int _pLayer;
}
[EditorFieldName("范围目标查找")]
public class AIRandomBehaviour : IAIBehaviour
{
    public EnAIBehaviourType GetAIBehaviourType() => EnAIBehaviourType.Random;
    private int _EnittyID;

    private float _pRadius;
    private int _pLayer;
    public void OnPoolDestroy()
    {
        _pRadius
            = _EnittyID
            = _pLayer
            = -1;
    }

    public void OnPoolInit(CommonSkillItemParamUserData userData)
    {
        var startIndex = userData.startIndex;
        var endIndex = startIndex + userData.paramCount;
        var arrParams = userData.arrParams;

        _pRadius = startIndex >= endIndex ? default : (arrParams[startIndex++] / 100f);
        _pLayer = startIndex >= endIndex ? default : arrParams[startIndex++];
    }

    public int GetTargetEntityID()
    {
        return _EnittyID;
    }
    public bool IsExistTarget()
    {
        return _EnittyID > 0;
    }

    public int[] ExecuteBehaviour(int entityID)
    {
        return null;
    }
}
public class AIAttackBehaviour : IAIBehaviour
{
    public EnAIBehaviourType GetAIBehaviourType() => EnAIBehaviourType.Attack;

    public void OnPoolDestroy()
    {
    }

    public void OnPoolInit(CommonSkillItemParamUserData userData)
    {
    }
    public int[] ExecuteBehaviour(int entityID)
    {
        return null;
    }


}
public class EntityAIMgr : Singleton<EntityAIMgr>
{
    private Dictionary<int, EntityAIInfo> _AiInfo = new();

    public void Register(int entityID)
    {

    }
    public void Unregister(int entityID)
    {

    }
}
