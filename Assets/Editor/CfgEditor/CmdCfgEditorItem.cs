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
        var cmdCfgList = ExcelUtil.ReadEditorCfgList<CmdCfg>();
        _TempCmdCfg = ExcelUtil.CreateTypeInstance<CmdCfg>();
        var id = cmdCfgList.Count + 1;
        for (int i = 0; i < cmdCfgList.Count; i++)
        {
            if (cmdCfgList.FindIndex(item => item.nCmdID == i + 1) < 0)
            {
                id = i + 1;
                break;
            }
        }
        ExcelUtil.SetCfgValue(_TempCmdCfg, nameof(_TempCmdCfg.nCmdID), id);
    }

    public void Save()
    {
        var cmdCfgList = ExcelUtil.ReadEditorCfgList<CmdCfg>();
        cmdCfgList.Sort((item, item2) => item.nCmdID < item2.nCmdID ? -1 : 1);
        ExcelUtil.SaveExcel(cmdCfgList);
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
                    ExcelUtil.SetCfgValue(cmdCfg, nameof(cmdCfg.arrParams), str);
                }
                catch { }
            }

        }
        EditorGUILayout.EndHorizontal();
    }
    public void Draw()
    {
        var cmdCfgList = ExcelUtil.ReadEditorCfgList<CmdCfg>();

        EditorGUILayout.BeginVertical();
        {
            var menuRect = EditorGUILayout.BeginVertical();
            EditorGUI.DrawRect(menuRect, new Color(1, 1, 1, 0.1f));
            {
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Create", GUILayout.Width(100), GUILayout.ExpandWidth(false)))
                    {
                        cmdCfgList.Insert(0, _TempCmdCfg);
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
                for (int i = 0; i < cmdCfgList.Count; i++)
                {
                    var cmdCfg = cmdCfgList[i];

                    DrawCmdCfg(cmdCfg, new Color(1, 1, 1, i % 2 == 0 ? 0.1f : 0.3f));
                }
            }
            EditorGUILayout.EndVertical();

        }
        EditorGUILayout.EndVertical();
    }
}
