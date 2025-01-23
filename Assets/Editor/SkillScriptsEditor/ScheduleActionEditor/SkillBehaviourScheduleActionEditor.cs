
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SkillBehaviourScheduleActionEditor : SkillBehaviourScheduleAction, ISkillScheduleActionEditor
{
    private ISkillBehaviourEditor _SkillBehaviourEditor = null;

    public void InitEditor()
    {
        var type = SkillFactroyEditor.GetSkillBehaviourEditor(behaviourType);
        _SkillBehaviourEditor = EditorUtil.Copy<ISkillBehaviourEditor>(_SkillBehaviour, type);
        _SkillBehaviourEditor.InitEditor();
    }
    public void Draw()
    {
        EditorGUILayout.BeginVertical();
        {
            schedule = EditorGUILayout.Slider(schedule, 0, 1, GUILayout.Width(200));
            behaviourType = (EnSkillBehaviourType)EditorGUILayout.EnumPopup(behaviourType, GUILayout.Width(100));
            _SkillBehaviourEditor.Draw();
        }
        EditorGUILayout.EndVertical();
    }

    public void GetStringData(ref List<int> data)
    {
        var index = data.Count;
        data.Add(Mathf.RoundToInt(schedule * 100));
        data.Add((int)behaviourType);

        data.Insert(index, data.Count - index);

        index = data.Count;
        _SkillBehaviourEditor.GetStringData(ref data);
        data.Insert(index, data.Count - index);
    }
}