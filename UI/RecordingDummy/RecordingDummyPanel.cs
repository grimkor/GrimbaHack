using epoch.db;
using GrimbaHack.Data;
using GrimbaHack.Modules;
using GrimbaHack.UI.Global;
using GrimbaHack.Utility;
using HarmonyLib;
using nway.gameplay;
using nway.gameplay.match;
using UnityEngine;
using UniverseLib.UI;

namespace GrimbaHack.UI.TrainingMode;

public class RecordingDummyPanel : MenuPanelBase
{
    public RecordingDummyPanel(UIBase owner) : base(owner)
    {
        StartTrainingMatchActionHandler.Instance.AddCallback(() => SetButtonVisible(true));
        StartMainMenuActionHandler.Instance.AddCallback(() =>
        {
            UIManager.HidePanel(PanelTypes.RecordingDummy);
            SetButtonVisible(false);
        });
        SetButtonVisible(false);
    }
    
    

    public override string Name => "Recording Dummy";

    public override int MinWidth => 500;

    public override int MinHeight => 300;

    public override Vector2 DefaultAnchorMin => new Vector2(0.5f, 0.5f);

    public override Vector2 DefaultAnchorMax => new Vector2(0.5f, 0.5f);

    public override PanelTypes PanelType => PanelTypes.RecordingDummy;

    protected override void ConstructPanelContent()
    {
        SetActive(false);
        DummyTrimRecording.CreateUIControls(ContentRoot);
        MultipleRecordingSlots.CreateUIControls(ContentRoot);
    }
    
    // [HarmonyPatch(typeof(AppStateManager), nameof(AppStateManager.SetAppState))]
    // public class PatchSetAppState
    // {
    //     public static void Postfix(AppState state)
    //     {
    //         Toolbar.setButtonVisibility(PanelTypes.TrainingMode,  Data.Global.isTrainingMatch());
    //     }
    // }
}