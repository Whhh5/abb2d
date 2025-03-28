using System.Collections.Generic;

public interface IBuffDaraEditor : ISkillTypeEditor, IEditorItemInit
{
    public EnBuff Buff { get; }
}


public abstract class BuffDaraEditor : IBuffDaraEditor
{
    public abstract EnBuff Buff { get; }

    public virtual void Draw()
    {
        
    }

    public virtual void GetStringData(ref List<int> data)
    {
        
    }

    public virtual void InitEditor()
    {
        
    }

    public virtual void InitParams(int[] arrParam)
    {
        
    }
}
