using System.Collections.Generic;
using GrimbaHack.Data;
using GrimbaHack.UI.Global;
using GrimbaHack.UI.BGMPlayer;
using GrimbaHack.UI.TrainingMode;
using UniverseLib.UI;
using UniverseLib.UI.Panels;

namespace GrimbaHack.UI;

public static class UIManager
{
    public static bool ShowUI
    {
        get => UIBase != null && UIBase.Enabled;
        set
        {
            if (UIBase == null || UIBase.Enabled == value) return;
            UniversalUI.SetUIActive("UI.Grimbakor.UI", value);
        }
    }

    static Dictionary<PanelTypes, PanelBase> _panels = new();

    internal static UIBase UIBase { get; private set; }

    public static void Init()
    {
        UIBase = UniversalUI.RegisterUI("UI.Grimbakor.UI", Update);
        Toolbar.CreateUI(UIBase.RootObject);
        // var globalPanel = new GlobalPanel(UIBase);
        _panels.Add(PanelTypes.Global, new GlobalPanel(UIBase));
        _panels.Add(PanelTypes.BGMPlayer, new BGMPlayerPanel(UIBase));
        _panels.Add(PanelTypes.TrainingMode, new TrainingModePanel(UIBase));
        _panels.Add(PanelTypes.RecordingDummy, new RecordingDummyPanel(UIBase));
        UIBase.Enabled = false;
    }

    public static void Update()
    {
    }

    public static void TogglePanel(PanelTypes panelType)
    {
        _panels[panelType]?.Toggle();
    }
    public static void ShowPanel(PanelTypes panelType)
    {
        _panels[panelType]?.SetActive(true);
    }
    public static void HidePanel(PanelTypes panelType)
    {
        Plugin.Log.LogInfo($"_panels[panelType] {_panels[panelType]}");
        _panels[panelType].SetActive(false);
    }
}