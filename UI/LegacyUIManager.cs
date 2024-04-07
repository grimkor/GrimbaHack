using System.Collections.Generic;
using GrimbaHack.Data;
using GrimbaHack.UI.BGMPlayer;
using GrimbaHack.UI.OnlineTraining;
using GrimbaHack.UI.RecordingDummy;
using GrimbaHack.UI.TrainingMode;
using GrimbaHack.UI.Twitch;
using UniverseLib.UI;
using UniverseLib.UI.Panels;

namespace GrimbaHack.UI;

public static class LegacyUIManager
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
        Toolbar.Toolbar.CreateUI(UIBase.RootObject);
        _panels.Add(PanelTypes.BGMPlayer, new BGMPlayerPanel(UIBase));
        _panels.Add(PanelTypes.RecordingDummy, new RecordingDummyPanel(UIBase));
        _panels.Add(PanelTypes.OnlineTrainingMode, new OnlineTrainingPanel(UIBase));
        _panels.Add(PanelTypes.Twitch, new TwitchPanel(UIBase));
        UIBase.Enabled = false;
    }

    public static void Update()
    {
    }

    public static void TogglePanel(PanelTypes panelType)
    {
        _panels[panelType]?.Toggle();
    }

    public static void RefreshUI()
    {
        if (ShowUI)
        {
            ShowUI = false;
            ShowUI = true;
        }
    }
}
