using UnityEngine;

public class LayerMixerCmdPlayableAdapter : CmdPlayableAdapter
{
    private PlayableAdapter m_CurPlayableAdater = null;
    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
    protected override void Initialization(PlayableGraphAdapter graph, IPlayableAdapterCustomData customData)
    {
        base.Initialization(graph, customData);

        m_CurPlayableAdater = graph.CreateClipPlayableAdapter(81);
        AddConnectRootAdapter(m_CurPlayableAdater);
    }
    public override EnAnimLayer GetOutputLayer()
    {
        return EnAnimLayer.Layer2;
    }
    public override float GetPlayTime()
    {
        return m_CurPlayableAdater.GetPlayTime();
    }
    public override float GetUnitTime()
    {
        return m_CurPlayableAdater.GetUnitTime();
    }
}
