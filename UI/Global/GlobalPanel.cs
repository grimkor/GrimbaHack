using System;
using System.Collections.Generic;
using GrimbaHack.Data;
using GrimbaHack.Modules;
using UnityEngine;
using UniverseLib.UI;

namespace GrimbaHack.UI.Global;

public class GlobalPanel : MenuPanelBase
{
    public GlobalPanel(UIBase owner) : base(owner)
    {
    }

    public override PanelTypes PanelType => PanelTypes.Global;
    public override string Name { get; } = "Global";
    public override int MinWidth { get; } = 500;
    public override int MinHeight { get; } = 200;

    public override Vector2 DefaultAnchorMin { get; } = new Vector2(0.5f, 0.5f);
    public override Vector2 DefaultAnchorMax { get; } = new Vector2(0.5f, 0.5f);
    public override bool CanDragAndResize => true;
    private List<Action> _updateCallbacks = new();

    protected override void ConstructPanelContent()
    {
        SetActive(false);
        PickSameCharacter.CreateUIControls(ContentRoot);
        CameraControl.CreateUIControls(ContentRoot);
        TextureLoader.CreateUIControls(ContentRoot);
    }

    public override void Update()
    {
        base.Update();
        foreach (var updateCallback in _updateCallbacks)
        {
            updateCallback();
        }
    }
}