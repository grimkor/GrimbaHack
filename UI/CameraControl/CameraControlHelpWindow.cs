using System.Collections.Generic;
using UnityEngine;
using UniverseLib.UI;
using UniverseLib.UI.Panels;

namespace GrimbaHack;

public class CameraControlHelpWindow : PanelBase
{
    public CameraControlHelpWindow(UIBase owner) : base(owner)
    {
    }

    public override string Name => "How To Use Camera Control";
    public override int MinWidth => 400;
    public override int MinHeight => 350;
    public override Vector2 DefaultAnchorMin => new Vector2(0.5f, 0.5f); // Centered
    public override Vector2 DefaultAnchorMax => new Vector2(0.5f, 0.5f); // Centered

    protected override void ConstructPanelContent()
    {
        Enabled = false;
        var textTemplate = new List<string>()
        {
            "F9 - Enable",
            "F10 - Disable",
            "Y - forward",
            "H - Backwards",
            "G - Left",
            "J - Right",
            "T - Up",
            "U - Down",
            "O - Look up",
            "L - Look Down",
            "K - Look Left",
            "; - Look Right"
        };

        foreach (string line in textTemplate)
        {
            var text = UIFactory.CreateLabel(UIRoot, "CameraControlHelpText", line);
            UIFactory.SetLayoutElement(text.gameObject, minHeight: 25, minWidth: 25);
        }
    }
}