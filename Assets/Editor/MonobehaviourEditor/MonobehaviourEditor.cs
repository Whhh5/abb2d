using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DG.DemiEditor;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(MonoBehaviour), true)]
public class MonobehaviourEditor : Editor
{
    private MonoBehaviour _Mono = null;
    Dictionary<FieldInfo, AutoReferenceAttribute> allFieldList = new();
    private void Awake()
    {
    }
    private void OnEnable()
    {
        _Mono = target as MonoBehaviour;

        UpdateAllFieldList();
        SetAllValue();
    }
    public override void OnInspectorGUI()
    {
        //if (allFieldList.Count > 0)
        //{
        //    if (GUILayout.Button("UpdateAllFieldList"))
        //    {
        //        UpdateAllFieldList();
        //        SetAllValue();
        //    }
        //}
        base.OnInspectorGUI();
    }

    private void UpdateAllFieldList()
    {
        allFieldList = new Dictionary<FieldInfo, AutoReferenceAttribute>();

        var type = _Mono.GetType();
        while (type != null)
        {
            var fieldInfoList = type.GetFields((BindingFlags)int.MaxValue);
            foreach (var item in fieldInfoList)
            {
                var att = item.GetCustomAttribute<AutoReferenceAttribute>();
                if (att == null)
                    continue;
                allFieldList.Add(item, att);
            }
            type = type.BaseType;
        }
    }
    private void SetAllValue()
    {

        foreach (var item in allFieldList)
        {
            var objName = string.IsNullOrEmpty(item.Value.objName)
                ? item.Key.Name
                : item.Value.objName;
            var obj = FindChild(objName, _Mono.transform);
            if (obj == null)
                continue;
            if (item.Key.FieldType.IsValueType)
                continue;
            var curValue = item.Key.GetValue(_Mono);
            if (item.Key.FieldType == typeof(GameObject))
            {
                item.Key.SetValue(_Mono, obj.gameObject);
                if (curValue as GameObject != obj.gameObject)
                {
                    AssetDatabase.SaveAssetIfDirty(_Mono);
                }
                continue;
            }
            var com = obj.GetComponent(item.Key.FieldType);
            if (com == null)
                continue;
            item.Key.SetValue(_Mono, com);
            if (curValue != (object)com)
            {
                
            }
        }
        AssetDatabase.SaveAssetIfDirty(_Mono);
    }

    private Transform FindChild(string name, Transform root)
    {
        if (string.Equals(root.name, name))
            return root;
        for (int i = 0; i < root.childCount; i++)
        {
            var childObj = root.GetChild(i);
            if (string.Equals(childObj.name, name))
                return childObj;

            var child0 = FindChild(name, childObj);
            if (child0 != null)
                return child0;
        }
        return null;
    }
}