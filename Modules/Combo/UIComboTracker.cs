using UnityEngine;

namespace GrimbaHack.Modules.Combo;

public class UIComboTracker
{
    public static UIComboTracker Instance { get; private set; }
    private readonly LabelValueOverlayText _statusOverlay = new("Status", "Ready", new Vector3(240, 240, 1));
    private readonly LabelValueOverlayText _nextStepOverlay = new("Next Step", "-", new Vector3(240, 210, 1));
    private bool _enabled;

    private UIComboTracker()
    {
    }

    static UIComboTracker()
    {
        Instance = new UIComboTracker
        {
            _statusOverlay =
            {
                Enable = false
            },
            _nextStepOverlay =
            {
                Enable = false
            }
        };
    }

    public void SetVisible(bool visible)
    {
        Instance._enabled = visible;
        _statusOverlay.Enable = visible;
        _nextStepOverlay.Enable = visible;
    }

    public void SetStatusText(string text)
    {
        _statusOverlay.Value = text;
    }

    public void SetNextStepText(string text)
    {
        _nextStepOverlay.Value = text;
    }

    public static string StripCharacterName(string text, Character character)
    {
        return text.Replace(character.GetCharacterName() + "_combat_", "");
    }
}