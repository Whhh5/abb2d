using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;


public interface ISkillItem
{

}

public class SkillWindowEditor : EditorWindow
{
    [MenuItem("Tools/SkillWindow %^e")]
    public static void OpenWindow()
    {
        var window = EditorWindow.GetWindow<SkillWindowEditor>();
        window.Show();
    }
    private class ItemInfo
    {
        public bool isShow = true;
        public bool isDelect = false;


        private Dictionary<int, Vector2> m_ScrollPos = new();
        public Vector2 GetScrollPos(int index)
        {
            if (!m_ScrollPos.ContainsKey(index))
                m_ScrollPos.Add(index, new());
            return m_ScrollPos[index];
        }
        public void SetScrollPos(int index, Vector2 pos)
        {
            m_ScrollPos[index] = pos;
        }
    }
    private Dictionary<int, ItemInfo> m_DicItemInfo = new();
    private Dictionary<int, SkillCfgEditor> m_ID2SkillEditor = new();
    private Dictionary<int, IEditorItem> m_DicSkilDrawData = new();
    private int m_CurSelectID = -1;
    private List<int> m_OpenList = new();

    private void OnEnable()
    {
        LoadData();
    }

    private void OnDisable()
    { 
        m_ID2SkillEditor.Clear();
        m_DicItemInfo.Clear();
        m_DicSkilDrawData.Clear();
        m_OpenList.Clear();
    }

    private void LoadData()
    {

        //m_DicSkillItem = GameSchedule.ReadCfg("SkillCfg", typeof(SkillCfgEditor)) as SkillCfgEditor[];

        var catalogObj = GameSchedule.ReadCfg("CfgCatalog", typeof(ExportExcelInfo));
        var excelPath = Path.Combine(ABBUtil.GetUnityRootPath(), "Misc", "Excel", "SkillCfg.xlsx");
        var excelPackage = ExcelUtil.ReadExcel(excelPath);
        var workbook = excelPackage.Workbook;
        var workSheetCount = excelPackage.Workbook.Worksheets.Count();
        var workSheet = excelPackage.Workbook.Worksheets[1];
        var catalogList = catalogObj as ExportExcelInfo[];
        var skillCatalog = Array.Find<ExportExcelInfo>(catalogList, (item) => item.excelInfo.excelName == "SkillCfg");
        var skillEditorType = typeof(SkillCfgEditor);
        var fields = skillEditorType.GetFields(BindingFlags.Public | BindingFlags.Instance);
        for (int i = skillCatalog.excelInfo.dataStartRow; i <= workSheet.Dimension.End.Row; i++)
        {
            if (!workSheet.IsValid(i))
                continue;
            var insSkillCfgEditor = Activator.CreateInstance<SkillCfgEditor>();
            for (int j = 0; j < fields.Length; j++)
            {
                var field = fields[j];
                if (!skillCatalog.field2ColList.TryGetValue(field.Name, out var col))
                    goto next;
                var excelStr = workSheet.GetValue<string>(i, col);
                var jsonStr = JsonConvert.DeserializeObject(excelStr, field.FieldType);
                var value = Convert.ChangeType(jsonStr, field.FieldType);
                field.SetValue(insSkillCfgEditor, value);
            }
            AddSkillItem(insSkillCfgEditor);
        next:;
        }

    }
    public IEditorItem GetAtkItem(int type)
    {
        return type switch
        {
            1 => new AttackLinkDataEditor(),
            2 => new RandomClipGraoupSkillDataEditor(),
            3 => new SingletonClipSkillDataEditor(),
            _ => null,
        };
    }
    private void AddSkillItem(SkillCfgEditor skillCfg)
    {
        var key = skillCfg.nSkillID; 
        m_DicItemInfo.Add(key, new());
        m_ID2SkillEditor.Add(key, skillCfg);

        IEditorItem linkData = GetAtkItem(skillCfg.nType);
        (linkData as ISkillData).InitData(skillCfg.arrParams);
        linkData.InitEditor();
        m_DicSkilDrawData.Add(key, linkData);
    }
    private void AddSkillItem(int skillType)
    {
        var skillCfg = new SkillCfgEditor(); 
        skillCfg.nType = skillType;
        for (int i = 1; i < m_DicItemInfo.Count + 2; i++)
        {
            if (m_DicItemInfo.ContainsKey(i))
                continue;
            skillCfg.nSkillID = i;
            break;
        }
        AddSkillItem(skillCfg);
    }
    private void ReloadData()
    {
        OnDisable();

        OnEnable();
    }

