using System.Collections.Generic;

namespace GrimbaHack.Modules.Combo;

public sealed class ComboTrackerController
{
    private bool _enabled;

    private ComboTrackerController()
    {
    }

    public static ComboTrackerController Instance { get; private set; }

    static ComboTrackerController()
    {
        Instance = new ComboTrackerController();
        ComboTracker.Instance.OnNextStep((step, info) =>
        {
            UIComboTracker.Instance.SetNextStepText(UIComboTracker.StripCharacterName(step, info.attacker));
        });
        ComboTracker.Instance.OnComplete(() =>
        {
            UIComboTracker.Instance.SetStatusText("Complete!");
            UIComboTracker.Instance.SetNextStepText("Complete!");
        });
    }

    public void SetEnabled(bool value)
    {
        _enabled = value;
        ComboTracker.Instance.Enabled = value;
        UIComboTracker.Instance.SetVisible(value);
        if (value)
        {
            if (ComboTracker.Instance.GetCombo().Count > 0)
            {
                ComboTracker.Instance.SetState(ComboTrackerState.Comparing);
            }
            else
            {
                ComboTracker.Instance.SetState(ComboTrackerState.Idle);
            }
        }
    }

    public List<string> GetCombo()
    {
        return ComboTracker.Instance.GetCombo();
    }
}