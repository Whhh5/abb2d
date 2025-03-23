using UnityEngine;

public static class UIUtil
{
    public static Vector2 WorldToUIPos(Vector3 worldPos)
    {
        var canvasRect = UIMgr.Instance.GetCanvasRect();
        var uiCamera = CameraMgr.Instance.GetUICamera();
        var mainCamera = CameraMgr.Instance.GetMainCamera();
        var screenPoint = RectTransformUtility.WorldToScreenPoint(mainCamera, worldPos);
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, uiCamera, out var uiPos))
            return Vector2.zero;
        return uiPos;
    }
}
