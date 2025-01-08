using UnityEngine;

public class LayerMixerCmdPlayableAdapter : CmdPlayableAdapter
{
    private PlayableAdapter m_CurPlayableAdater = null;
    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
    public override void OnPoolInit<T>(ref T userData)
    {
        base.OnPoolInit(ref userData);

        m_CurPlayableAdater = m_Graph.CreateClipPlayableAdapter(81);
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
