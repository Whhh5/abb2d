using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTypeLoopData : ISkillTypeData<AttackLinkSkillDataUserData>
{
    protected List<SkillItemInfo> m_DataList = new();
    protected Dictionary<EnBuff, int[]> m_BuffList = new();

    private List<int> _BuffAddKeyList = new();

    //
    private EnCmdStep m_StepIndex = EnCmdStep.None;
    private float m_LastExecuteTime = float.NaN;
    private SkillItemInfo CurAtkLinkItemData => m_DataList[(int)m_StepIndex];
    private IPlayableAdapter _MainAdapter = null;
    private PlayableGraphAdapter _Graph = null;
    private int _EntityID = -1;
    private PlayableAdapter m_PlayableAdapter = null;
    private int m_LoopCount = GlobalConfig.IntM1;

    public void OnPoolDestroy()
    {
        PlayableAdapter.Destroy(m_PlayableAdapter);
        foreach (var item in m_DataList)
        {
            ClassPoolMgr.Instance.Push(item);
        }
        m_DataList.Clear();
        m_BuffList.Clear();
        _BuffAddKeyList.Clear();
        _Graph = null;
        _MainAdapter = null;
        m_PlayableAdapter = null;

        m_LoopCount
            = _EntityID
            = GlobalConfig.IntM1;
        m_StepIndex = EnCmdStep.None;
        m_LastExecuteTime = float.NaN;
    }



    public void OnPoolInit(AttackLinkSkillDataUserData userData)
    {
        var data = userData.arrParams;
        var arrIndex = 0;
        var atkCount = data?[arrIndex++];
        for (int i = 0; i < atkCount; i++)
        {
            var atkData = ClassPoolMgr.Instance.Pull<SkillItemInfo>();
            var arrCount = data[arrIndex++];
            atkData.Init(data, arrCount, ref arrIndex);
            AddAttackData(atkData);
        }

        var buffCount = arrIndex < data?.Length ? data[arrIndex++] : default;
        for (int i = 0; i < buffCount; i++)
        {
            var buff = (EnBuff)data[arrIndex++];
            var paramCount = data[arrIndex++];
            var arrParams = data.Copy(arrIndex, paramCount);
            arrIndex += paramCount;
            m_BuffList.Add(buff, arrParams);
        }
    }
    public void InitRuntime(IPlayableAdapter mainAdapter)
    {
        _MainAdapter = mainAdapter;
        _Graph = _MainAdapter.GetGraph();
        _EntityID = _Graph;


        m_StepIndex = EnCmdStep.Step0;
        m_LastExecuteTime = ABBUtil.GetGameTimeSeconds();

        m_PlayableAdapter = _Graph.CreateClipPlayableAdapter(CurAtkLinkItemData._ClipID);
        _MainAdapter.AddConnectRootAdapter(m_PlayableAdapter);
    }
    public void OnEnable(int entityID)
    {
        foreach (var item in m_BuffList)
        {
            var buffDataParams = BuffUtil.ConvertBuffData(item.Key, item.Value);
            var addKey = BuffMgr.Instance.AddEntityBuff(entityID, entityID, item.Key, buffDataParams);
            BuffUtil.PushConvertBuffData(buffDataParams);
            if (BuffMgr.Instance.GetBuffType(addKey) != EnBuffType.Time)
                _BuffAddKeyList.Add(addKey);
        }
        CurAtkLinkItemData.OnEnable(_EntityID);
    }
    public void OnDisable(int entityID)
    {
        CurAtkLinkItemData.OnDisable(_EntityID);
        foreach (var addKey in _BuffAddKeyList)
        {
            BuffMgr.Instance.RemoveEntityBuff(addKey);
        }
        _BuffAddKeyList.Clear();
    }

    private void AddAttackData(SkillItemInfo atkData)
    {
        m_DataList.Add(atkData);
    }
    private void RemoveAttackData(int index)
    {
        m_DataList.RemoveAt(index);
    }
    private int GetCount()
    {
        return m_DataList.Count;
    }
    public EnAnimLayer GetOutputLayer()
    {
        return m_PlayableAdapter.GetOutputLayer();
    }
    public bool NextAnimLevelComdition()
    {
        return m_StepIndex == EnCmdStep.Step1
            ? false
            : _MainAdapter.GetPlaySchedule01() >= CurAtkLinkItemData.canNextTime;
    }
    public float GetPlayTime()
    {
        return m_PlayableAdapter.GetPlayTime();
    }
    public float GetUnitTime()
    {
        return m_PlayableAdapter.GetUnitTime();
    }
    public int GetPlayCount()
    {
        var time = m_PlayableAdapter.GetPlayTime() / m_PlayableAdapter.GetUnitTime();
        return Mathf.FloorToInt(time);
    }



    public bool CurIsPlayEnd()
    {
        if (!CurAtkLinkItemData._IsAutoRemove)
            return false;
        if (m_StepIndex == EnCmdStep.Step1)
            return false;
        return true;
    }
    public void Reexcute()
    {
        m_LastExecuteTime = ABBUtil.GetGameTimeSeconds();
        if (m_StepIndex + GlobalConfig.Int1 >= (EnCmdStep)GetCount())
            return;
        if (m_StepIndex == EnCmdStep.Step1)
            return;
        if (_MainAdapter.GetPlaySchedule01() < GlobalConfig.Float095)
            return;
        CurAtkLinkItemData.OnDisable(_Graph);
        m_StepIndex++;
        CurAtkLinkItemData.OnEnable(_Graph);
        //var lastAdapter = m_PlayableAdapter;
        //lastAdapter.Complete();
        //m_PlayableAdapter = _Graph.CreateClipPlayableAdapter(CurAtkLinkItemData._ClipID);

        //_MainAdapter.DisconnectRootAdapter();
        //PlayableAdapter.Destroy(lastAdapter);
        //_MainAdapter.ConnectRootAdapter(m_PlayableAdapter);

        var from = m_PlayableAdapter;
        var to = _Graph.CreateClipPlayableAdapter(CurAtkLinkItemData._ClipID);
        from.Complete();
        _MainAdapter.DisconnectRootAdapter();
        m_PlayableAdapter = _Graph.CreateMixerPlayableAdapter(from, to, GlobalConfig.Float02, MixerComplete);
        _MainAdapter.ConnectRootAdapter(m_PlayableAdapter);
    }
    public void CmdEnd()
    {
        CurAtkLinkItemData.OnDisable(_EntityID);
        m_StepIndex++;
        CurAtkLinkItemData.OnEnable(_EntityID);
        var from = m_PlayableAdapter;
        var to = _Graph.CreateClipPlayableAdapter(CurAtkLinkItemData._ClipID);
        from.Complete();
        _MainAdapter.DisconnectRootAdapter();
        m_PlayableAdapter = _Graph.CreateMixerPlayableAdapter(from, to, GlobalConfig.Float02, MixerComplete);
        _MainAdapter.ConnectRootAdapter(m_PlayableAdapter);
    }
    private void MixerComplete(PlayableMixerAdapter mixer, PlayableAdapter from, PlayableAdapter to)
    {
        mixer.DisconnectAll();
        _MainAdapter.DisconnectRootAdapter();
        PlayableAdapter.Destroy(from);
        PlayableAdapter.Destroy(mixer);
        _MainAdapter.ConnectRootAdapter(to);
        m_PlayableAdapter = to;
    }
    public void Update()
    {
        if (m_StepIndex == EnCmdStep.Step1
            && ABBUtil.GetGameTimeSeconds() - m_LastExecuteTime > GlobalConfig.Float02)
        {
            if (m_LoopCount != GetPlayCount())
            {
                CmdEnd();
            }
        }
        else
        {
            var curCRound = GetPlayCount();
            if (m_LoopCount != curCRound)
            {
                CurAtkLinkItemData.ResetAllItemData();
                m_LoopCount = curCRound;
            }
        }


        if (CurAtkLinkItemData.ScheduleEventIsValid())
        {
            var curAttackItem = CurAtkLinkItemData.GetCurScheduleItem();
            var schedule = _MainAdapter.GetPlaySchedule() % GetUnitTime() / GetUnitTime();
            if (schedule > curAttackItem.schedule)
            {
                //if (!curAttackItem.GetIsEffect())
                //{
                curAttackItem.onEvent(_EntityID, curAttackItem.userData);
                //curAttackItem.SetIsEffect(true);
                CurAtkLinkItemData.NextEventAction();
                //}
            }
        }
    }
}
