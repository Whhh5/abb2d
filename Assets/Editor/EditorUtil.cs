
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
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
            var clipCfgList = ExcelUtil.ReadEditorCfgList<ClipCfg>();
            var assetCfgList = ExcelUtil.ReadEditorCfgList<AssetCfg>();
            if (clipID > 0)
            {
                var clipCfg = clipCfgList.Find(value => value.nClipID == clipID);
                if (clipCfg != null)
                {
                    var assetCfg = assetCfgList.Find(value => value.nAssetID == clipCfg.nAssetID);
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

                for (int j = 0; j < clipCfgList.Count; j++)
                {
                    var item = clipCfgList[j];
                    var itemAssetCfg = assetCfgList.Find(value => value.nAssetID == item.nAssetID);
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
            var assetCfgList = ExcelUtil.ReadEditorCfgList<AssetCfg>();
            if (assetID > 0)
            {
                var assetCfg = assetCfgList.Find(value => value.nAssetID == assetID);
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
            if (GUILayout.Button(content, GUILayout.Width(190), GUILayout.ExpandWidth(false)))
            {
                var menu = new GenericMenu();

                for (int j = 0; j < assetCfgList.Count; j++)
                {
                    var itemAssetCfg = assetCfgList[j];
                    if (isShow != null && !isShow(itemAssetCfg))
                        continue;
                    var itemAsset = AssetDatabase.LoadAssetAtPath<T>(itemAssetCfg.strPath);
                    if (itemAsset == null)
                        continue;
                    menu.AddItem(new()
                    {
                        text = $"{itemAssetCfg.nAssetID}-{itemAssetCfg.enName}",
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
}