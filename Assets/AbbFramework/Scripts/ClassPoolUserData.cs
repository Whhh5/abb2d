
using UnityEngine;
using UnityEngine.Playables;

public class PoolNaNUserData : IClassPoolUserData
{
    public void OnPoolDestroy()
    {
    }
}
public class ABBEventDataUserData : IClassPoolUserData
{
    public EnABBEvent abbevent;

    public void OnPoolDestroy()
    {
        abbevent = EnABBEvent.NONE;
    }
}
public class Entity3DComDataUserData : IClassPoolUserData
{
    public int entityID;
    public void OnPoolDestroy()
    {
        entityID = -1;
    }
}
public class EntityBuffDataUserData : IClassPoolUserData
{
    public int targetEntityID;
    public int sourceEntityID;
    public EnBuff buff;
    public void OnPoolDestroy()
    {
        targetEntityID = -1;
        buff = EnBuff.None;
    }
}
public class LayerMixerInfoUserData : IClassPoolUserData
{
    public EnAnimLayer layer;
    public ScriptPlayable<BridgePlayableAdapter> layerAdapter;
    public void OnPoolDestroy()
    {
        layer = EnAnimLayer.None;
        layerAdapter = ScriptPlayable<BridgePlayableAdapter>.Null;
    }
}
public class PlayableAdapterUserData : IClassPoolUserData
{
    public PlayableGraphAdapter graph;
    public IPlayableAdapterCustomData customData;

    public virtual void OnPoolDestroy()
    {
        graph = null;
        customData = null;
    }
}
public class PlayableGraphAdapterUserData : IClassPoolUserData
{
    public int entityID;
    public Animator anim;

    public void OnPoolDestroy()
    {
        entityID = -1;
        anim = null;
    }
}
public class GODataUserData : IClassPoolUserData
{
    public GameObject go;
    public EnLoadTarget target;

    public void OnPoolDestroy()
    {
        go = null;
        target = EnLoadTarget.None;
    }
}
public class AttackLinkSkillDataUserData : AttackLinkUserData
{
    public int[] arrParams;

    public override void OnPoolDestroy()
    {
        arrParams = null;
    }
}


public class CommonArrayUserData : IClassPoolUserData
{
    public int[] value;
    public void OnPoolDestroy()
    {
        value = null;
    }
}

public class EntityBuffExposion2UserData : IEntityBuffParams
{
    //public 
    public void OnPoolDestroy()
    {
        
    }

    public void OnPoolInit(CommonArrayUserData userData)
    {
        
    }
}
public class EntityBuffTimeDefaultInfo : IEntityBuffParams, IBuffTimeInfo
{
    private float _Time = -1;
    public float GetTime()
    {
        return _Time;
    }

    public void OnPoolInit(CommonArrayUserData userData)
    {
        var count = userData.value[0];
        _Time = userData.value[1] / 100f;
    }

    public void OnPoolDestroy()
    {
        _Time = -1;
    }

}
public class EntityMoveDownBuffUserData : IEntityBuffParams
{
    public float value;


    public void OnPoolInit(CommonArrayUserData userData)
    {
        var count = userData.value[0];
        value = userData.value[1] / 100f;
    }

    public void OnPoolDestroy()
    {
        value = -1;
    }
}

public abstract class AttackLinkUserData : IClassPoolUserData
{
    public abstract void OnPoolDestroy();
}

public class MonsterBaseDataUserData: EntityDataUserData
{
    public int monsterID;
}
public abstract class EntityDataUserData : IClassPoolUserData
{
    public virtual void OnPoolDestroy()
    {

    }
}
public abstract class PlayableBehaviourAdapterUserData : IClassPoolUserData
{
    public virtual void OnPoolDestroy()
    {

    }
}
public abstract class CmdPlayableAdapterUserData : PlayableAdapterUserData
{

}

public class SkillBehaviourUserData : IClassPoolUserData
{
    public int[] arrValue;
    public void OnPoolDestroy()
    {
        arrValue = null;
    }
}
public sealed class GeneralIntUserData : IClassPoolUserData
{
    public int intValue;
    public void OnPoolDestroy()
    {
        intValue = -1;
    }
}
public sealed class GeneralInt2UserData : IClassPoolUserData
{
    public int intValue;
    public int intValue2;
    public void OnPoolDestroy()
    {
        intValue
            = intValue2
            = -1;
    }
}

public class AIModuleUserData : IClassPoolUserData
{
    public int entityID;
    public int aiModuleCfgID;
    public int moduleDataID;
    public virtual void OnPoolDestroy()
    {
        aiModuleCfgID
            = entityID
            = moduleDataID
            = -1;
    }
}
public sealed class AIRandomMoveModuleUserData : AIModuleUserData
{
    public EnAIRangeType rangeType = EnAIRangeType.None;
    public int[] typeParams = null;
    public Vector3 centerPos;
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        rangeType = EnAIRangeType.None;
        typeParams = null;
        centerPos = Vector3.zero;
    }
}

public class EffectDataUserData : IClassPoolUserData
{
    public int effctCfgID;
    public void OnPoolDestroy()
    {
        
    }
}

