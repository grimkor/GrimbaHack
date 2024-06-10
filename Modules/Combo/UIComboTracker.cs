using System.Collections.Generic;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;
using UnityEngine.UI;

namespace GrimbaHack.Modules.Combo;

public class UIComboTracker
{
    public static UIComboTracker Instance { get; private set; }
    private readonly LabelValueOverlayText _statusOverlay = new("Status", "Ready", new Vector3(240, 240, 1));
    private readonly LabelValueOverlayText _nextStepOverlay = new("Next Step", "-", new Vector3(240, 210, 1));
    private UIComboTrackerBehaviour Behaviour;
    private bool _enabled;

    private UIComboTracker()
    {
    }

    static UIComboTracker()
    {
        Instance = new UIComboTracker();
        Instance._nextStepOverlay.Enabled = false;
        Instance._statusOverlay.Enabled = false;
        ClassInjector.RegisterTypeInIl2Cpp<UIComboTrackerBehaviour>();
        var go = new GameObject();
        Object.DontDestroyOnLoad(go);
        go.hideFlags = HideFlags.HideAndDontSave;
        Instance.Behaviour = go.AddComponent<UIComboTrackerBehaviour>();
        Instance.Behaviour.SetEnable(false);
    }

    public void SetVisible(bool visible)
    {
        Instance.Behaviour.SetEnable(visible);
        Instance._enabled = visible;
        _statusOverlay.Enabled = visible;
        _nextStepOverlay.Enabled = visible;
        Instance.Behaviour.enabled = visible;
    }

    public void SetCombo(List<string> combo)
    {
        var textRows = Instance.Behaviour._rows;
        var lowestMax = combo.Count < textRows.Count ? combo.Count : textRows.Count;
        textRows.ForEach(x => x.enabled = false);
        for (int i = 0; i < lowestMax; i++)
        {
            textRows[i].text = combo[i];
            textRows[i].enabled = true;
        }
    }

    public void SetStatusText(string text)
    {
        _statusOverlay.Value = text;
    }

    public void SetNextStepText(string text)
    {
        _nextStepOverlay.Value = text;
    }

    public static string StripCharacterName(string text, Character character)
    {
        return text.Replace(character.GetCharacterName() + "_combat_", "");
    }
}

class UIComboTrackerBehaviour : MonoBehaviour
{
    public GameObject containerObject;
    public RectTransform rectTransform { get; set; }
    public List<Text> _rows = new();

    public void Setup()
    {
        ClearRows();
        var rootObject = new GameObject();
        var canvas = rootObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        rootObject.AddComponent<CanvasScaler>();

        // Container
        containerObject = new GameObject();
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


        for (int i = 0; i < 10; i++)
        {
            var go = new GameObject();
            var text = go.AddComponent<Text>();
            text.AssignDefaultFont();
            text.fontSize = 20;
            text.text = $"Row {i}";
            text.enabled = false;
            go.transform.SetParent(containerObject.transform);
            _rows.Add(text);
        }

        rectTransform = containerObject.GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (rectTransform)
        {
            containerObject.transform.localPosition = new(0, 0, 0);
            containerObject.transform.localScale = new(1, 1, 1);
            rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, rectTransform.sizeDelta.x);
        }
    }

    private void ClearRows()
    {
        _rows.ForEach(Destroy);
        _rows.Clear();
    }

    public void SetEnable(bool value)
    {
        if (value)
        {
            Setup();
        }
        else
        {
            ClearRows();
        }
    }
}