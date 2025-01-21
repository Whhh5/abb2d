

using System.Collections.Generic;

public interface IEditorItemInit
{
    public void InitParams(int[] arrParam);
}
public interface ISkillTypeEditor
{
    public void InitEditor();
    public void GetStringData(ref List<int> data);
    public void Draw();
}