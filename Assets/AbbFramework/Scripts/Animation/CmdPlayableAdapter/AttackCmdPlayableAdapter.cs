using UnityEngine;
using UnityEngine.Playables;


public class AttackCmdPlayableAdapterData : IPlayableAdapterCustomData
{
    public int[] arrParams;

    public void OnPoolDestroy()
    {
        arrParams = null;
    }

    public void OnPoolEnable()
    {
    }

    public void OnPoolInit<T>(ref T userData) where T : struct, IPoolUserData
    {
    }

    public void PoolConstructor()
    {
    }

    public void PoolRelease()
    {
    }
}
public class AttackCmdPlayableAdapter : CmdPlayableAdapter
{
    private AttackLinkSkillData m_LinkData = null;
    private AttackLinkItemData curAttackData => m_LinkData.GetData(m_Index);
    private PlayableAdapter m_CurClipAdapter = null;
    private int m_Index = -1;
    private int m_PortID = -1;
    protected override void OnDestroy()
    {
        ClassPoolMgr.Instance.Push(m_LinkData);
        m_LinkData = null;
        
        PlayableAdapter.Destroy(m_CurClipAdapter);
        base.OnDestroy();
        m_CurClipAdapter = null;
        m_Index = -1;
        m_PortID = -1;

    }
    public override float GetPlayTime()
    {
        return m_CurClipAdapter.GetPlayTime();
    }
    public override float GetUnitTime()
    {
        return m_CurClipAdapter.GetUnitTime();
    }
    public override void OnPoolInit<T>(ref T userData)
    {
        base.OnPoolInit(ref userData);
        if (userData is not PlayableAdapterUserData playableData)
            return;
        var data = playableData.customData as AttackCmdPlayableAdapterData;

        var skillData = new AttackLinkSkillDataUserData()
        {
            arrParams = data.arrParams,
        };
        m_LinkData = ClassPoolMgr.Instance.Pull<AttackLinkSkillData, AttackLinkSkillDataUserData>(ref skillData);


        m_Index = 0;
        m_CurClipAdapter = m_Graph.CreateClipPlayableAdapter(curAttackData.clipID);
        m_PortID = AddConnectRootAdapter(m_CurClipAdapter, GlobalConfig.Int0, GlobalConfig.Float1);
    }
    public override bool NextAnimLevelComdition()
    {
        return GetPlaySchedule01() > curAttackData.canNextTime;
    }
    public override void ExecuteCmd()
    {
        base.ExecuteCmd();
        m_LinkData.OnEnable(m_Graph);
        curAttackData.OnEnable(m_Graph);
    }
    public override void RemoveCmd()
    {
        curAttackData.OnDisable(m_Graph);
        m_LinkData.OnDisable(m_Graph);

        base.RemoveCmd();
    }
    public override void ReExecuteCmd()
    {
        base.ReExecuteCmd();
        var slider = GetPlaySlider();
        if (slider < curAttackData.atkEndTime)
            return;
        var targetIndex = (m_Index + 1) % m_LinkData.GetCount();

        SetAttackIndex(targetIndex);

    }
    private float GetPlaySlider()
    {
        var curTime = GetPlayTime();
        var maxTime = GetUnitTime();
        var slider = curTime / maxTime;
        return Mathf.Clamp01(slider);
    }

    private void SetAttackIndex(int index)
    {
        curAttackData.OnDisable(m_Graph);
        m_Index = index;
        curAttackData.OnEnable(m_Graph);
        var fromAdapter = m_CurClipAdapter;
        var toAdapter = m_Graph.CreateClipPlayableAdapter(curAttackData.clipID);

        fromAdapter.Complete();
        DisconnectRootAdapter();
        m_CurClipAdapter = m_Graph.CreateMixerPlayableAdapter(fromAdapter, toAdapter, GlobalConfig.Float02, MixerComplete);

        ConnectRootAdapter(m_PortID, m_CurClipAdapter.GetPlayable(), 0, 1);
    }
    private void MixerComplete(PlayableMixerAdapter mixer, PlayableAdapter frome, PlayableAdapter to)
    {
        mixer.DisconnectAll();
        DisconnectRootAdapter(m_PortID);
        PlayableAdapter.Destroy(frome);
        ConnectRootAdapter(m_PortID, to.GetPlayable(), GlobalConfig.Int0, GlobalConfig.Int1);
        m_CurClipAdapter = to;
    }
    public override bool OnPrepareFrame(Playable playable, FrameData info)
    {
        if (!base.OnPrepareFrame(playable, info))
            return false;

        if (curAttackData.ScheduleEventCount > 0)
        {
            var curAttackItem = curAttackData.GetCurScheduleItem();
            if (GetPlaySchedule01() >= curAttackItem.GetEnterSchedule())
            {
                if (!curAttackItem.GetIsEffect())
                {
                    curAttackItem.Enter(m_Graph);
                    curAttackItem.SetIsEffect(true);
                    curAttackData.NextEventAction();
                }
            }
        }
        return true;
    }
}
