
using System.Collections.Generic;

public abstract class SkillTypeItem : IEditorItem, ISkillData
{
    public void Draw()
    {
    }

    public void GetStringData(ref List<int> data)
    {
    }

    public void InitEditor()
    {
    }

    public void OnPoolDestroy()
    {
    }

    public void OnPoolEnable()
    {
    }

    public void OnPoolInit<T>(ref T userData) where T : IClassPoolUserData
    {
    }

    public void PoolConstructor()
    {
    }

    public void PoolRelease()
    {
    }
}