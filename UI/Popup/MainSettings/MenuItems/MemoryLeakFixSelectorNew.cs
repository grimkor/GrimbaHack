using System;
using GrimbaHack.Modules;
using nway.ui;

namespace GrimbaHack.UI.Popup.MainSettings.MenuItems;

static class MemoryLeakFixSelectorNew
{
    public static void Generate(UIPage menuPage)
    {
        var selector =
            menuPage.AddItem<MenuListSelector<DefaultMenuOptions>>("memoryLeakFixSelector");
        var items = new Il2CppSystem.Collections.Generic.List<DefaultMenuOptions>();
        items.Add(DefaultMenuOptions.Enabled);
        items.Add(DefaultMenuOptions.Disabled);
        selector.Items =
            items.TryCast<Il2CppSystem.Collections.Generic.IList<DefaultMenuOptions>>();
        selector.LocalizedText = "Memory Leak Fix";
        selector.CurrentItem =
            MemoryLeakFix.Instance.Enabled ? DefaultMenuOptions.Enabled : DefaultMenuOptions.Disabled;
        selector.OnValueChanged = (Action<DefaultMenuOptions, DefaultMenuOptions>)((newValue, _) =>
        {
            MemoryLeakFix.Instance.Enabled = newValue == DefaultMenuOptions.Enabled;
        });
    }
}
