
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class EditorUtil
{
    public static object Copy(object source, Type targetType)
    {
        var sourceType = source.GetType();
        var target = Activator.CreateInstance(targetType);


        Dictionary<string, FieldInfo> sourceFieldList = new();

        var tempType = sourceType;
        do
        {
            var arrField = sourceType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default);
            foreach (var item in arrField)
            {
                if (sourceFieldList.ContainsKey(item.Name))
                    continue;
                sourceFieldList.Add(item.Name, item);
            }
            tempType = tempType.BaseType;

        } while (tempType != null);

        Dictionary<string, FieldInfo> targetFieldList = new();
        tempType = targetType;
        do
        {
            var arrField = tempType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default);
            foreach (var item in arrField)
            {
                if (targetFieldList.ContainsKey(item.Name))
                    continue;
                targetFieldList.Add(item.Name, item);
            }
            tempType = tempType.BaseType;

        } while (tempType != null);


        foreach (var item in sourceFieldList)
        {
            if (!targetFieldList.TryGetValue(item.Key, out var fieldInfo))
                continue;
            var sourceValue = item.Value.GetValue(source);
            fieldInfo.SetValue(target, sourceValue);
        }
        var result = target;
        return result;
    }
    public static TResult Copy<TResult>(object source, Type targetType)
        where TResult : class
    {
        var result = Copy(source, targetType);
        return result as TResult;
    }
    public static T Copy<T>(object source)
        where T : class, new()
    {
        var result = Copy<T>(source, typeof(T));
        return result;
    }

    public static string GetClassName(object target)
    {
        var type = target.GetType();
        var att = type.GetCustomAttribute<EditorFieldNameAttribute>();
        var name = att?.fieldName ?? target.ToString();
        return name;
    }
    public static string GetEnumName(Enum target)
    {
        var type = target.GetType();
        var fieldInfo = type.GetField(target.ToString());
        var att = fieldInfo.GetCustomAttribute<EditorFieldNameAttribute>();
        var name = att?.fieldName ?? target.ToString();
        return name;
    }


    public static void DrawLinkHor(float height)
    {
        var space = height - 3f;

        EditorGUILayout.BeginVertical();
        {
            GUILayout.Space(space / 2);
            GUILayout.Button("", GUILayout.Height(3f), GUILayout.ExpandWidth(true));
            GUILayout.Space(space / 2);
        }
        EditorGUILayout.EndVertical();
    }
    public static void DrawLinkVer(float weight)
    {
        var space = weight - 3f;

        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Space(space / 2);
            GUILayout.Button("", GUILayout.Width(3f), GUILayout.ExpandHeight(true));
            GUILayout.Space(space / 2);
        }
        EditorGUILayout.EndHorizontal();
    }

    public static void DrawEnum(Enum defValue)
    {
        var type = defValue.GetType();
        var values = Enum.GetValues(type);
        var menu = new GenericMenu();
        var selectValue = defValue;
        for (int i = 0; i < values.Length; i++)
        {
            var value = values.GetValue(i);
            var eObj = (Enum)value;
            var name = GetEnumName(eObj);
            menu.AddItem(new(name), object.Equals(eObj, defValue), () =>
            {
                selectValue = eObj;
            });
        }
        menu.ShowAsContext();
        defValue = selectValue;
        return;
    }

    static GUIContent _AssetContent = new GUIContent()
    {
        text = "AssetID:",
        tooltip = "AssetCfg -> id",
    };
    static string _SearchAsset = "";
    static public void DrawAssetID<T>(int assetID, Action<int> selectID, Func<AssetCfg, bool> isShow)
        where T : UnityEngine.Object
    {
        EditorGUILayout.BeginHorizontal(GUILayout.Width(250), GUILayout.ExpandWidth(false));
        {
            EditorGUILayout.LabelField(_AssetContent, GUILayout.Width(50));

            var content = new GUIContent()
            {
                text = $"{assetID}",
            };
            if (assetID > 0)
            {
                var assetCfg = ExcelUtil.GetCfg<AssetCfg>(assetID);
                if (assetCfg != null && AssetDatabase.LoadAssetAtPath<T>(assetCfg.strPath))
                {
                    var name = Path.GetFileName(assetCfg.strPath);
                    content.text += $"-{name}";
                }
                else
                {
                    content.text += "-Error";
                }
            }
            else
            {
                content.text += "-Error";
            }

            var rect = GUILayoutUtility.GetRect(new GUIContent(), GUI.skin.box, GUILayout.Width(190), GUILayout.ExpandWidth(false));

            var isShowSearch = rect.Contains(Event.current.mousePosition);
            var btnRect = new Rect(rect)
            {
                width = isShowSearch ? 90 : 190,
            };
            if (isShowSearch)
            {
                var searchRect = new Rect(rect)
                {
                    x = rect.x + 90,
                    width = 100,
                };
                _SearchAsset = GUI.TextField(searchRect, _SearchAsset);
            }
            if (GUI.Button(btnRect, content))
            {
                var menu = new GenericMenu();
                var assetCfgCount = ExcelUtil.GetCfgCount<AssetCfg>();
                for (int j = 0; j < assetCfgCount; j++)
                {
                    var itemAssetCfg = ExcelUtil.GetCfgByIndex<AssetCfg>(j);
                    if (!string.IsNullOrWhiteSpace(_SearchAsset))
                        if (!itemAssetCfg.strPath.Contains(_SearchAsset, StringComparison.CurrentCultureIgnoreCase))
                            continue;

                    if (isShow != null && !isShow(itemAssetCfg))
                        continue;
                    var itemAsset = AssetDatabase.LoadAssetAtPath<T>(itemAssetCfg.strPath);
                    if (itemAsset == null)
                        continue;
                    menu.AddItem(new()
                    {
                        text = $"{itemAssetCfg.nAssetID}-{itemAssetCfg.strDescEditor}",
                    }, itemAssetCfg.nAssetID == assetID, () =>
                    {
                        selectID(itemAssetCfg.nAssetID);
                    });
                }
                menu.ShowAsContext();
            }
        }
        EditorGUILayout.EndHorizontal();
    }


    static public void DrawCfgField<TCfg>(string title, int cfgID, Action<int> selectID, float width)
        where TCfg : ICfg
    {
        EditorGUILayout.BeginHorizontal();
        {
            var titleWidth = width / 3;
            GUILayout.Label(title, GUILayout.Width(titleWidth), GUILayout.ExpandWidth(false));
            var rect = GUILayoutUtility.GetRect(new GUIContent(), GUI.skin.box, GUILayout.Width(width - titleWidth), GUILayout.ExpandWidth(false));
            DrawCfgField<TCfg>(rect, cfgID, selectID);
        }
        EditorGUILayout.EndHorizontal();
    }
    static public void DrawCfgField<TCfg>(int cfgID, Action<int> selectID, int width)
        where TCfg : ICfg
    {
        var rect = GUILayoutUtility.GetRect(new GUIContent(), GUI.skin.box, GUILayout.Width(width), GUILayout.ExpandWidth(false));
        DrawCfgField<TCfg>(rect, cfgID, selectID);
    }
    private static Dictionary<Type, string> _SearchDic = new();
    static public void DrawCfgField<TCfg>(Rect rect, int cfgID, Action<int> selectID)
        where TCfg : ICfg
    {
        EditorGUILayout.BeginHorizontal();
        {
            var content = new GUIContent()
            {
                text = $"{cfgID}",
            };
            var descField = typeof(TCfg).GetField("strDescEditor", (BindingFlags)int.MaxValue);
            var curItem = ExcelUtil.GetCfg<TCfg>(cfgID);
            if (curItem != null)
            {
                var strObj = descField?.GetValue(curItem);
                var str = string.IsNullOrWhiteSpace($"{strObj}") ? "" : $"{strObj}";
                content.text += $"-{str}";
            }
            else
            {
                content.text += "-Error";
            }

            if (!_SearchDic.TryGetValue(typeof(TCfg), out var searchStr))
                _SearchDic.Add(typeof(TCfg), "");

            var isShowSearchTxt = rect.Contains(Event.current.mousePosition);
            var btnWidth = rect.width / (isShowSearchTxt ? 2 : 1);
            var btnRect = new Rect(rect) { width = btnWidth, };

            if (isShowSearchTxt)
            {
                var searchTxtRect = new Rect(rect) { x = rect.x + btnWidth, width = rect.width - btnWidth, };
                _SearchDic[typeof(TCfg)] = GUI.TextField(searchTxtRect, _SearchDic[typeof(TCfg)]);
            }
            if (GUI.Button(btnRect, content))
            {
                var menu = new GenericMenu();

                var count = ExcelUtil.GetCfgCount<TCfg>();
                for (int j = 0; j < count; j++)
                {
                    var item = ExcelUtil.GetCfgByIndex<TCfg>(j);
                    var key = item.GetID();

                    var strObj = descField?.GetValue(item);
                    var str = string.IsNullOrWhiteSpace($"{strObj}") ? "null" : $"{strObj}";

                    if (!string.IsNullOrWhiteSpace(_SearchDic[typeof(TCfg)]))
                        if (!key.ToString().Contains(_SearchDic[typeof(TCfg)], StringComparison.CurrentCultureIgnoreCase))
                            if (!str.Contains(_SearchDic[typeof(TCfg)], StringComparison.CurrentCultureIgnoreCase))
                                continue;

                    menu.AddItem(new()
                    {
                        text = $"{key}-{str}"
                    }, key == cfgID, () =>
                    {
                        selectID(key);
                    });
                }
                menu.ShowAsContext();
            }
        }
        EditorGUILayout.EndHorizontal();
    }
    private static Vector2 _LastMousePos;
    public static void DrawSliderRange(Rect rect, ref float start, ref float end, float min, float max)
    {
        start = Mathf.Clamp(start, min, max);
        end = Mathf.Max(start, Mathf.Min(end, max));

        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.5f));
        var greenRect = new Rect(rect)
        {
            x = rect.xMin + start / (max - min) * rect.width,
            width = (end - start) / (max - min) * rect.width,
        };
        EditorGUI.DrawRect(greenRect, new Color(0, 0.5f, 0, 1f));

        var leftBtnRect = new Rect(rect)
        {
            x = greenRect.xMin - 15,
            width = 30,
        };
        var rightBtnRect = new Rect(rect)
        {
            x = greenRect.xMax - 15,
            width = 30,
        };

        var leftTxtRect = new Rect(rect)
        {
            x = rect.x,
            width = 30,
        };
        var rightTxtRect = new Rect(rect)
        {
            x = rect.xMax - 30,
            width = 30,
        };


        var isEnable = rightBtnRect.Contains(Event.current.mousePosition) || leftBtnRect.Contains(Event.current.mousePosition);
        GUI.enabled = !isEnable;
        if (GUI.enabled)
        {
            var input = Mathf.RoundToInt(start * 100) / 100f;
            var tempStart = EditorGUI.FloatField(leftTxtRect, input, new GUIStyle(GUI.skin.textField) { alignment = TextAnchor.MiddleLeft });
            var isStart = tempStart != input;
            if (isStart)
                start = tempStart;
            var input2 = Mathf.RoundToInt(end * 100) / 100f;
            var tempEnd = EditorGUI.FloatField(rightTxtRect, input2, new GUIStyle(GUI.skin.textField) { alignment = TextAnchor.MiddleRight });
            var isEnd = tempEnd != input2;
            if (isEnd)
                end = tempEnd;
        }
        else
        {
            EditorGUI.FloatField(leftTxtRect, Mathf.RoundToInt(start * 100) / 100f, new GUIStyle(GUI.skin.textField) { alignment = TextAnchor.MiddleLeft });
            EditorGUI.FloatField(rightTxtRect, Mathf.RoundToInt(end * 100) / 100f, new GUIStyle(GUI.skin.textField) { alignment = TextAnchor.MiddleRight });
        }
        GUI.enabled = true;

        GUI.RepeatButton(leftBtnRect, new GUIContent($"{start / (max - min) * 100:0}"));
        GUI.RepeatButton(rightBtnRect, new GUIContent($"{end / (max - min) * 100:0}"));


        if (rightBtnRect.Contains(Event.current.mousePosition))
        {
            switch (Event.current.type)
            {
                case EventType.MouseDown:
                    {
                        Event.current.Use();
                    }
                    break;
                case EventType.MouseDrag:
                    {
                        end += Event.current.delta.x / rect.width * (max - min);
                        //Debug.Log($"        {Event.current.delta}");
                        Event.current.Use();
                    }
                    break;
                case EventType.MouseUp:
                    {
                        Event.current.Use();
                    }
                    break;
                default:
                    break;
            }
        }
        if (leftBtnRect.Contains(Event.current.mousePosition))
        {
            switch (Event.current.type)
            {
                case EventType.MouseDown:
                    {
                        Event.current.Use();
                    }
                    break;
                case EventType.MouseDrag:
                    {
                        start += Event.current.delta.x / rect.width * max;
                        //Debug.Log($"        {Event.current.delta}");
                        Event.current.Use();
                    }
                    break;
                case EventType.MouseUp:
                    {
                        Event.current.Use();
                    }
                    break;
                default:
                    break;
            }
        }
    }
}