using nway.ui;
using UnityEngine;
using UnityEngine.UI;

namespace GrimbaHack.Modules;

public class LabelValueOverlayText
{
    public GameObject _containerObject;
    private LocalizedText _labelText;
    private LocalizedText _valueText;

    public string Label
    {
        get { return _labelText.TextComponent.text; }
        set { _labelText.normalText = value; }
    }

    public string Value
    {
        get { return _valueText.TextComponent.text; }
        set { _valueText.normalText = value; }
    }

    public Vector3 Position
    {
        get { return _containerObject.transform.position; }
        set { _containerObject.transform.position = value; }
    }

    public bool Enabled
    {
        get => _containerObject.active;
        set => _containerObject.SetActive(value);
    }

    public LabelValueOverlayText(string label, string value, Vector3 position)
    {
        RenderTextOverlay(label, value, position);
    }

    private void RenderTextOverlay(string labelText, string valueText, Vector3 position)
    {
        // Check if it's already rendered, if so, don't render again
        if (_containerObject) return;
        var canvasGameObject =
            BaseUIManager.instance.Layers[UILayerType.ForegroundOrthographic].rootCanvas.gameObject;


        // Container
        _containerObject = new GameObject();
        _containerObject.transform.SetParent(canvasGameObject.transform);
        _containerObject.transform.localPosition = position;
        _containerObject.transform.localScale = new Vector3(1, 1, 1);
        _containerObject.AddComponent<CanvasRenderer>();
        _containerObject.AddComponent<VerticalLayoutGroup>();
        _containerObject.AddComponent<ContentSizeFitter>();

        // Frame Advantage pair
        var frameAdvantageContainer = new GameObject();
        frameAdvantageContainer.transform.SetParent(_containerObject.transform);
        frameAdvantageContainer.transform.localPosition = new Vector3(1, 1, 1);
        frameAdvantageContainer.transform.localScale = new Vector3(1, 1, 1);
        var layout = frameAdvantageContainer.AddComponent<HorizontalLayoutGroup>();
        layout.childControlHeight = true;
        layout.childControlWidth = false;
        layout.childScaleHeight = false;
        layout.childScaleWidth = false;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = false;
        layout.childAlignment = TextAnchor.MiddleLeft;
        layout.spacing = 8;
        frameAdvantageContainer.AddComponent<LayoutElement>();

        var labelGo = new GameObject();
        labelGo.transform.SetParent(frameAdvantageContainer.transform);
        labelGo.AddComponent<CanvasRenderer>();
        _labelText = labelGo.AddComponent<LocalizedText>();
        labelGo.transform.localScale = new Vector3(1, 1, 1);
        labelGo.transform.localPosition = new Vector3(1, 1, 1);
        _labelText.normalText = labelText;
        _labelText.TextComponent.horizontalOverflow = HorizontalWrapMode.Overflow;
        _labelText.TextComponent.fontSize = 30;
        var labelShadow = labelGo.AddComponent<Shadow>();
        labelShadow.effectColor = new Color(0, 0, 0, 1);
        labelShadow.effectDistance = new Vector2(4, -4);
        labelShadow.useGraphicAlpha = true;
        _labelText.TextComponent.font = FontAssetManager.Instance.OverlayFont;

        var valueGo = new GameObject();
        valueGo.transform.SetParent(frameAdvantageContainer.transform);
        valueGo.AddComponent<CanvasRenderer>();
        _valueText = valueGo.AddComponent<LocalizedText>();
        valueGo.transform.localScale = new Vector3(1, 1, 1);
        valueGo.transform.localPosition = new Vector3(1, 1, 1);
        _valueText.normalText = valueText;
        _valueText.TextComponent.horizontalOverflow = HorizontalWrapMode.Overflow;
        _valueText.TextComponent.fontSize = 30;
        var valueShadow = valueGo.AddComponent<Shadow>();
        valueShadow.effectColor = new Color(0, 0, 0, 1);
        valueShadow.effectDistance = new Vector2(4, -4);
        valueShadow.useGraphicAlpha = true;

        _valueText.TextComponent.font = FontAssetManager.Instance.OverlayFont;
    }
}
