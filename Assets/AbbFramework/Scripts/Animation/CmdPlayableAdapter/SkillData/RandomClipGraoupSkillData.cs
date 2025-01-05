
using UnityEngine;
using System.Collections;

public class RandomClipGraoupSkillData : ISkillData
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
}
