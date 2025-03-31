
using System.Collections.Generic;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using UnityEditor;
using UnityEngine;

public class SkillBuffScheduleActionEditor : SkillBuffScheduleAction, ISkillScheduleActionEditor
{
    private EnBuff buff => (EnBuff)buffID;
    private IBuffDaraEditor _IBuffDaraEditor = null;
    public void InitEditor()
    {
        _IBuffDaraEditor = SkillFactroyEditor.GetBuffDataEditor(buff);
        _IBuffDaraEditor?.InitEditor();
        _IBuffDaraEditor?.InitParams(arrBuffParams);
    }
    public void Draw()
    {
        EditorGUILayout.BeginVertical();
        {
            EditorUtil.DrawCfgField<BuffCfg>("buff", buffID, id =>
            {
                if (buffID == id)
                    return;
                _IBuffDaraEditor = SkillFactroyEditor.GetBuffDataEditor((EnBuff)id);
                _IBuffDaraEditor.InitParams(null);

                buffID = id;
            }, 300);


            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("schedule", GUILayout.Width(50));

                var buffCfg = ExcelUtil.GetCfg<BuffCfg>((int)buff);
                if (buffCfg != null)
                {
                    if ((EnBuffType)buffCfg.nBuffType == EnBuffType.Time)
                    {
                        startSchedule = EditorGUILayout.Slider(startSchedule, 0, 1);
                        endSchedule = 0;
                    }
                    else
                    {
                        var rect = GUILayoutUtility.GetRect(new GUIContent(), GUI.skin.box, GUILayout.ExpandWidth(true));
                        EditorUtil.DrawSliderRange(rect, ref startSchedule, ref endSchedule, 0f, 1f);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            _IBuffDaraEditor?.Draw();

        }
        EditorGUILayout.EndVertical();
    }

    public void GetStringData(ref List<int> data)
    {
        var count = data.Count;
        data.Add(Mathf.RoundToInt(startSchedule * 100f));
        data.Add(buffID);
        data.Add(Mathf.RoundToInt(endSchedule * 100f));
        data.Insert(count, data.Count - count);

        //data.Add(arrBuffParams?.Length ?? 0);

        count = data.Count;
        _IBuffDaraEditor?.GetStringData(ref data);
        data.Insert(count, data.Count - count);
    }
    public float GetEnterSchedule()
    {
        return startSchedule;
    }

    public void Sumilation(Rect rect, float itemStartTime, float itemEndTime)
    {
        
    }
}