using System.Collections.Generic;
using GrimbaHack.Data;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;

namespace GrimbaHack.UI.Global;

public sealed class Toolbar
{
    public static GameObject ButtonContainer { get; private set; }

    private Toolbar()
    {
    }

    public static Toolbar Instance { get; private set; }

    static Toolbar()
    {
        Instance = new Toolbar();
    }

    public Dictionary<PanelTypes, MenuPanelBase> _panels = new();

    public static void setButtonVisibility(PanelTypes panel, bool enabled)
    {
        var panelButton = Instance._panels[panel];
        if (panelButton != null)
        {
            panelButton.setButtonVisible(enabled);
        }
    }

    public static void CreateUI(GameObject uiRoot)
    {
        var toolbarPanel = UIFactory.CreateUIObject("ToolbarPanel", uiRoot);
        UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(toolbarPanel, false, false, true, true, 5, 5, 5, 5, 5,
            TextAnchor.MiddleCenter);
        toolbarPanel.AddComponent<Image>().color = Color.black;

        var toolbarRect = toolbarPanel.GetComponent<RectTransform>();
        toolbarRect.pivot = new Vector2(0.5f, 1f);

        toolbarRect.anchorMin = new Vector2(0.5f, 1f);
        toolbarRect.anchorMax = new Vector2(0.5f, 1f);
        toolbarRect.anchoredPosition = new Vector2(0f, 0f);
        toolbarRect.sizeDelta = new Vector2(1000f, 50f);

        ButtonContainer = UIFactory.CreateUIObject("ButtonFlex", toolbarPanel);
        UIFactory.SetLayoutElement(ButtonContainer.gameObject, 25, flexibleHeight: 1000, flexibleWidth: 1000);
        UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(ButtonContainer, false, true, true, true, 5, 2, 2, 2, 2);
        var Text = UIFactory.CreateLabel(toolbarPanel.gameObject, "VersionText", Data.Global.Version,
            TextAnchor.MiddleRight, Color.white, fontSize: 16);
    }
}