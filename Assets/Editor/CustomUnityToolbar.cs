
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System;
#if UNITY_2019_1_OR_NEWER
using UnityEngine.UIElements;
#else
using UnityEngine.Experimental.UIElements;
#endif

/// <summary>
/// 扩展Unity的按钮栏
/// </summary>
[InitializeOnLoad]
public static class CustomUnityToolbar
{
    public static Type m_toolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
    public static ScriptableObject m_currentToolbar;

    static CustomUnityToolbar()
    {
        EditorApplication.delayCall += OnUpdate;
    }

    public static void OnUpdate()
    {
        // Relying on the fact that toolbar is ScriptableObject and gets deleted when layout changes
        if (m_currentToolbar == null)
        {
            // Find toolbar
            var toolbars = Resources.FindObjectsOfTypeAll(m_toolbarType);
            m_currentToolbar = toolbars.Length > 0 ? (ScriptableObject)toolbars[0] : null;

            if (m_currentToolbar != null)
            {
                var root = m_currentToolbar.GetType().GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
                var rawRoot = root.GetValue(m_currentToolbar);
                var mRoot = rawRoot as VisualElement;
                RegisterCallback("ToolbarZoneLeftAlign", GUILeft);
                RegisterCallback("ToolbarZoneRightAlign", GUIRight);

                void RegisterCallback(string root, Action cb)
                {
                    var toolbarZone = mRoot.Q(root);

                    var parent = new VisualElement()
                    {
                        style = {
                            flexGrow = 1,
                            flexDirection = FlexDirection.Row,
                        }
                    };
                    var container = new IMGUIContainer();
                    container.onGUIHandler += () =>
                    {
                        cb?.Invoke();
                    };
                    parent.Add(container);
                    toolbarZone.Add(parent);
                }
            }
        }
    }

    private static BaseType baseType = new();
    /// <summary>
    /// 绘制左侧的元素
    /// </summary>
    private static void GUILeft()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Update Load Target"))
        {
            LoadConfigEditor.CreateLoadConfigJson();
        }
        GUILayout.EndHorizontal();
    }

    /// <summary>
    /// 绘制右侧的元素
    /// </summary>
    private static void GUIRight()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear"))
        {
        }
        if (GUILayout.Button("Camera"))
        {
            
        }
        if (GUILayout.Button("Level1"))
        {
        }
        GUILayout.EndHorizontal();
    }
}

public class BaseType
{

}
public class ChildType : BaseType
{

}