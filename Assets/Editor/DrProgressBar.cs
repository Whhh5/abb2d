using UnityEngine;
using UnityEditor;

public class MultiProgressBarExample : EditorWindow
{
    private float progress1 = 0.3f; // 进度条 1 的初始进度
    private float progress2 = 0.6f; // 进度条 2 的初始进度
    private float progress3 = 0.8f; // 进度条 3 的初始进度

    [MenuItem("Window/Multi Progress Bar Example")]
    public static void ShowWindow()
    {
        GetWindow<MultiProgressBarExample>("Multi Progress Bar Example");
    }

    private void OnGUI()
    {
        GUILayout.Label("Custom Progress Bars", EditorStyles.boldLabel);

        DrawProgressBar(ref progress1, "Progress Bar 1");
        DrawProgressBar(ref progress2, "Progress Bar 2");
        DrawProgressBar(ref progress3, "Progress Bar 3");

        // 进度条控制按钮
        if (GUILayout.Button("Reset Progress"))
        {
            ResetProgress();
        }

        // 更新进度条按键
        if (GUILayout.Button("Randomize Progress"))
        {
            RandomizeProgress();
        }
    }

    private void DrawProgressBar(ref float progress, string label)
    {
        // 创建一个矩形区域
        Rect progressBarRect = GUILayoutUtility.GetRect(0, 30);

        // 绘制外框
        EditorGUI.DrawRect(progressBarRect, Color.gray);

        // 绘制填充部分
        float fillWidth = progressBarRect.width * progress;
        Rect fillRect = new Rect(progressBarRect.x, progressBarRect.y, fillWidth, progressBarRect.height);
        EditorGUI.DrawRect(fillRect, Color.green);

        // 显示进度百分比
        GUI.Label(progressBarRect, label + ": " + (progress * 100).ToString("F0") + "%", EditorStyles.label);

        // 鼠标输入处理
        HandleMouseInput(progressBarRect, ref progress);
    }

    private void HandleMouseInput(Rect rect, ref float progress)
    {
        Event e = Event.current;

        if (rect.Contains(e.mousePosition))
        {
            switch (e.type)
            {
                case EventType.MouseDrag:
                    // 更新进度值
                    UpdateProgress(e.mousePosition.x, rect, ref progress);
                    e.Use();
                    break;
                case EventType.MouseDown:
                    // 直接使用点击位置更新进度值
                    UpdateProgress(e.mousePosition.x, rect, ref progress);
                    e.Use();
                    break;
            }
        }
    }

    private void UpdateProgress(float mouseX, Rect rect, ref float progress)
    {
        float normalizedX = mouseX - rect.x;
        progress = Mathf.Clamp(normalizedX / rect.width, 0, 1); // 限制进度值在0到1之间
    }

    private void ResetProgress()
    {
        progress1 = 0;
        progress2 = 0f;
        progress3 = 0f;
    }

    private void RandomizeProgress()
    {
        progress1 = Random.Range(0f, 1f);
        progress2 = Random.Range(0f, 1f);
        progress3 = Random.Range(0f, 1f);
    }
}