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
        ExcelUtil.SaveExcel<EffectCfg>();
    }

    private void UpdateTempEffectCfg()
    {
        _TempEffectCfg = ExcelUtil.CreateTypeInstance<EffectCfg>();
        var effectID = ExcelUtil.GetNextIndex<EffectCfg>();
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

            var assetCfg = ExcelUtil.GetCfg<AssetCfg>(effectCfg.nAssetID);
            var maxTime = 0f;
            if (assetCfg != null)
            {
                var ass = AssetDatabase.LoadAssetAtPath<EffectEntity>(assetCfg.strPath);
                if (ass != null)
                {
                    ass.UpdateMaxTime();
                    maxTime = ass.GetSingletonMaxLifeTime();
                }
            }
            ;
            EditorGUILayout.FloatField(maxTime, GUILayout.Width(50));
            if (maxTime != effectCfg.fDelayDestroyTime)
                ExcelUtil.SetCfgValue(effectCfg, nameof(effectCfg.fDelayDestroyTime), maxTime);

            EditorUtil.DrawCfgField<AssetCfg>(effectCfg.nAssetID, selectID =>
            {
                if (selectID != effectCfg.nAssetID)
                    ExcelUtil.SetCfgValue(effectCfg, nameof(effectCfg.nAssetID), selectID);
            }, 200);
        }
        EditorGUILayout.EndHorizontal();
    }
    private void DrawTitle()
    {
        var rect = EditorGUILayout.BeginVertical();
        EditorGUI.DrawRect(rect, new Color(1, 1, 1, 0.1f));
        {
            if (GUILayout.Button("Create", GUILayout.Width(100)))
            {
                ExcelUtil.AddCfg<EffectCfg>(_TempEffectCfg);
                UpdateTempEffectCfg();
            }
            DrawEffectCfgItem(_TempEffectCfg, new Color(0, 1, 1, 0.1f));
        }
        EditorGUILayout.EndVertical();
    }
    private void DrawData()
    {
        var count = ExcelUtil.GetCfgCount<EffectCfg>();

        EditorGUILayout.BeginVertical();
        {
            for (int i = 0; i < count; i++)
            {
                var item = ExcelUtil.GetCfgByIndex<EffectCfg>(i);
                DrawEffectCfgItem(item, new Color(1, 1, 1, i % 2 == 0 ? 0.1f : 0.2f));
                GUILayout.Space(2);
            }
        }
        EditorGUILayout.EndVertical();
    }
}
