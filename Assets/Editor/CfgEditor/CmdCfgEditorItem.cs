using System;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;


public enum EnCmdType
{
    None = 0,
    Skill,
}
[CfgEditorItemAttribute]
public class CmdCfgEditorItem : ICfgEditorItem
{
    private CmdCfg _TempCmdCfg = null;
    public string GetMenuName()
    {
        return "CmdCfg";
    }

    public void OnDisable()
    {

    }

    public void OnEnable()
    {
        UpdateNextTempCmdCfg();
    }
    private void UpdateNextTempCmdCfg()
    {
        _TempCmdCfg = ExcelUtil.CreateTypeInstance<CmdCfg>();
        var id = ExcelUtil.GetNextIndex<CmdCfg>();
        ExcelUtil.SetCfgValue(_TempCmdCfg, nameof(_TempCmdCfg.nCmdID), id);
    }

    public void Save()
    {
        ExcelUtil.SaveExcel<CmdCfg>();
    }

    private void DrawCmdCfg(CmdCfg cmdCfg, Color color)
    {
        var rect = EditorGUILayout.BeginHorizontal();
        EditorGUI.DrawRect(rect, color);
        {
            GUILayout.Label($"{cmdCfg.nCmdID}", GUILayout.Width(30));

            var nLevel = EditorGUILayout.IntField(cmdCfg.nLevel, GUILayout.Width(50));
            if (nLevel != cmdCfg.nLevel)
                ExcelUtil.SetCfgValue(cmdCfg, nameof(cmdCfg.nLevel), nLevel);

            var nType = (int)(EnCmdType)EditorGUILayout.EnumPopup((EnCmdType)cmdCfg.nType, GUILayout.Width(50));
            if (nType != cmdCfg.nType)
                ExcelUtil.SetCfgValue(cmdCfg, nameof(cmdCfg.nType), nType);

            var listParams = JsonConvert.SerializeObject(cmdCfg.arrParams);
            var str = EditorGUILayout.TextField(listParams, GUILayout.Width(200));
            if (listParams != str)
            {
                try
                {
                    var arr = JsonConvert.DeserializeObject<int[]>(str);
                    ExcelUtil.SetCfgValue(cmdCfg, nameof(cmdCfg.arrParams), arr);
                }
                catch { }
            }

        }
        EditorGUILayout.EndHorizontal();
    }
    public void Draw()
    {
        var cmdCfgCount = ExcelUtil.GetCfgCount<CmdCfg>();
        EditorGUILayout.BeginVertical();
        {
            var menuRect = EditorGUILayout.BeginVertical();
            EditorGUI.DrawRect(menuRect, new Color(1, 1, 1, 0.1f));
            {
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Create", GUILayout.Width(100), GUILayout.ExpandWidth(false)))
                    {
                        ExcelUtil.AddCfg(_TempCmdCfg);
                        UpdateNextTempCmdCfg();
                    }
                }
                EditorGUILayout.EndHorizontal();

                DrawCmdCfg(_TempCmdCfg, new Color(0, 1, 1, 0.1f));
            }
            EditorGUILayout.EndVertical();

            GUILayout.Space(20);

            EditorGUILayout.BeginVertical();
            {
                for (int i = 0; i < cmdCfgCount; i++)
                {
                    var cmdCfg = ExcelUtil.GetCfgByIndex<CmdCfg>(i);

                    DrawCmdCfg(cmdCfg, new Color(1, 1, 1, i % 2 == 0 ? 0.1f : 0.3f));
                }
            }
            EditorGUILayout.EndVertical();

        }
        EditorGUILayout.EndVertical();
    }
}
