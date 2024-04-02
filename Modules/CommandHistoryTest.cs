using System.Linq;
using nway.ui;
using UnityEngine;
using UnityEngine.UI;

namespace GrimbaHack.Modules;

public class CommandHistoryItem
{
    private GameObject _containerObject;
    private LocalizedText _text;

    public string Label
    {
        get => _text.TextComponent.text;
        set => _text.normalText = value;
    }

    public Vector3 Position
    {
        get => _containerObject.transform.position;
        set => _containerObject.transform.position = value;
    }

    public bool Enable
    {
        get => _containerObject.active;
        set => _containerObject.SetActive(value);
    }

    public CommandHistoryItem(string label, Vector3 position)
    {
        RenderTextOverlay(label, position);
    }

    private void RenderTextOverlay(string labelText, Vector3 position)
    {
        if (_containerObject) return;

        var rootObject = new GameObject();
        var canvas = rootObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        rootObject.AddComponent<CanvasScaler>();

        // Container
        var containerObject = new GameObject();
        containerObject.transform.SetParent(rootObject.transform);
        // Canvas Renderer
        containerObject.AddComponent<CanvasRenderer>();
        var background = containerObject.AddComponent<Image>();
        background.color = new(0, 0, 0, 0.5f);
        // Size Fitter
        var sizeFitter = containerObject.AddComponent<ContentSizeFitter>();
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        // Vertical Layout Group
        var containerLayout = containerObject.AddComponent<VerticalLayoutGroup>();
        containerLayout.childForceExpandHeight = true;
        containerLayout.childForceExpandWidth = true;
        containerLayout.childControlHeight = true;
        containerLayout.childControlWidth = true;

        containerLayout.transform.localPosition = new(0,0,0);
        containerLayout.transform.localScale = new(1,1,1);

        for (int i = 0; i < 10; i++)
        {
            var go = new GameObject();
            go.transform.SetParent(containerObject.transform);
            var text = go.AddComponent<Text>();
            text.AssignDefaultFont();
            text.fontSize = 20;
            text.text = $"Test";
        }

        var rectTransform = containerObject.GetComponent<RectTransform>();
        if (rectTransform)
        {
            rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left,0,  rectTransform.sizeDelta.x);
        }
    }
}
