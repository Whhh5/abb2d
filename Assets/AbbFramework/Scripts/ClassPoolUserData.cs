
using UnityEngine;
using UnityEngine.Playables;

public struct PoolNaNUserData : IPoolUserData
{

}
public struct ABBEventDataUserData : IPoolUserData
{
    public EnABBEvent abbevent;
}
public struct Entity3DComDataUserData : IPoolUserData
{
    public Entity3DData entity3DData;
}
public struct EntityBuffDataUserData : IPoolUserData
{
    public int entityID;
    public EnBuff buff;
}
public struct LayerMixerInfoUserData : IPoolUserData
{
    public EnAnimLayer layer;
    public ScriptPlayable<AdapterPlayable> layerAdapter;
}
public struct PlayableAdapterUserData : IPoolUserData
{
    public PlayableGraphAdapter graph;
    public IPlayableAdapterCustomData customData;
}
public struct PlayableGraphAdapterUserData : IPoolUserData
{
    public int entityID;
    public Animator anim;
}
public struct GODataUserData : IPoolUserData
{
    public GameObject go;
    public EnLoadTarget target;
}
public struct AttackLinkSkillDataUserData: IPoolUserData
{
    public int[] arrParams;
}
public struct EntityMoveDownBuffParamsUserData: IPoolUserData
{
    public float value;
}