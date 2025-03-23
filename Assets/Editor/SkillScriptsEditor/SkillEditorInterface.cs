

using System.Collections.Generic;
using UnityEngine.Playables;

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
public interface ISkillSimulationEditor
{
    public float GetMaxSimulationTime();
    public void InitSimulation(ref PlayableGraph graph);
    public void UpdateSimulation(float time);
    public void DrawSimulation();
}