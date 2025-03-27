using UnityEditor;
using UnityEngine;

[CfgEditorItem]
public class EffectCfgEditorItem : ICfgEditorItem
{
    private EffectCfg _TempEffectCfg = null;
    public string GetMenuName()
    {
        return "EffectCfg";
    }

    public void OnDisable()
    {
    }

    public void OnEnable()
    {
        UpdateTempEffectCfg();
    }

    public void Save()
    {
        var effectCfgList = ExcelUtil.ReadEditorCfgList<EffectCfg>();
        effectCfgList.Sort((item, item2) => item.nEffectID < item2.nEffectID ? -1 : 1);
        ExcelUtil.SaveExcel(effectCfgList);
    }

    private void UpdateTempEffectCfg()
    {
        _TempEffectCfg = ExcelUtil.CreateTypeInstance<EffectCfg>();

        var effectCfg = ExcelUtil.ReadEditorCfgList<EffectCfg>();
        var effectID = effectCfg.Count + 1;
        for (int i = 0; i < effectCfg.Count; i++)
        {
            if (effectCfg.FindIndex(item => item.nEffectID == i + 1) < 0)
            {
                effectID = i + 1;
                break;
            }
        }
        ExcelUtil.SetCfgValue(_TempEffectCfg, nameof(_TempEffectCfg.nEffectID), effectID);
    }
    public void Draw()
    {
        EditorGUILayout.BeginVertical();
        {
            DrawTitle();
            GUILayout.Space(20);
            DrawData();
        }
        EditorGUILayout.EndVertical();
    }
    private void DrawEffectCfgItem(EffectCfg effectCfg, Color color)
    {
        var rect = EditorGUILayout.BeginHorizontal();
        EditorGUI.DrawRect(rect, color);
        {
            GUILayout.Label($"{effectCfg.nEffectID}", GUILayout.Width(30));

            var strDescEditor = EditorGUILayout.TextField(effectCfg.strDescEditor, GUILayout.Width(100));
            if (strDescEditor != effectCfg.strDescEditor)
                ExcelUtil.SetCfgValue(effectCfg, nameof(effectCfg.strDescEditor), strDescEditor);

            EditorUtil.DrawAssetID<GameObject>(effectCfg.nAssetID, selectID =>
            {
                if (selectID != effectCfg.nAssetID)
                    ExcelUtil.SetCfgValue(effectCfg, nameof(effectCfg.nAssetID), selectID);
            }, cfg => true);
        }
        EditorGUILayout.EndHorizontal();
    }
    private void DrawTitle()
    {
        var effectCfgList = ExcelUtil.ReadEditorCfgList<EffectCfg>();
        var rect = EditorGUILayout.BeginVertical();
        EditorGUI.DrawRect(rect, new Color(1, 1, 1, 0.1f));
        {
            if (GUILayout.Button("Create", GUILayout.Width(100)))
            {
                effectCfgList.Add(_TempEffectCfg);
                UpdateTempEffectCfg();
            }
            DrawEffectCfgItem(_TempEffectCfg, new Color(0, 1, 1, 0.1f));
        }
        EditorGUILayout.EndVertical();
    }
    private void DrawData()
    {
        var effectCfgList = ExcelUtil.ReadEditorCfgList<EffectCfg>();

        EditorGUILayout.BeginVertical();
        {
            for (int i = 0; i < effectCfgList.Count; i++)
            {
                DrawEffectCfgItem(effectCfgList[i], new Color(1, 1, 1, i % 2 == 0 ? 0.1f : 0.2f));
                GUILayout.Space(2);
            }
        }
        EditorGUILayout.EndVertical();
    }
}
