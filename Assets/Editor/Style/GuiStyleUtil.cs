using System;
using System.Collections;
using System.Collections.Generic;
using DG.DOTweenEditor;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public static class GuiStyleUtil
{
    // 创建背景纹理
    private static Texture2D MakeTex(int width, int height, Color col)
    {
        Texture2D texture = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = col;
        }
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
    public static GUIStyle CreateLayoutBoxBackgroud(Color32 col)
    {
        var customBoxStyle = new GUIStyle(GUI.skin.box);
        customBoxStyle.normal.textColor = Color.white;
        customBoxStyle.fontSize = 14;
        customBoxStyle.margin = new RectOffset(5, 10, 5, 10);
        customBoxStyle.padding = new RectOffset(10, 10, 10, 10);
        customBoxStyle.normal.background = MakeTex(1, 1, new Color(col.r / 255f, col.g / 255f, col.b / 255f, col.a / 255f));
        return customBoxStyle;
    }
    public static bool DrawCloseButton(Rect rect)
    {
        var myTexture = EditorLoad.LoadTexture2D(EnEditorRes.btn_close);
        // 使用 Content 创建纹理按钮
        var content = new GUIContent(myTexture);
        // 绘制按钮
        var result = GUI.Button(rect, content);

        return result;
    }
    public static bool DrawCloseButton()
    {
        var myTexture = EditorLoad.LoadTexture2D(EnEditorRes.btn_close);

        // 创建自定义按钮样式
        var buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.alignment = TextAnchor.MiddleCenter;

        // 使用 Content 创建纹理按钮
        GUIContent content = new GUIContent(myTexture);
        // 绘制按钮
        var result = GUILayout.Button(content, buttonStyle, GUILayout.Width(30), GUILayout.Height(30));

        return result;
    }

    public static bool DrawAddButton(Rect rect)
    {
        var myTexture = EditorLoad.LoadTexture2D(EnEditorRes.btn_add);

        // 使用 Content 创建纹理按钮
        var content = new GUIContent(myTexture);
        // 绘制按钮
        var result = GUI.Button(rect, content);

        return result;
    }
    public static bool DrawAddButton()
    {
        var myTexture = EditorLoad.LoadTexture2D(EnEditorRes.btn_add);

        // 创建自定义按钮样式
        var buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.alignment = TextAnchor.MiddleCenter;

        // 使用 Content 创建纹理按钮
        GUIContent content = new GUIContent(myTexture);
        // 绘制按钮
        var result = GUILayout.Button(content, buttonStyle, GUILayout.Width(30), GUILayout.Height(30));

        return result;
    }
    public static bool DrawButton(Rect rect, Color bgColor, string title, Color txtColor)
    {
        var style = new GUIStyle(GUI.skin.button);
        var isBtn = GUI.Button(rect, new GUIContent() { }, style);
        var labelStyle = new GUIStyle(GUI.skin.label)
        {
            normal = { textColor = txtColor },
            hover = { textColor = txtColor },
            active = { textColor = txtColor },
            alignment = TextAnchor.MiddleCenter,
        };
        EditorGUI.DrawRect(rect, bgColor);
        GUI.Label(rect, new GUIContent() { text = title }, labelStyle);
        return isBtn;
    }
    public static bool DrawButton(GUIContent content, GUIStyle style = null, Action closeCB = null, float width = 100, float height = 30)
    {
        var closeT2D = EditorLoad.LoadTexture2D(EnEditorRes.btn_close);

        // 创建自定义按钮样式
        var buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.alignment = TextAnchor.MiddleCenter;

        // 获取按钮的矩形区域
        Rect buttonRect = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.button, GUILayout.Width(width), GUILayout.Height(height)); // 固定宽高的按钮

        var closeSizeX = Mathf.Min(buttonRect.width / 2, buttonRect.height);
        var closeRect = new Rect(buttonRect)
        {
            x = buttonRect.x + buttonRect.width - closeSizeX,
            width = closeSizeX,
        };

        // Rect closeRect = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.button, GUILayout.Width(100), GUILayout.Height(30)); // 固定宽高的按钮

        GUI.enabled = closeCB == null || !closeRect.Contains(Event.current.mousePosition);
        // 绘制按钮
        var result = style == null ? GUI.Button(buttonRect, content) : GUI.Button(buttonRect, content, style);
        GUI.enabled = true;

        var closeContent = new GUIContent(closeT2D);
        if (buttonRect.Contains(Event.current.mousePosition))
        {
            if (closeCB != null && GUI.Button(closeRect, closeContent))
            {
                closeCB?.Invoke();
            }
        }


        return result;
    }

    public static GUIStyle CreateButtonBackground()
    {
        var style = new GUIStyle(GUI.skin.button)
        {
            //normal = { textColor = Color.red },
            //hover = { textColor = Color.green },
            //active = { textColor = Color.black },

            alignment = TextAnchor.MiddleCenter,
            fontSize = 14,
        };

        style.normal.background = EditorLoad.LoadTexture2D(EnEditorRes.btn_blue);
        style.hover.background = EditorLoad.LoadTexture2D(EnEditorRes.btn_red);
        style.active.background = EditorLoad.LoadTexture2D(EnEditorRes.btn_orange);
        style.focused.background = EditorLoad.LoadTexture2D(EnEditorRes.btn_close);

        style.padding = new(1, 1, 1, 1);
        return style;
    }


    public static float DrawSlider(float value, Rect rect)
    {
        var curE = Event.current;
        var mousePos = curE.mousePosition;
        // 进度条的外框
        rect.xMin -= 5;
        rect.xMax -= 5;
        EditorGUI.DrawRect(rect, Color.gray); // 外框颜色

        // 计算填充部分
        float fillWidth = rect.width * value;
        var fillRect = new Rect(rect.x, rect.y, fillWidth, rect.height);
        EditorGUI.DrawRect(fillRect, Color.green); // 填充颜色


        var txtWidth = 30f;
        var txtPaddingY = 2f;
        var txtRect = new Rect(rect)
        {
            x = rect.x + (rect.width - txtWidth) / 2,
            y = rect.y + txtPaddingY / 2,
            height = rect.height - txtPaddingY,
            width = txtWidth,
        };
        var txtStyle = new GUIStyle(GUI.skin.textField)
        {
            normal = { textColor = Color.yellow },

            alignment = TextAnchor.MiddleCenter,
        };

        var areaRect = new Rect(rect)
        {
            width = rect.width + 0.5f,
        };
        var newValeu = EditorGUI.IntField(txtRect, Mathf.RoundToInt(value * 100), txtStyle);
        if (newValeu != Mathf.RoundToInt(value * 100))
        {
            value = Mathf.Clamp(newValeu / 100f, 0, 100);
        }

        if (areaRect.Contains(mousePos))
        {
            switch (curE.type)
            {
                case EventType.MouseDown:
                    {
                        curE.Use();
                    }
                    break;
                case EventType.MouseDrag:
                    {
                        var x = mousePos.x - rect.xMin;
                        value = Mathf.Clamp01(x / rect.width);
                        curE.Use();
                    }
                    break;
                case EventType.MouseUp:
                    {
                        curE.Use();
                    }
                    break;
                default:
                    break;
            }
        }
        return value;
    }
}


public enum EnEditorRes
{
    btn_blue,
    btn_red,
    btn_orange,
    btn_close,
    btn_add,
}
public static class EditorLoad
{
    public static Dictionary<EnEditorRes, string> _ResMap = new()
    {
        { EnEditorRes.btn_blue, "Assets/Editor/ImageEditor/btn_blue.png" },
        { EnEditorRes.btn_close, "Assets/Editor/ImageEditor/btn_close.png" },
        { EnEditorRes.btn_red, "Assets/Editor/ImageEditor/btn_red.png" },
        { EnEditorRes.btn_orange, "Assets/Editor/ImageEditor/btn_orange.png" },
        { EnEditorRes.btn_add, "Assets/Editor/ImageEditor/btn_add.png" },
    };
    public static Texture2D LoadTexture2D(EnEditorRes res)
    {
        var path = _ResMap[res];
        var texture2D = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        return texture2D;
    }
}
