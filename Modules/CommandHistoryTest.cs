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
            text.text = $"FUCK YOU I WONT DO WHAT YOU TELL ME {i + 1}";
        }

        var rectTransform = containerObject.GetComponent<RectTransform>();
        if (rectTransform)
        {
            rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left,0,  rectTransform.sizeDelta.x);
        }

        // var sprites = Object.FindObjectsOfType<Sprite>();
        // var med = sprites.ToList().Find(sprite => sprite.name.Contains("button_pc_medium") && sprite.name.Contains("Clone"));
        // var spriteGo = new GameObject();
        // spriteGo.AddComponent<CanvasRenderer>();
        // var spr = spriteGo.AddComponent<Image>();
        // spr.sprite = med;
        // spr.transform.localScale = new Vector3(1, 1, 1);
        // spr.transform.localPosition = new Vector3(1, 1, 1);
        // spr.m_Sprite = med;
        // spriteGo.transform.SetParent(_containerObject.transform);
        // Plugin.Log.LogInfo($"{canvas.renderMode}");


        // var labelGo = new GameObject();
        // labelGo.transform.SetParent(horizontalLayout.transform);
        // labelGo.AddComponent<CanvasRenderer>();
        // _text = labelGo.AddComponent<LocalizedText>();
        // labelGo.transform.localScale = new Vector3(1, 1, 1);
        // labelGo.transform.localPosition = new Vector3(1, 1, 1);
        // _text.normalText = labelText;
        // _text.TextComponent.fontSize = 30;
        // _text.TextComponent.alignment = TextAnchor.MiddleLeft;
        // var labelShadow = labelGo.AddComponent<Shadow>();
        // labelShadow.effectColor = new Color(0, 0, 0, 1);
        // labelShadow.effectDistance = new Vector2(4, -4);
        // labelShadow.useGraphicAlpha = true;

        // assign the font if we found it
        // if (font != null)
        // {
        //     _text.TextComponent.font = font;
        // }
        // else
        // {
        //     _text.TextComponent.AssignDefaultFont();
        // }
    }
}