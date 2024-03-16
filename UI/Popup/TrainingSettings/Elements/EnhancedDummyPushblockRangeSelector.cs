using System;
using GrimbaHack.Modules;
using GrimbaHack.UI.MenuItems;
using nway.ui;

namespace GrimbaHack.UI.Popup.TrainingSettings.Elements;

static class EnhancedDummyPushblockRangeSelector
{
    public static BetterRangeSelector Generate(UIPage menuPage)
    {
        var selector =
            menuPage.AddItem<BetterRangeSelector>("enhancedPushblockRangeSelector");
        selector.LocalizedText = "% of Blockstun to Pushblock";
        selector.MinValue = 1;
        selector.MaxValue = 100;
        selector.CurrentValue = ExtraPushblockOptionsBehaviour.PercentToPushblock;
        selector.OnValueChanged = (Action<int, int>)((newValue, _) =>
        {
            ExtraPushblockOptions.Instance.SetPercentToPushblock(newValue);
        });
        selector.Enabled = ExtraPushblockOptions.Instance.Enabled;
        return selector;
    }
}