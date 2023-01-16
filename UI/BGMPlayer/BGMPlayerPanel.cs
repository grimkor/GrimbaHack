using System;
using GrimbaHack.Data;
using GrimbaHack.Modules;
using UnityEngine;
using UniverseLib.UI;

namespace GrimbaHack.UI.BGMPlayer;

public sealed class BGMPlayerPanel : MenuPanelBase
{
    public BGMPlayerPanel(UIBase owner) : base(owner)
    {
    }

    public override PanelTypes PanelType => PanelTypes.BGMPlayer;
    public override string Name => "BGM Player";
    public override int MinWidth => 300;
    public override int MinHeight => 100;
    public override Vector2 DefaultAnchorMin { get; } = new(.5f, .5f);
    public override Vector2 DefaultAnchorMax { get; } = new(.5f, .5f);
    private Action _callback;

    protected override void ConstructPanelContent()
    {
        SetActive(false);
        BGMSelector.CreateUIControls(UIRoot, out _callback);
    }

    public override void Update()
    {
        base.Update();
        _callback();
    }
}