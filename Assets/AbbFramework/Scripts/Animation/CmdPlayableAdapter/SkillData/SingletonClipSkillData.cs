

using System;

public class SingletonClipSkillData : ISkillData
{
    protected int[] m_ArrClip;
    public void InitData(int[] data)
    {
        if (data != null)
        {
            m_ArrClip = new int[data.Length];
            data?.CopyTo(0, m_ArrClip, data.Length);
        }
    }

    public void OnPoolDestroy()
    {
        m_ArrClip = null;
    }

    public void OnPoolEnable()
    {
    }

    public void OnPoolInit(CustomPoolData userData)
    {
    }

    public void PoolConstructor()
    {
    }

    public void PoolRelease()
    {
    }
}