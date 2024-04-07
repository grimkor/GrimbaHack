using System;
using GrimbaHack.Modules;
using nway.ui;

namespace GrimbaHack.UI.Popup.TrainingSettings.MenuItems;

static class EnhancedDummyPushblockSelector
{
    public static void Generate(UIPage menuPage)
    {
        var selector =
            menuPage.AddItem<MenuListSelector<DefaultMenuOptions>>("enhancedPushblockSelector");
        var rangeSelector = EnhancedDummyPushblockRangeSelector.Generate(menuPage);
        var items = new Il2CppSystem.Collections.Generic.List<DefaultMenuOptions>();
        items.Add(DefaultMenuOptions.Enabled);
        items.Add(DefaultMenuOptions.Disabled);
        selector.Items =
            items.TryCast<Il2CppSystem.Collections.Generic.IList<DefaultMenuOptions>>();
        selector.LocalizedText = "Enhanced Pushblock Options";
        selector.CurrentItem =
            ExtraPushblockOptions.Instance.Enabled ? DefaultMenuOptions.Enabled : DefaultMenuOptions.Disabled;

        selector.OnValueChanged = (Action<DefaultMenuOptions, DefaultMenuOptions>)((newValue, _) =>
        {
            ExtraPushblockOptions.Instance.Enabled = newValue == DefaultMenuOptions.Enabled;
            rangeSelector.Enabled = newValue == DefaultMenuOptions.Enabled;
        });
    }
}