using GrimbaHack.Modules.ComboTrial.UI;
using HarmonyLib;
using nway.gameplay;
using nway.gameplay.ui;
using nway.ui;
using UnityEngine;
using UnityEngine.UI;

namespace GrimbaHack.Modules.ComboTrial;

[HarmonyPatch(typeof(GameManager), nameof(GameManager.RequestUnpauseApp))]
public class PlaybackOverlayText
{
    private static GameObject _containerObject;
    private static GameObject labelGo;
    private static GameObject valueGo;
    private static GameObject _layoutContainer;

    public static bool Enabled
    {
        get => _containerObject?.active == true;
        set => _containerObject.SetActive(value);
    }

    public static void Setup()
    {
        RenderTextOverlay();
    }

    private static void RenderTextOverlay()
    {
        // Check if it's already rendered, if so, don't render again
        if (_containerObject is not null)
        {
            _containerObject.SetActive(true);
            return;
        }

        var canvasGameObject =
            BaseUIManager.instance.Layers[UILayerType.ForegroundOrthographic].rootCanvas.gameObject;


        // Container
        _containerObject = new GameObject();
        _containerObject.transform.SetParent(canvasGameObject.transform);
        _containerObject.transform.localPosition = new Vector3(1, 1, 1);
        _containerObject.transform.localScale = new Vector3(1, 1, 1);
        _containerObject.AddComponent<CanvasRenderer>();
        var containerLayout = _containerObject.AddComponent<VerticalLayoutGroup>();
        containerLayout.childControlHeight = true;
        containerLayout.childControlWidth = false;
        containerLayout.childScaleHeight = false;
        containerLayout.childScaleWidth = false;
        containerLayout.childForceExpandHeight = false;
        containerLayout.childForceExpandWidth = false;
        var containerContentSizeFitter = _containerObject.AddComponent<ContentSizeFitter>();
        containerContentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        containerContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        _layoutContainer = new GameObject();
        _layoutContainer.transform.SetParent(_containerObject.transform);
        _layoutContainer.transform.localPosition = new Vector3(1, 1, 1);
        _layoutContainer.transform.localScale = new Vector3(1, 1, 1);
        var layout = _layoutContainer.AddComponent<HorizontalLayoutGroup>();
        layout.childControlHeight = true;
        layout.childControlWidth = true;
        layout.childScaleHeight = false;
        layout.childScaleWidth = false;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = true;
        layout.childAlignment = TextAnchor.UpperLeft;
        layout.spacing = 8;
        _layoutContainer.AddComponent<LayoutElement>();
        var contentSizeFitter = _layoutContainer.AddComponent<ContentSizeFitter>();
        contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        RenderBinding();
        RenderLabelText();
        AlignItemsToScreen();

    }

    private static void AlignItemsToScreen()
    {
        var rect = _containerObject.GetComponent<RectTransform>();
        if (rect is not null)
        {
            rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 20, 40);
        }
    }

    private static void RenderLabelText()
    {
        if (valueGo is not null) Object.Destroy(valueGo);
        valueGo = new GameObject();
        valueGo.transform.SetParent(_layoutContainer.transform);
        var contentSizeFitter = valueGo.AddComponent<ContentSizeFitter>();
        contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        valueGo.AddComponent<CanvasRenderer>();
        var text = valueGo.AddComponent<LocalizedText>();
        valueGo.transform.localScale = new Vector3(1, 1, 1);
        valueGo.transform.localPosition = new Vector3(1, 1, 1);

        text.normalText = GetFunction2Binding() != -1 ? "Watch Demo" : "[Playback button not bound]";
        text.TextComponent.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.TextComponent.fontSize = 30;
        var valueShadow = valueGo.AddComponent<Shadow>();
        valueShadow.effectColor = new Color(0, 0, 0, 1);
        valueShadow.effectDistance = new Vector2(4, -4);
        valueShadow.useGraphicAlpha = true;

        text.TextComponent.font = FontAssetManager.Instance.OverlayFont;
    }

    private static void RenderBinding()
    {
        if (labelGo) Object.Destroy(labelGo);
        labelGo = new GameObject();
        labelGo.transform.SetParent(_layoutContainer.transform);
        labelGo.transform.SetSiblingIndex(0);
        var playbackButtonMap = GetFunction2Binding();
        if (playbackButtonMap == -1) return;
        if (UserPersistence.primaryUser.p1ButtonMap
            .GetOrCreateConfig(ControllerManager.GetController(0)).isKeyboard)
        {
            var bindingText = ButtonIconHelper.GetKeyName(playbackButtonMap);
            labelGo.transform.SetParent(_layoutContainer.transform);
            labelGo.AddComponent<CanvasRenderer>();
            var text = labelGo.AddComponent<LocalizedText>();
            labelGo.transform.localScale = new Vector3(1, 1, 1);
            labelGo.transform.localPosition = new Vector3(1, 1, 1);
            text.normalText = $"[{bindingText}]";
            text.TextComponent.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.TextComponent.fontSize = 30;
            text.color = Color.yellow;
            var labelShadow = labelGo.AddComponent<Shadow>();
            labelShadow.effectColor = new Color(0, 0, 0, 1);
            labelShadow.effectDistance = new Vector2(4, -4);
            labelShadow.useGraphicAlpha = true;
            text.TextComponent.font = FontAssetManager.Instance.OverlayFont;
        }
        else
        {
            var playbackButtonSprite =
                ButtonIconHelper.GetButtonIconSpriteName(ButtonMode.Xbox, playbackButtonMap, true);
            var sprite = SpriteMap.Instance.GetMapping(playbackButtonSprite);
            if (sprite)
            {
                var img = labelGo.GetComponent<Image>();
                if (img is null)
                {
                    img = labelGo.AddComponent<Image>();
                    var rect = labelGo.GetComponent<RectTransform>();
                    if (rect is not null)
                    {
                        rect.sizeDelta = new() { x = 40, y = 40 };
                        rect.transform.localPosition = new(1, 1, 1);
                        rect.transform.localScale = new(1, 1, 1);
                    }
                }

                img.sprite = sprite;
                img.overrideSprite = sprite;
                img.preserveAspect = true;
            }
        }

        var contentSizeFitter = labelGo.AddComponent<ContentSizeFitter>();
        contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    private static int GetFunction2Binding()
    {
        return UserPersistence.primaryUser.p1ButtonMap.GetPhysicalButton(
            ControllerManager.GetController(0),
            ButtonMap.RemappableButton.Function2);
    }

    public static void Prefix()
    {
        if (!Enabled) return;
        RenderBinding();
        RenderLabelText();
        AlignItemsToScreen();
    }

    public static void Teardown()
    {
        Object.Destroy(_containerObject);
        _containerObject = null;
    }
}
