
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SkillBuffScheduleActionEditor : SkillBuffScheduleAction, ISkillScheduleActionEditor
{
    private EnBuff buff => (EnBuff)buffID;
    private IBuffDaraEditor _IBuffDaraEditor = null;
    public void InitEditor()
    {
        _IBuffDaraEditor = SkillFactroyEditor.GetBuffDataEditor(buff);
        _IBuffDaraEditor.InitEditor();
        _IBuffDaraEditor?.InitParams(arrBuffParams);
    }
    public void Draw()
    {
        EditorGUILayout.BeginVertical();
        {
            addSchedule = EditorGUILayout.Slider("schedule:", addSchedule, 0, 1, GUILayout.Width(200));

            EditorUtil.DrawCfgField<BuffCfg>("buff", buffID, id =>
            {
                if (buffID == id)
                    return;
                _IBuffDaraEditor = SkillFactroyEditor.GetBuffDataEditor((EnBuff)id);
                _IBuffDaraEditor.InitParams(null);

                buffID = id;
            }, 300);

            //buffID = EditorGUILayout.IntField("buffID:", buffID, GUILayout.Width(200));
            //var type = SkillFactroyEditor.GetBuffDataEditor(buff);
            _IBuffDaraEditor?.Draw();

        }
        EditorGUILayout.EndVertical();
    }

    public void GetStringData(ref List<int> data)
    {
        var count = data.Count;
        data.Add(Mathf.RoundToInt(addSchedule * 100f));
        data.Add(buffID);
        data.Insert(count, data.Count - count);

        //data.Add(arrBuffParams?.Length ?? 0);

        count = data.Count;
        _IBuffDaraEditor.GetStringData(ref data);
        data.Insert(count, data.Count - count);
    }
}