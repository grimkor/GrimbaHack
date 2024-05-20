using nway.ui;

namespace GrimbaHack.UI.Popup.CommonElements;

public class BetterRangeSelector : MenuRangeSelector
{
    public int ChangeAmount = 5;
    new bool Increment()
    {
        CurrentValue += 5;
        selectable.PlaySfxEvent(PlaySFXOnSelectable.SelectableEvent.ChangeSelectorValue);
        return true;
    }
    new bool Decrement()
    {
        CurrentValue -= 5;
        selectable.PlaySfxEvent(PlaySFXOnSelectable.SelectableEvent.ChangeSelectorValue);
        return true;
    }
}