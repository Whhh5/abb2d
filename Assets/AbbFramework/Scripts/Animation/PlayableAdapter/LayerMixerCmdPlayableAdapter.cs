using UnityEngine;

public class LayerMixerCmdPlayableAdapter : SkillTypePlayableAdapter
{
    private PlayableAdapter m_CurPlayableAdater = null;
    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
    public override void OnPoolInit(PlayableAdapterUserData userData)
    {
        base.OnPoolInit(userData);

        m_CurPlayableAdater = m_Graph.CreateClipPlayableAdapter(81);
        AddConnectRootAdapter(m_CurPlayableAdater);
    }
    public override EnAnimLayer GetOutputLayer()
    {
        return EnAnimLayer.Bottom;
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
