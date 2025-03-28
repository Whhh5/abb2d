
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using static Codice.CM.WorkspaceServer.WorkspaceTreeDataStore;

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

    public static void DrawEnum(ref Enum defValue)
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

    static GUIContent _ClipContent = new GUIContent()
    {
        text = "clip ID",
        tooltip = "ClipCfg -> id",
    };
    static public void DrawClipID(int clipID, Action<int> selectID)
    {
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField(_ClipContent, GUILayout.Width(50));

            var content = new GUIContent()
            {
                text = $"{clipID}",
            };
            if (clipID > 0)
            {
                var clipCfg = ExcelUtil.GetCfg<ClipCfg>(clipID);
                if (clipCfg != null)
                {
                    var assetCfg = ExcelUtil.GetCfg<AssetCfg>(clipCfg.nAssetID);
                    var asset = AssetDatabase.LoadAssetAtPath<AnimationClip>(assetCfg.strPath);
                    content.text += $"-{asset.name}";

                    EditorGUILayout.ObjectField(asset, asset.GetType(), false, GUILayout.Width(100));
                }
                else
                {
                    content.text += "-Error";
                }
            }
            if (GUILayout.Button(content, GUILayout.Width(200)))
            {
                var menu = new GenericMenu();

                var clipCount = ExcelUtil.GetCfgCount<ClipCfg>();
                for (int j = 0; j < clipCount; j++)
                {
                    var item = ExcelUtil.GetCfgByIndex<ClipCfg>(j);
                    var itemAssetCfg = ExcelUtil.GetCfg<AssetCfg>(item.nAssetID);
                    var itemAsset = AssetDatabase.LoadAssetAtPath<AnimationClip>(itemAssetCfg.strPath);
                    menu.AddItem(new()
                    {
                        text = $"{item.nClipID}-{itemAsset.name}"
                    }, item.nClipID == clipID, () =>
                    {
                        selectID(item.nClipID);
                    });
                }
                menu.ShowAsContext();
            }
        }
        EditorGUILayout.EndHorizontal();
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
}