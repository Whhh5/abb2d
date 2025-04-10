using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

[CfgEditorItem]
public class MonsterControllerCfgEditorItem : ICfgEditorItem
{
    public string GetMenuName()
    {
        return "Monster Ctl";
    }

    private MonsterControllerCfg _TempCfg = null;
    public void OnDisable()
    {
    }

    public void OnEnable()
    {
        _ItemStatus.Clear();
        UpdateTempMonsterController();
    }

    public void Save()
    {
        ExcelUtil.SaveExcel<MonsterControllerCfg>();
    }

    private void UpdateTempMonsterController()
    {
        _TempCfg = ExcelUtil.CreateTypeInstance<MonsterControllerCfg>();
        var id = ExcelUtil.GetNextIndex<MonsterControllerCfg>();
        ExcelUtil.SetCfgValue(_TempCfg, nameof(_TempCfg.nControllerID), id);
    }

    private class ItemInfo
    {
        public int id;
        public bool isShowList = true;
        public Rect rect;
        public MonsterControllerCfg cfg;
    }
    private List<ItemInfo> _ItemStatus = new();

    private string _SearchKeyCode = "";
    float keyCodeWidth = 100;
    float cmdWidth = 100;
    private Rect DrawMonsterControllerItem(MonsterControllerCfg cfg, Color color)
    {
        var rect = EditorGUILayout.BeginHorizontal();
        EditorGUI.DrawRect(rect, color);
        {
            GUILayout.Label($"{cfg.nControllerID}", GUILayout.Width(30));

            var allWidth = 20;
            var addWidth = 20;

            var totalWidth = keyCodeWidth + cmdWidth + allWidth + addWidth;
            var itemRect = GUILayoutUtility.GetRect(new GUIContent(), GUI.skin.box, GUILayout.Width(totalWidth), GUILayout.ExpandWidth(false));

            var index = _ItemStatus.FindIndex(item => item.id == cfg.nControllerID);
            if (index < 0)
            {
                _ItemStatus.Add(new()
                {
                    id = cfg.nControllerID,
                    isShowList = false,
                    rect = itemRect,
                    cfg = cfg,
                });
                _ItemStatus.Sort((item, item2) => item.id > item2.id ? -1 : 1);
            }
            var info = _ItemStatus.Find(item => item.id == cfg.nControllerID);
            var isShowList = info.isShowList;
            info.rect = itemRect;
            if (isShowList)
            {
                info.rect = itemRect;
                var addRect = new Rect(itemRect)
                {
                    x = itemRect.x + itemRect.width - allWidth,
                    width = allWidth
                };
                var addImg = EditorLoad.LoadTexture2D(EnEditorRes.btn_add);
                if (GUI.Button(addRect, addImg))
                {
                    if (cfg.arrParams == null)
                    {
                        ExcelUtil.SetCfgValue(cfg, nameof(cfg.arrParams), new int[2]);
                    }
                    else
                    {
                        var newParam = new int[cfg.arrParams.Length + 2];
                        Array.Copy(cfg.arrParams, newParam, cfg.arrParams.Length);
                        ExcelUtil.SetCfgValue(cfg, nameof(cfg.arrParams), newParam);
                    }
                }
            }
            {
                var showRect = new Rect(itemRect)
                {
                    x = itemRect.x + itemRect.width - allWidth - addWidth,
                    width = allWidth,
                };
                var showImg = EditorLoad.LoadTexture2D(isShowList ? EnEditorRes.btn_orange : EnEditorRes.btn_blue);
                if (GUI.Button(showRect, showImg))
                {
                    info.isShowList = !info.isShowList;
                }
            }

        }
        EditorGUILayout.EndHorizontal();
        return rect;
    }
    public void Draw()
    {
        EditorGUILayout.BeginVertical();
        {
            DrawTitle();
            GUILayout.Space(20);
            DrawCfgList();

            DrawArrList();
        }
        EditorGUILayout.EndVertical();


    }
    private void DrawTitle()
    {
        var rect = EditorGUILayout.BeginVertical();
        EditorGUI.DrawRect(rect, new Color(1, 1, 1, 0.1f));
        {
            if (GUILayout.Button("Create", GUILayout.Width(100)))
            {
                ExcelUtil.AddCfg<MonsterControllerCfg>(_TempCfg);
                UpdateTempMonsterController();
            }
            DrawMonsterControllerItem(_TempCfg, new Color(0, 1, 1, 0.1f));
        }
        EditorGUILayout.EndVertical();
    }
    private void DrawCfgList()
    {
        EditorGUILayout.BeginVertical();
        {
            var count = ExcelUtil.GetCfgCount<MonsterControllerCfg>();
            for (int i = 0; i < count; i++)
            {
                var item = ExcelUtil.GetCfgByIndex<MonsterControllerCfg>(i);
                var rect = DrawMonsterControllerItem(item, new Color(1, 1, 1, i % 2 == 0 ? 0.1f : 0.2f));

                if (rect.Contains(Event.current.mousePosition))
                {
                    rect.x = rect.x + rect.width - 50;
                    rect.width = 50;
                    var texture = EditorLoad.LoadTexture2D(EnEditorRes.btn_close);
                    if (GUI.Button(rect, texture))
                    {

                    }
                }
            }
        }
        EditorGUILayout.EndVertical();
    }

