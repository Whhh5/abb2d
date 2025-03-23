using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;

public interface ICfgEditorItem
{
    public string GetMenuName();
    public void OnEnable();
    public void OnDisable();
    public void Draw();
    public void Save();
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Constructor)]
public class CfgEditorItemAttribute : Attribute
{
    public Type type;
    public CfgEditorItemAttribute()
    {
        //// 获取当前正在应用此特性的类的类型
        //var stackTrace = new System.Diagnostics.StackTrace();

        //var framsList = stackTrace.GetFrames();

        //for (int i = 0; i < framsList.Count(); i++)
        //{ 
        //    var method2 = stackTrace.GetFrame(i).GetMethod();
        //    var type2 = method2.DeclaringType;
        //}

        //var method = stackTrace.GetFrame(0).GetMethod();
        //var type = method.DeclaringType;

        //// 检查该类是否实现了 IMyInterface 接口
        //if (!typeof(ICfgEditorItem).IsAssignableFrom(type))
        //{
        //    throw new InvalidOperationException($"类 {type.FullName} 必须实现 ICfgEditorItem 接口才能使用 CfgEditorItemAttribute 特性。");
        //}
    }
    public CfgEditorItemAttribute(Type type)
    {
        this.type = type;
    }
}
public class CfgEditorWindow : EditorWindow
{
    private List<ICfgEditorItem> _ItemList = new();
    private int _SelectIndex = -1;

    private void OnEnable()
    {
        Assembly currentAssembly = Assembly.GetExecutingAssembly();
        List<Type> typesWithAttribute = currentAssembly.GetTypes()
            .Where(type => type.GetCustomAttribute<CfgEditorItemAttribute>(true) != null)
            .ToList();
        foreach (var item in typesWithAttribute)
        {
            var newCl = Activator.CreateInstance(item);
            _ItemList.Add(newCl as ICfgEditorItem);
        }
        if (IndexValid(_SelectIndex))
        {
            _ItemList[_SelectIndex].OnEnable();
        }
    }
    private void OnDisable()
    {
        if (IndexValid(_SelectIndex))
        {
            _ItemList[_SelectIndex].OnDisable();
        }
        _ItemList.Clear();
    }
    private bool IndexValid(int index)
    {
        return index >= 0 && index < _ItemList.Count;
    }
    public void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        {
            var leftRect = EditorGUILayout.BeginVertical(GUILayout.Width(200), GUILayout.ExpandWidth(false));
            EditorGUI.DrawRect(leftRect, new Color(1, 1, 1, 0.1f));
            {
                var menuRect = EditorGUILayout.BeginHorizontal();
                EditorGUI.DrawRect(menuRect, new Color(1, 1, 1, 0.1f));
                {
                    if (GUILayout.Button("Save", GUILayout.Width(50)))
                    {
                        if (IndexValid(_SelectIndex))
                        {
                            _ItemList[_SelectIndex].Save();
                        }
                    }
                    if (GUILayout.Button("Save All", GUILayout.Width(50)))
                    {
                        for (int i = 0; i < _ItemList.Count; i++)
                        {
                            _ItemList[i].Save();
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(20);

                for (int i = 0; i < _ItemList.Count; i++)
                {
                    var item = _ItemList[i];
                    var name = item.GetMenuName();
                    var btnContent = new GUIContent()
                    {
                        text = name,
                        image = i == _SelectIndex ? EditorLoad.LoadTexture2D(EnEditorRes.btn_blue) : null
                    };
                    if (GUILayout.Button(btnContent, GUILayout.ExpandWidth(true), GUILayout.Width(100), GUILayout.Height(20)))
                    {
                        if (_SelectIndex != i)
                        {
                            if (IndexValid(_SelectIndex))
                            {
                                _ItemList[_SelectIndex].OnDisable();
                            }
                            item.OnEnable();
                            _SelectIndex = i;
                        }
                    }
                }
            }
            EditorGUILayout.EndHorizontal();


            if (IndexValid(_SelectIndex))
            {
                var item = _ItemList[_SelectIndex];
                item.Draw();
            }
        }
        EditorGUILayout.EndHorizontal();
    }
}
