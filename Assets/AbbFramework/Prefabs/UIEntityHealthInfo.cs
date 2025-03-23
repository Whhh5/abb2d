using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;


public class UIEntityHealthInfoDataUserData : IClassPoolUserData
{
    public float time;

    public int fromValue;
    public int toValue;

    public int entityID;

    public void OnPoolDestroy()
    {

    }
}
public class UIEntityHealthInfoData : UIWindowItemData
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_UIEntityHealthInfo;

    public float StartTime { get; private set; }
    public float Time { get; private set; }
    public int FromValue { get; private set; }
    public int ToValuie { get; private set; }

    public int TargetEntityID;

    public bool IsEnd()
    {
        var result = ABBUtil.GetGameTimeSeconds();
        return (StartTime + Time) < result;
    }
    public override void OnPoolInit(IClassPoolUserData userData)
    {
        base.OnPoolInit(userData);

        var data = userData as UIEntityHealthInfoDataUserData;
        FromValue = data.fromValue;
        ToValuie = data.toValue;
        Time = data.time;

        TargetEntityID = data.entityID;
        StartTime = ABBUtil.GetGameTimeSeconds();
    }
    public void UpdateValue(int toValue)
    {
        ToValuie = toValue;
        StartTime = ABBUtil.GetGameTimeSeconds();
    }

    public Vector2 GetUILocalPos()
    {
        var entityData = Entity3DMgr.Instance.GetEntity3DData(TargetEntityID);
        var worldPos = entityData.GetTopPoint();
        var result = UIUtil.WorldToUIPos(worldPos);
        return result;
    }
}
public class UIEntityHealthInfo : UIWindowItem
{
    [SerializeField, AutoReference(typeof(UIEntityHealthInfo))]
    private RectTransform _Rect = null;
    [SerializeField, AutoReference(typeof(UIEntityHealthInfo))]
    private CanvasGroup _CanvasGroup = null;
    [SerializeField, AutoReference]
    private RectTransform _LineRect = null;
    [SerializeField, AutoReference]
    private RectTransform _RedRect = null;
    [SerializeField, AutoReference]
    private RectTransform _GreenRect = null;

    private UIEntityHealthInfoData _UIEntityHealthInfoData = null;
    private float _CurSliderValue = 0;
    private float _MaxRectWidth = 0;

    private List<RectTransform> _LineList = new();

    protected override void Awake()
    {
        base.Awake();
        _LineRect.gameObject.SetActive(false);
    }
    public override void OnUnload()
    {
        base.OnUnload();

        for (int i = 0; i < _LineList.Count; i++)
            GameObject.Destroy(_LineList[i].gameObject);
        _LineList.Clear();
    }
    public override void LoadCompeletion()
    {
        base.LoadCompeletion();
        _UIEntityHealthInfoData = m_EntityData as UIEntityHealthInfoData;
        _CanvasGroup.alpha = 0;
        var maxHealthValue = Entity3DMgr.Instance.GetEntityMaxHealthValue(_UIEntityHealthInfoData.TargetEntityID);
        _CurSliderValue = (float)_UIEntityHealthInfoData.FromValue / maxHealthValue;
        _MaxRectWidth = _Rect.rect.width;

        var lineUnit = 100;
        var count = maxHealthValue / lineUnit;
        count += maxHealthValue % lineUnit == 0 ? -1 : 0;
        for (int i = 0; i < count; i++)
        {
            var lineItem = GameObject.Instantiate(_LineRect, _LineRect.parent);
            var localPos = lineItem.anchoredPosition;
            localPos.x = (i + 1f) * lineUnit / maxHealthValue * _MaxRectWidth;
            lineItem.anchoredPosition = localPos;
            lineItem.gameObject.SetActive(true);
            _LineList.Add(lineItem);
        }
    }


    protected override void Update()
    {
        base.Update();

        UpdateAlpha();

        UpdateLocalPos();

        UpdateSliderAnim();

    }
    private void UpdateLocalPos()
    {
        _Rect.anchoredPosition = _UIEntityHealthInfoData.GetUILocalPos();
    }
    private void UpdateAlpha()
    {
        var showTimeSlider = 0.2f;
        var hideTimeSlider = 0.7f;
        var curTime = ABBUtil.GetGameTimeSeconds();
        var time = _UIEntityHealthInfoData.Time;
        var slider = (curTime - _UIEntityHealthInfoData.StartTime) / time;
        var addAlpha = slider > hideTimeSlider
            ? -1 * time * (1 - hideTimeSlider) * ABBUtil.GetTimeDelta()
            : +1 * time * showTimeSlider * ABBUtil.GetTimeDelta();
        var alpha = Mathf.Clamp01(_CanvasGroup.alpha + addAlpha);
        _CanvasGroup.alpha = alpha;
    }

    private void UpdateSliderAnim()
    {
        var maxHealthValue = Entity3DMgr.Instance.GetEntityMaxHealthValue(_UIEntityHealthInfoData.TargetEntityID);
        var toValueSlider = (float)_UIEntityHealthInfoData.ToValuie / maxHealthValue;
        _CurSliderValue = Mathf.Lerp(_CurSliderValue, toValueSlider, ABBUtil.GetTimeDelta());

        _RedRect.SetSizeWithCurrentAnchors( RectTransform.Axis.Horizontal, _CurSliderValue * _MaxRectWidth);

        _GreenRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, toValueSlider * _MaxRectWidth); 
    }
}
