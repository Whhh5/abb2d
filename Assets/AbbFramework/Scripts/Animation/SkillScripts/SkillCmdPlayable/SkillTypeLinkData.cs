using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillTypeLinkData : ISkillTypeData<AttackLinkSkillDataUserData>
{
    protected List<SkillItemInfo> m_DataList = new();
    protected Dictionary<EnBuff, int[]> m_BuffList = new();
    private List<int> _BuffAddKeyList = new();

    private int _Index = 0;
    private SkillItemInfo curAttackData => m_DataList[_Index];

    private PlayableAdapter m_CurClipAdapter = null;
    private int m_PortID = -1;

    private int _EntityID = -1;
    private IPlayableAdapter _MainAdapter = null;
    private PlayableGraphAdapter _Graph = null;
    public void OnPoolDestroy()
    {
        foreach (var item in m_DataList)
        {
            ClassPoolMgr.Instance.Push(item);
        }
        PlayableAdapter.Destroy(m_CurClipAdapter);

        _Index = 0;
        m_DataList.Clear();
        m_BuffList.Clear();
        _BuffAddKeyList.Clear();

        m_CurClipAdapter = null;
        _Graph = null;
        _MainAdapter = null;
        m_PortID
            = _EntityID
            = -1;
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

    public void InitRuntme(IPlayableAdapter mainAdapter)
    {
        _Graph = mainAdapter.GetGraph();
        _MainAdapter = mainAdapter;
        _EntityID = _Graph;
        m_CurClipAdapter = _Graph.CreateClipPlayableAdapter(curAttackData._ClipID);
        m_PortID = _MainAdapter.AddConnectRootAdapter(m_CurClipAdapter, GlobalConfig.Int0, GlobalConfig.Float1);

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
        curAttackData.OnEnable(_EntityID);
    }
    public void OnDisable(int entityID)
    {
        curAttackData.OnDisable(_EntityID);
        foreach (var addKey in _BuffAddKeyList)
        {
            BuffMgr.Instance.RemoveEntityBuff(addKey);
        }
        _BuffAddKeyList.Clear();
    }
    public bool GetIsAutoRemove()
    {
        return curAttackData._IsAutoRemove;
    }
    public float GetPlayTime()
    {
        return m_CurClipAdapter.GetPlayTime();
    }
    public float GetUnitTime()
    {
        return m_CurClipAdapter.GetUnitTime();
    }
    public EnAnimLayer GetOutputLayer()
    {
        return m_CurClipAdapter.GetOutputLayer();
    }

    public void ReExecuteCmd()
    {
        var slider = GetPlaySlider();
        if (slider < curAttackData.atkEndTime)
            return;

        var targetIndex = (_Index + 1) % GetCount();
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
        curAttackData.OnDisable(_EntityID);
        _Index = index;
        curAttackData.OnEnable(_EntityID);
        var fromAdapter = m_CurClipAdapter;
        var toAdapter = _Graph.CreateClipPlayableAdapter(curAttackData._ClipID);

        fromAdapter.Complete();
        _MainAdapter.DisconnectRootAdapter();
        m_CurClipAdapter = _Graph.CreateMixerPlayableAdapter(fromAdapter, toAdapter, GlobalConfig.Float02, MixerComplete);

        _MainAdapter.ConnectRootAdapter(m_PortID, m_CurClipAdapter.GetPlayable(), 0, 1);
    }
    private void MixerComplete(PlayableMixerAdapter mixer, PlayableAdapter frome, PlayableAdapter to)
    {
        mixer.DisconnectAll();
        _MainAdapter.DisconnectRootAdapter(m_PortID);
        PlayableAdapter.Destroy(frome);
        _MainAdapter.ConnectRootAdapter(m_PortID, to.GetPlayable(), GlobalConfig.Int0, GlobalConfig.Int1);
        m_CurClipAdapter = to;
    }
    public void Update()
    {
        if (curAttackData.ScheduleEventIsValid())
        {
            var curAttackItem = curAttackData.GetCurScheduleItem();
            if (_MainAdapter.GetPlaySchedule01() >= curAttackItem.schedule)
            {
                //if (!curAttackItem.GetIsEffect())
                //{
                curAttackItem.onEvent(_EntityID, curAttackItem.userData);
                //curAttackItem.SetIsEffect(true);
                curAttackData.NextEventAction();
                //}
            }
        }
    }
    public float GetCanNextTime()
    {
        return curAttackData.canNextTime;
    }
}
