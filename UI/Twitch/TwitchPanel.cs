using GrimbaHack.Data;
using GrimbaHack.Modules.Twitch;
using UnityEngine;
using UniverseLib.UI;

namespace GrimbaHack.UI.Twitch;

public class TwitchPanel : MenuPanelBase
{
    public TwitchPanel(UIBase owner) : base(owner)
    {
    }

    protected override void ConstructPanelContent()
    {
        SetActive(false);
        TwitchApi.CreateUIControls(ContentRoot);
        MatchPrediction.CreateUIControls(ContentRoot);
    }

    public override string Name { get; } = "Twitch Integration";
    public override int MinWidth { get; } = 500;
    public override int MinHeight { get; } = 200;
    public override Vector2 DefaultAnchorMin { get; } = new Vector2(0.5f, 0.5f);
    public override Vector2 DefaultAnchorMax { get; } = new Vector2(0.5f, 0.5f);
    public override bool CanDragAndResize => true;

    public override PanelTypes PanelType => PanelTypes.Twitch;
}