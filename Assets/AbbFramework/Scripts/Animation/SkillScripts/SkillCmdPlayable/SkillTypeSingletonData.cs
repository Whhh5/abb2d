

using System;

public class SkillTypeSingletonData : ISkillTypeData<AttackLinkSkillDataUserData>
{
    protected int[] m_ArrClip;
    //protected SkillItemInfo[] _SkillItemInfoList = new SkillItemInfo[1];
    public void OnPoolDestroy()
    {
        //ClassPoolMgr.Instance.Push(_SkillItemInfoList[0]);
        m_ArrClip = null;
    }

    public void OnPoolInit(AttackLinkSkillDataUserData userData)
    {
        var data = userData.arrParams;
        if (data == null)
            return;
        m_ArrClip = new int[data.Length];
        data.CopyTo(0, m_ArrClip, data.Length);

        //var startIndex = 0;
        //var atkData = ClassPoolMgr.Instance.Pull<SkillItemInfo>();
        //atkData.Init(data, data.Length, ref startIndex);
        //_SkillItemInfoList[0] = atkData;

    }
}