    private void DrawArrList()
    {
        for (var k = 0; k < _ItemStatus.Count; k++)
        {
            var item = _ItemStatus[k];
            var cfg = item.cfg;
            var arr = cfg.arrParams;
            var isShowList = item.isShowList;
            var itemRect = item.rect;
            if (arr == null)
                continue;
                var count = isShowList ? arr.Length : Mathf.Min(arr.Length, 2);
            var height = 20f;
            var interval = 2;
            if (isShowList)
            {
                var bgRect = new Rect(itemRect)
                {
                    height = (count / 2) * (height + interval),
                };
                EditorGUI.DrawRect(bgRect, new Color(1, 0, 1, 0.1f));
            }

            for (int i = 0; i < count; i += 2)
            {
                var index = i;
                var curkeyCode = (KeyCode)arr[index];
                var cmdID = arr[index + 1];
                var keyRect = new Rect(itemRect) { y = itemRect.y + (i / 2) * (height + interval), width = keyCodeWidth, };
                var cmdRect = new Rect(itemRect) { x = itemRect.x + keyCodeWidth, y = keyRect.y, width = cmdWidth, };
                var isContains = keyRect.Contains(Event.current.mousePosition);
                keyRect.width *= isContains ? 0.5f : 1;
                if (GUI.Button(keyRect, $"{curkeyCode}"))
                {
                    var menu = new GenericMenu();
                    for (int j = 0; j < 1000; j++)
                    {
                        if (!Enum.IsDefined(typeof(KeyCode), j))
                            continue;
                        var keyCode = (KeyCode)j;
                        if (!string.IsNullOrWhiteSpace(_SearchKeyCode))
                            if (!keyCode.ToString().Contains(_SearchKeyCode, StringComparison.CurrentCultureIgnoreCase))
                                continue;
                        menu.AddItem(new() { text = $"{keyCode}" }, keyCode == curkeyCode, () =>
                        {
                            if (keyCode == curkeyCode)
                                return;
                            arr[index] = (int)keyCode;
                            ExcelUtil.SetCfgValue(cfg, nameof(cfg.arrParams), arr);
                        });
                    }
                    menu.ShowAsContext();
                }
                if (isContains)
                {
                    var strRect = new Rect(keyRect)
                    {
                        width = keyRect.width,
                        x = keyRect.x + keyRect.width,
                    };
                    _SearchKeyCode = GUI.TextField(strRect, _SearchKeyCode);
                }
                EditorUtil.DrawCfgField<CmdCfg>(cmdRect, cmdID, selectID =>
                {
                    if (selectID == cmdID)
                        return;
                    arr[index + 1] = selectID;
                    ExcelUtil.SetCfgValue(cfg, nameof(cfg.arrParams), arr);
                });
                var closeRect = new Rect(cmdRect)
                {
                    x = cmdRect.x + cmdRect.width,
                    width = 20,
                };
                if (i > 0 && GUI.Button(closeRect, EditorLoad.LoadTexture2D(EnEditorRes.btn_close)))
                {
                    var newArray = arr.ToList();
                    newArray.RemoveRange(index, 2);
                    arr = newArray.ToArray();

                    ExcelUtil.SetCfgValue(cfg, nameof(cfg.arrParams), arr);
                    _ItemStatus.RemoveAt(k);
                }

                GUILayout.Space(interval);
            }
        }
    }
}
