using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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
        if (allFieldList.Count > 0)
        {
            if (GUILayout.Button("UpdateAllFieldList"))
            {
                UpdateAllFieldList();
            }
        }
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
            var obj = _Mono.transform.Find(objName);
            if (obj == null)
                continue;
            if (item.Key.FieldType.IsValueType)
                continue;
            var com = obj.GetComponent(item.Key.FieldType);
            if (com == null)
                continue;
            item.Key.SetValue(_Mono, com);
        }

        EditorUtility.SetDirty(_Mono);
    }
}