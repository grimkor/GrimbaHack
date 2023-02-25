using GrimbaHack.Data;
using GrimbaHack.Modules;
using GrimbaHack.Utility;
using UnityEngine;
using UniverseLib.UI;

namespace GrimbaHack.UI.TrainingMode;

public class OnlineTrainingPanel : MenuPanelBase
{
    public OnlineTrainingPanel(UIBase owner) : base(owner)
    {
        OnEnterPremadeMatchActionHandler.Instance.AddCallback(() =>
            SetButtonVisible(true));
        OnEnterMainMenuActionHandler.Instance.AddCallback(() => { SetButtonVisible(false); });
        SetButtonVisible(false);
    }


    public override string Name => "Online Training";

    public override int MinWidth => 500;

    public override int MinHeight => 300;

    public override Vector2 DefaultAnchorMin => new Vector2(0.5f, 0.5f);

    public override Vector2 DefaultAnchorMax => new Vector2(0.5f, 0.5f);

    public override PanelTypes PanelType => PanelTypes.OnlineTrainingMode;

    protected override void ConstructPanelContent()
    {
        SetActive(false);
        CollisionBoxViewer.CreateUIControls(ContentRoot);
        OnlineTrainingMode.CreateUIControls(ContentRoot);
    }
}