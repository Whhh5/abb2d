
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
    public Entity3DData entity3DData;
    public void OnPoolDestroy()
    {
        entity3DData = null;
    }
}
public class EntityBuffDataUserData : IClassPoolUserData
{
    public int entityID;
    public EnBuff buff;
    public void OnPoolDestroy()
    {
        entityID = -1;
        buff = EnBuff.None;
    }
}
public class LayerMixerInfoUserData : IClassPoolUserData
{
    public EnAnimLayer layer;
    public ScriptPlayable<AdapterPlayable> layerAdapter;
    public void OnPoolDestroy()
    {
        layer = EnAnimLayer.None;
        layerAdapter = ScriptPlayable<AdapterPlayable>.Null;
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
public class AttackLinkSkillDataUserData: AttackLinkUserData
{
    public int[] arrParams;

    public override void OnPoolDestroy()
    {
        arrParams = null;
    }
}
public class EntityMoveDownBuffParamsUserData : IClassPoolUserData
{
    public float value;

    public void OnPoolDestroy()
    {
        value = -1;
    }
}
public abstract class AttackLinkUserData : IClassPoolUserData
{
    public abstract void OnPoolDestroy();
}

public abstract class EntityDataUserData : IClassPoolUserData
{
    public virtual void OnPoolDestroy()
    {

    }
}
public abstract class PlayableBehaviourAdapterUserData: IClassPoolUserData
{
    public virtual void OnPoolDestroy()
    {

    }
}
public abstract class CmdPlayableAdapterUserData: PlayableAdapterUserData
{

}
public class JumpDownCmdPlayableAdapterUserData : IPlayableAdapterCustomData
{
    public int[] arrPArams;
    //public EnComIndex index;
    //public int[] arrClipID;
    public void OnPoolDestroy()
    {
        arrPArams = null;
        //index = EnComIndex.None;
        //arrClipID = null;
    }
}