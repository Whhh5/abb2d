using System;
using UnityEditor;
using UnityEngine;

public class AttributesUtil : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


public class SkillAttribute : Attribute
{
    public EnPhysicsType type;

    public SkillAttribute(EnPhysicsType type)
    {
        this.type = type;
    }
}
public class EnumNameAttribute : Attribute
{
    public string name;

    public EnumNameAttribute(string name)
    {
        this.name = name;
    }
}


public class EditorFieldNameAttribute : Attribute
{
    public string fieldName;
    public EditorFieldNameAttribute(string name)
    {
        fieldName = name;
    }
}



public enum EditorDrawFieldType
{

}
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class EditorDrawFieldAttribute : Attribute
{
    public string fieldName;
    public EditorDrawFieldAttribute(EditorDrawFieldType fieldType)
    {
        
    }
    public void Draw(object value)
    {

    }
}


 
[CustomPropertyDrawer(typeof(AnyTypeAttribute))]
public class AnyTypeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // 根据不同的属性类型，使用不同的EditorGUILayout方法
        switch (property.propertyType)
        {
            case SerializedPropertyType.Integer:
                property.intValue = EditorGUILayout.IntField(label, property.intValue);
                break;
            case SerializedPropertyType.Boolean:
                property.boolValue = EditorGUILayout.Toggle(label, property.boolValue);
                break;
            case SerializedPropertyType.Float:
                property.floatValue = EditorGUILayout.FloatField(label, property.floatValue);
                break;
            case SerializedPropertyType.String:
                property.stringValue = EditorGUILayout.TextField(label, property.stringValue);
                break;
            // 添加更多类型的处理...
            default:
                EditorGUILayout.LabelField(label.text, "Unsupported type");
                break;
        }
    }
}

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
// 自定义一个Attribute，用于标记需要使用AnyTypeDrawer的字段
public class AnyTypeAttribute : PropertyAttribute
{
}
