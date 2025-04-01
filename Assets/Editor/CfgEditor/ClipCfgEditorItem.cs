using System;
using UnityEditor;
using UnityEngine;

[CfgEditorItemAttribute]
public class ClipCfgEditorItem : ICfgEditorItem
{
    private string _SearchStr = "";
    private Vector2 _ClipListPos = Vector2.zero;
    private int _SelectAssetID = 0;
    private ClipCfg _TempIns = null;

    public string GetMenuName()
    {
        return "ClipCfg";
    }

    private void UpdateNextClipCfg()
    {
        _TempIns = ExcelUtil.CreateTypeInstance<ClipCfg>();
        var id = ExcelUtil.GetNextIndex<ClipCfg>();
        ExcelUtil.SetCfgValue(_TempIns, nameof(_TempIns.nClipID), id);
    }
    public void OnDisable()
    {

    }

    public void OnEnable()
    {
        UpdateNextClipCfg();
    }

    public void Save()
    {
        ExcelUtil.SaveExcel<ClipCfg>();
    }
    private void DrawClip(ClipCfg clipCfg, Color color)
    {
        var itemRect = EditorGUILayout.BeginHorizontal();
        EditorGUI.DrawRect(itemRect, color);
        {
            var assetCfg = ExcelUtil.GetCfg<AssetCfg>(clipCfg.nAssetID);

            EditorGUILayout.LabelField($"{clipCfg.nClipID}", GUILayout.Width(30));
            var ass = assetCfg == null ? null : AssetDatabase.LoadAssetAtPath<AnimationClip>(assetCfg.strPath);
            EditorGUILayout.ObjectField(ass, typeof(AnimationClip), false, GUILayout.Width(100));


            var isIK = EditorGUILayout.Toggle(clipCfg.bIsIK > 0, GUILayout.Width(20)) ? 1 : 0;
            if (isIK != clipCfg.bIsIK)
                ExcelUtil.SetCfgValue(clipCfg, nameof(clipCfg.bIsIK), isIK);

            var length = ass?.length ?? -1;
            EditorGUILayout.LabelField($"{length}:0.00", GUILayout.Width(50));
            if (length != clipCfg.fLength)
                ExcelUtil.SetCfgValue(clipCfg, nameof(clipCfg.fLength), length);

            var nLayer = (int)(EnAnimLayer)EditorGUILayout.EnumPopup((EnAnimLayer)clipCfg.nLayer, GUILayout.Width(100));
            if (nLayer != clipCfg.nLayer)
                ExcelUtil.SetCfgValue(clipCfg, nameof(clipCfg.nLayer), nLayer);
        }
        EditorGUILayout.EndHorizontal();
    }
    public void Draw()
    {
        EditorGUILayout.BeginVertical();
        {
            var rect = EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(rect, new Color(1, 1, 1, 0.1f));
            {
                var searchRect = new Rect(rect)
                {
                    x = rect.position.x + rect.width - 120,
                    y = rect.position.y + 5,
                    width = 100,
                    height = 20,
                };
                _SearchStr = GUI.TextField(searchRect, _SearchStr);
            }
            {
                //var titleRect = GUILayoutUtility.GetRect(new GUIContent(), GUI.skin.box, GUILayout.Width(true), GUILayout.Height(30));

                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorUtil.DrawCfgField<AssetCfg>(
                            _SelectAssetID
                            , value => _SelectAssetID = value
                            , 300);
                        if (_SelectAssetID > 0)
                        {
                            if (GUILayout.Button("Add", GUILayout.Width(100)))
                            {
                                ExcelUtil.AddCfg<ClipCfg>(_TempIns);
                                UpdateNextClipCfg();
                                _SelectAssetID = -1;
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    if (_SelectAssetID > 0)
                    {
                        var assetCfg = ExcelUtil.GetCfg<AssetCfg>(_SelectAssetID);
                        ExcelUtil.SetCfgValue(_TempIns, nameof(_TempIns.nAssetID), _SelectAssetID);
                        DrawClip(_TempIns, new Color(0, 1, 1, 0.1f));
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();


            GUILayout.Space(10);

            var clipCfgCount = ExcelUtil.GetCfgCount<ClipCfg>();
            _ClipListPos = EditorGUILayout.BeginScrollView(_ClipListPos, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            for (int i = 0; i < clipCfgCount; i++)
            {
                var clipCfg = ExcelUtil.GetCfgByIndex<ClipCfg>(i);
                var assetCfg = ExcelUtil.GetCfg<AssetCfg>(clipCfg.nAssetID);
                if (!string.IsNullOrWhiteSpace(_SearchStr))
                {
                    if (!clipCfg.nClipID.ToString().Contains(_SearchStr, StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (assetCfg == null)
                            continue;
                        if (!assetCfg.strPath.Contains(_SearchStr, StringComparison.CurrentCultureIgnoreCase))
                            continue;
                    }
                }
                DrawClip(clipCfg, new Color(1, 1, 1, i % 2 == 0 ? 0.1f : 0.3f));
                GUILayout.Space(2);
            }
            EditorGUILayout.EndScrollView();
        }
        EditorGUILayout.EndVertical();
    }
}