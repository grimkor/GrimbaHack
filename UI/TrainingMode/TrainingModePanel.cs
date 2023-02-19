using GrimbaHack.Data;
using GrimbaHack.Modules;
using GrimbaHack.Utility;
using UnityEngine;
using UniverseLib.UI;

namespace GrimbaHack.UI.TrainingMode;

public class TrainingModePanel : MenuPanelBase
{
    public TrainingModePanel(UIBase owner) : base(owner)
    {
        StartTrainingMatchActionHandler.Instance.AddCallback(() => SetButtonVisible(true));
        StartMainMenuActionHandler.Instance.AddCallback(() =>
        {
            SetActive(false);
            SetButtonVisible(false);
            
        });
        SetButtonVisible(false);
    }
    
    

    public override string Name => "Training Mode";

    public override int MinWidth => 500;

    public override int MinHeight => 300;

    public override Vector2 DefaultAnchorMin => new Vector2(0.5f, 0.5f);

    public override Vector2 DefaultAnchorMax => new Vector2(0.5f, 0.5f);

    public override PanelTypes PanelType => PanelTypes.TrainingMode;

    protected override void ConstructPanelContent()
    {
        SetActive(false);
        FrameDataManager.CreateUIControls(ContentRoot);
        CollisionBoxViewer.CreateUIControls(ContentRoot);
        SimulationSpeed.CreateUIControls(ContentRoot);
        DummyExPunish.CreateUIControls(ContentRoot);
        ExtraPushblockOptions.CreateUIControls(ContentRoot);
        UnlimitedInstall.CreateUIControls(ContentRoot);
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