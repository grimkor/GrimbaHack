using GrimbaHack.Data;
using GrimbaHack.Modules;
using UnityEngine;
using UniverseLib.UI;

namespace GrimbaHack.UI.TrainingMode;

public class TrainingModePanel : MenuPanelBase
{
    public TrainingModePanel(UIBase owner) : base(owner)
    {
    }


    public override string Name => "Training Mode";

    public override int MinWidth => 300;

    public override int MinHeight => 200;

    public override Vector2 DefaultAnchorMin => new Vector2(0.5f, 0.5f); 

    public override Vector2 DefaultAnchorMax => new Vector2(0.5f, 0.5f);

    public override PanelTypes PanelType => PanelTypes.TrainingMode;
    
    protected override void ConstructPanelContent()
    {
        SetActive(false);
        FrameDataManager.CreateUIControls(ContentRoot);
        CollisionBoxViewer.CreateUIControls(ContentRoot);
    }
}