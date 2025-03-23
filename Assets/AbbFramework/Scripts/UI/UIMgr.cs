using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum EnUIWindowType
{
    Window,
    Dialog,
    Tips,
    Hint,
}
public class UIMgr : SingletonMono<UIMgr>
{
    [SerializeField]
    private Canvas m_MainCanvas = null;
    [SerializeField]
    private RectTransform _CanvasRect = null;
    [SerializeField]
    private RectTransform m_UIRootRect = null;
    [SerializeField]
    //[SerializableGroup]
    private RectTransform m_UITipsRootRect = null;

    private Dictionary<Type, int> m_Type2EntityDataID = new();
    private Dictionary<EnUIWindowType, RectTransform> m_WindowType2Root = new();
    protected override void Awake()
    {
        base.Awake();
        ShowWindow<UIStartGameWindowData>();
    }

    public RectTransform GetUITipsRootRect()
    {
        return m_UITipsRootRect;
    }
    public RectTransform GetCanvasRect()
    {
        return _CanvasRect;
    }
    public Vector2 GetUISize()
    {
        return m_MainCanvas.GetComponent<RectTransform>().sizeDelta;
    }
    public Camera GetCanvasCamera()
    {
        return m_MainCanvas.worldCamera;
    }
    private T GetWindow<T>()
        where T : UIWindowData
    {
        var type = typeof(T);
        var windowData = GetWindow(type);
        return windowData as T;
    }
    private UIWindowData GetWindow(Type type)
    {
        if (!m_Type2EntityDataID.TryGetValue(type, out var entityID))
            return null;
        var windowData = EntityMgr.Instance.GetEntityData(entityID);
        return windowData as UIWindowData;
    }
    public void ShowWindow<T>(object userData = null)
        where T: UIWindowData, new()
    {
        var type = typeof(T);
        var dataID = EntityMgr.Instance.CreateEntityData<T>();
        m_Type2EntityDataID.Add(type, dataID);
        var windowData = GetWindow<T>();
        windowData.OnShow(userData);

        if (!m_WindowType2Root.TryGetValue(windowData.WindowType, out var root))
            root = m_UIRootRect;
        windowData.SetParentTran(root);

        EntityMgr.Instance.LoadEntity(dataID);
    }
    public void HideWindow<T>()
        where T: UIWindowData
    {
        var type = typeof(T);
        HideWindow(type);
    }
    private void HideWindow(Type type)
    {
        var windowData = GetWindow(type);
        windowData.OnHide();

        m_Type2EntityDataID.Remove(type);
        EntityMgr.Instance.UnloadEntity(windowData.EntityID);
        EntityMgr.Instance.RecycleEntityData(windowData.EntityID);
    }
    public void HideAllWindow()
    {
        var list = new List<Type>();
        foreach (var item in m_Type2EntityDataID.Keys)
            list.Add(item);
        foreach (var item in list)
            HideWindow(item);
    }

    public void AddBtnListener(Button btn, UnityAction action)
    {
        btn.onClick.AddListener(action);
    }
    public void RemoveBtnListener(Button btn, UnityAction action)
    {
        btn.onClick.RemoveListener(action);
    }

    public int CreateWindowItem<T>(RectTransform parent, IClassPoolUserData userData)
        where T: UIWindowItemData, new()
    {
        var entityID = EntityMgr.Instance.CreateEntityData<T>(userData);
        var entityData = EntityMgr.Instance.GetEntityData<T>(entityID);
        entityData.SetParentTran(parent);

        EntityMgr.Instance.LoadEntity(entityID);
        return entityID;
    }
    public void DestroyWindowItem(int entityID)
    {
        EntityMgr.Instance.RecycleEntityData(entityID);
    }
}
