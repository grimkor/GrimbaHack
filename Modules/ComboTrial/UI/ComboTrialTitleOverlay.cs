using epoch.db;
using GrimbaHack.Utility;
using nway.gameplay.match;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace GrimbaHack.Modules.ComboTrial.UI;

public class ComboTrialTitleOverlay
{
    public static readonly ComboTrialTitleOverlay Instance = new();
    private GameObject _rootGameObject;
    private GameObject _overlayObject;
    private string _title;

    static ComboTrialTitleOverlay()
    {
        OnEnterMainMenuActionHandler.Instance.AddCallback(() => { Instance.Teardown(); });
    }

    public void Teardown()
    {
        if (Instance._overlayObject != null)
        {
            Object.DestroyObject(Instance._overlayObject);
            Instance._overlayObject = null;
        }
    }

    public void Init(string title)
    {
        Instance.Teardown();
        Instance._title = title;
        Instance.RenderRootObject();
    }

    public void Show()
    {
        if (!ComboTrialManager.Instance.IsComboTrial) return;
        if (GameManager.instance.appStateManager.state != AppState.Combat &&
            MatchManager.instance.matchType != MatchType.Training) return;

        Instance._overlayObject.SetActive(true);
    }

    private void RenderRootObject()
    {
        Instance._rootGameObject = new("grimui_combotrial_titleGO");
        Object.DontDestroyOnLoad(Instance._rootGameObject);
        var canvas = Instance._rootGameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        Instance._rootGameObject.AddComponent<CanvasScaler>();

        Instance._overlayObject = new GameObject("grimui_combotrial_title");
        Instance._overlayObject.transform.SetParent(Instance._rootGameObject.transform);
        Instance._overlayObject.AddComponent<HorizontalLayoutGroup>();

        var rootSizeFitter = Instance._overlayObject.AddComponent<ContentSizeFitter>();
        rootSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        rootSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        var background = new GameObject("grimui_overlay_background");
        background.transform.SetParent(Instance._overlayObject.transform);
        var layoutElement = background.AddComponent<LayoutElement>();
        layoutElement.ignoreLayout = true;

        var bgRect = background.GetComponent<RectTransform>();
        if (bgRect != null)
        {
            bgRect.anchorMin = new(0.5f, 0);
            bgRect.anchorMax = new(0.5f, 1);
            bgRect.localPosition = new(0, 0, 0);
        }

        var leftBg = new GameObject("grimui_overlay_bg_left");
        leftBg.transform.SetParent(background.transform);
        var leftBgImage = leftBg.AddComponent<Image>();
        leftBgImage.sprite = SpriteMap.Instance.GetMapping("panel_streak");
        leftBgImage.type = Image.Type.Sliced;
        leftBgImage.color = new(1, 1, 1, 0.7f);
        var leftBgRect = leftBg.GetComponent<RectTransform>();
        if (leftBgRect != null)
        {
            leftBgRect.pivot = new(0, 0.5f);
            leftBgRect.anchorMin = new(0.5f, 0);
            leftBgRect.anchorMax = new(0.5f, 1);
            leftBgRect.offsetMin = new(0, 30);
            leftBgRect.offsetMax = new(400, -30);
            leftBgRect.localScale = new(-1, 1, 1);
        }

        var rightBg = new GameObject("grimui_overlay_bg_right");
        rightBg.transform.SetParent(background.transform);
        var rightBgImage = rightBg.AddComponent<Image>();
        rightBgImage.sprite = SpriteMap.Instance.GetMapping("panel_streak");
        rightBgImage.type = Image.Type.Sliced;
        rightBgImage.color = new(57, 163, 1, 0.7f);
        var rightBgRect = rightBg.GetComponent<RectTransform>();
        if (rightBgRect != null)
        {
            rightBgRect.pivot = new(0, 0.5f);
            rightBgRect.anchorMin = new(0.5f, 0);
            rightBgRect.anchorMax = new(0.5f, 1);
            rightBgRect.offsetMin = new(0, 30);
            rightBgRect.offsetMax = new(400, -30);
            rightBgRect.localScale = new(1, 1, 1);
        }

        var textContainer = new GameObject("grimui_title_text_container");
        textContainer.transform.SetParent(Instance._overlayObject.transform);
        var text = textContainer.AddComponent<Text>();
        text.text = Instance._title;
        text.font = FontAssetManager.Instance.SuperFont;
        text.fontSize = 24;

        var rect = Instance._overlayObject.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.localPosition = new(0, 0, 0);
            rect.anchoredPosition = new(0,0);
            rect.anchorMin = new(0.5f, 1);
            rect.anchorMax = new(0.5f, 1);
            rect.pivot = new(0.5f, 1);
            rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 10, rect.sizeDelta.y);
        }
    }
}
