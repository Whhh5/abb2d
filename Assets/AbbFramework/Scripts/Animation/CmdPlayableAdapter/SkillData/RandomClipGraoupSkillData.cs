
using UnityEngine;
using System.Collections;

public class RandomClipGraoupSkillData : ISkillData<AttackLinkSkillDataUserData>
{
    protected int[] m_ArrClip;

    
    public void OnPoolDestroy()
    {
        m_ArrClip = null;
    }

    public void OnPoolEnable()
    {
    }

    public void OnPoolInit(AttackLinkSkillDataUserData userData)
    {
        var data = userData.arrParams;
        if (data != null)
        {
            m_ArrClip = new int[data.Length];
            data?.CopyTo(0, m_ArrClip, data.Length);
        }
    }

    public void PoolConstructor()
    {
    }

    public void PoolRelease()
    {
    }
}
