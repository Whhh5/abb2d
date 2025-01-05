
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class EditorUtil
{
    public static TResult Copy<TResult>(object source, Type targetType)
        where TResult : class
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
        var result = target as TResult;
        return result;
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
    //public static void DrawInstance(object target)
    //{
    //    var type = target.GetType();
    //    var typeName = GetClassName(target);
    //    var fields = new List<FieldInfo>();

    //    do
    //    {
    //        var temoFields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic) ?? new FieldInfo[0];
    //        fields.AddRange(temoFields);
    //        type = type.BaseType;

    //    } while (type != null);

    //    var vaildFields = new Dictionary<string, FieldInfo>();
    //    foreach (var item in fields)
    //    {
    //        var fieldAtt = item.GetCustomAttribute<AnyTypeAttribute>();
    //        if (fieldAtt == null)
    //            continue;
    //        vaildFields.Add(fieldAtt.fieldName, item);
    //    }
    //    EditorGUILayout.BeginVertical();
    //    {

    //        EditorGUILayout.BeginVertical();
    //        {
    //            EditorGUILayout.LabelField(typeName, GUILayout.Width(100));
    //            EditorGUILayout.BeginHorizontal();
    //            {
    //                EditorGUILayout.Space(30);
    //                EditorGUILayout.BeginVertical();
    //                {
    //                    foreach (var item in vaildFields)
    //                    {
    //                        var value = item.Value.GetValue(target);
    //                        value = EditorGUILayout.field(value, item.Value.FieldType, GUILayout.Width(70));

    //                    }
    //                    for (int i = 0; i < vaildFields.Count; i++)
    //                    {
    //                        var fieldInfo = vaildFields[i];

    //                    }
    //                }
    //                EditorGUILayout.EndVertical();
    //            }
    //            EditorGUILayout.EndHorizontal();
    //        }
    //        EditorGUILayout.EndVertical();
    //    }
    //    EditorGUILayout.EndVertical();

    //}
}