    private void SaveData()
    {
        var list = new List<SkillCfgEditor>(m_DicSkilDrawData.Count);
        var catalogObj = GameSchedule.ReadCfg("CfgCatalog", typeof(ExportExcelInfo));
        var excelPath = Path.Combine(ABBUtil.GetUnityRootPath(), "Misc", "Excel", "SkillCfg.xlsx");
        var excelPackage = ExcelUtil.ReadExcel(excelPath);
        var workbook = excelPackage.Workbook;
        var workSheetCount = excelPackage.Workbook.Worksheets.Count();
        var workSheet = excelPackage.Workbook.Worksheets[1];
        var catalogList = catalogObj as ExportExcelInfo[];
        var skillCatalog = Array.Find<ExportExcelInfo>(catalogList, (item) => item.excelInfo.excelName == "SkillCfg");
        var skillEditorType = typeof(SkillCfgEditor);
        var fields = skillEditorType.GetFields(BindingFlags.Public | BindingFlags.Instance);
        var rowIndex = skillCatalog.excelInfo.dataStartRow;
        foreach (var cfg in m_ID2SkillEditor)
        {
            var key = cfg.Key;
            var item = cfg.Value;
            var itemInfo = m_DicItemInfo[key];
            var drawItem = m_DicSkilDrawData[key];
            if (itemInfo.isDelect)
                continue;
            var listData = new List<int>();
            drawItem.GetStringData(ref listData);
            item.arrParams = listData.ToArray();
            workSheet.SetValue(rowIndex, 1, "*");
            workSheet.Cells[rowIndex, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                if (!skillCatalog.field2ColList.TryGetValue(field.Name, out var col))
                    continue;
                var value = field.GetValue(item);
                var jsonValue = JsonConvert.SerializeObject(value);
                workSheet.SetValue(rowIndex, col, jsonValue);
            }
            rowIndex++;
            //list.Add(skillCfg);
        }
        for (int i = rowIndex; i <= workSheet.Dimension.End.Row; i++)
        {
            for (int j = 1; j <= workSheet.Dimension.End.Column; j++)
            {
                workSheet.SetValue(i, j, null);
            }
        }

        excelPackage.Save();

        var result = EditorUtility.DisplayDialog("excel", "save success", "ok", "open");
        if (!result)
        {
            excelPackage.File.OpenFile();
        }
        excelPackage.Dispose();
    }

    private Vector2 m_ScrollPos1 = Vector2.zero;
    private Vector2 m_ScrollPos2 = Vector2.zero;
    private Vector2 m_ScrollPos3 = Vector2.zero;

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        {

            EditorGUILayout.BeginVertical();
            {

                EditorGUILayout.BeginHorizontal(GUILayout.Width(200));
                {
                    if (GUILayout.Button("保存"))
                    {
                        SaveData();
                    }
                    if (GUILayout.Button("重载"))
                    {
                        ReloadData();
                        m_CurSelectID = -1;
                        m_OpenList.Clear();
                    }
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.Label("skill list");
                EditorGUILayout.BeginHorizontal(GUILayout.Width(200));
                {
                    if (GUILayout.Button("新建"))
                    {

                        var rect = new Rect()
                        {
                            center = Vector2.zero,
                            width = 200,
                        };
                        GUIContent[] contents = new GUIContent[]
                        {
                            new(){ text = "1"},
                            new(){ text = "2"},
                            new(){ text = "3"},
                        };
                        EditorUtility.DisplayCustomMenu(rect, contents, 0, (object userData, string[] options, int selected) =>
                        {
                            var value = int.Parse(options[selected]);
                            AddSkillItem(value);

                        }, "ads");
                    }
                }
                EditorGUILayout.EndHorizontal();
                m_ScrollPos1 = EditorGUILayout.BeginScrollView(m_ScrollPos1, GUILayout.Width(200));
                {
                    foreach (var item in m_ID2SkillEditor)
                    {
                        var key = item.Key;
                        var cfg = item.Value;
                        EditorGUILayout.BeginHorizontal();
                        {
                            var itemInfo = m_DicItemInfo[key];
                            if (itemInfo.isDelect)
                            {
                                GUILayout.Label(cfg.strName);
                            }
                            else
                            {
                                if (GUILayout.Button(cfg.strName))
                                {
                                    m_CurSelectID = key;
                                    m_ScrollPos3 = Vector3.zero;
                                    if (!m_OpenList.Contains(key))
                                        m_OpenList.Add(key);
                                }
                            }
                            if (GUILayout.Button(itemInfo.isDelect ? "❌" : "✅", GUILayout.Width(50)))
                            {
                                itemInfo.isDelect = !itemInfo.isDelect;
                                if (itemInfo.isDelect)
                                {
                                    m_OpenList.Remove(key);
                                    if (m_CurSelectID == key)
                                    {
                                        m_CurSelectID = -1;
                                    }
                                }
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();


            EditorGUILayout.BeginVertical();
            {
                m_ScrollPos2 = EditorGUILayout.BeginScrollView(m_ScrollPos2, GUILayout.Height(30));
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        for (var i = 0; i < m_OpenList.Count; i++)
                        {
                            var skillID = m_OpenList[i];
                            var skillCfg = m_ID2SkillEditor[skillID];
                            var itemInfo = m_DicItemInfo[skillID];

                            if (GUILayout.Button(skillCfg.strName, GUILayout.Width(100), GUILayout.Height(skillID == m_CurSelectID ? 40 : 20)))
                            {
                                m_CurSelectID = skillID;
                                m_ScrollPos3 = Vector3.zero;
                            }
                            if (GUILayout.Button("❌", GUILayout.Width(20)))
                            {
                                if (skillID == m_CurSelectID)
                                {
                                    m_CurSelectID = -1;
                                }
                                m_OpenList.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
                GUILayout.Space(20);


                m_ScrollPos3 = EditorGUILayout.BeginScrollView(m_ScrollPos3);
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        if (m_DicSkilDrawData.TryGetValue(m_CurSelectID, out var linkData))
                        {
                            var cfg = m_ID2SkillEditor[m_CurSelectID];
                            EditorGUILayout.BeginVertical();
                            {
                                EditorGUILayout.BeginHorizontal();
                                {
                                    cfg.strName = EditorGUILayout.TextField(cfg.strName, GUILayout.Width(200));
                                }
                                EditorGUILayout.EndHorizontal();
                                linkData.Draw();
                            }
                            EditorGUILayout.EndVertical();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();
    }
}
