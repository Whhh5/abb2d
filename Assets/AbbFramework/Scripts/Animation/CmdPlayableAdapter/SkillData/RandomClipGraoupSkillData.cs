
using UnityEngine;
using System.Collections;

public class RandomClipGraoupSkillData : ISkillData
{
    protected int[] m_ArrClip;
    public void OnPoolDestroy()
    {
        m_ArrClip = null;
    }

    public void OnPoolEnable()
    {
    }

    public void OnPoolInit<T>(ref T userData) where T : struct, IPoolUserData
    {
        if (userData is not AttackLinkSkillDataUserData user)
            return;
        var data = user.arrParams;
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
