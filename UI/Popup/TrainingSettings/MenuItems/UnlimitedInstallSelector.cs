using System;
using GrimbaHack.Modules;
using nway.ui;

namespace GrimbaHack.UI.Popup.TrainingSettings.MenuItems;

public static class UnlimitedInstallSelector
{

    public static void Generate(MenuPage menuPage)
    {
        var selector =
            menuPage.Page.AddItem<MenuListSelector<DefaultMenuOptions>>("unlimitedInstallTimeSelector");
        var items = new Il2CppSystem.Collections.Generic.List<DefaultMenuOptions>();
        items.Add(DefaultMenuOptions.Enabled);
        items.Add(DefaultMenuOptions.Disabled);
        selector.Items =
            items.TryCast<Il2CppSystem.Collections.Generic.IList<DefaultMenuOptions>>();
        selector.LocalizedText = "Unlimited Install (Adam/Eric)";
        selector.CurrentItem = UnlimitedInstall.Instance.GetEnabled()
            ? DefaultMenuOptions.Enabled
            : DefaultMenuOptions.Disabled;
        selector.OnValueChanged = (Action<DefaultMenuOptions, DefaultMenuOptions>)((newValue, oldValue) =>
        {
            UnlimitedInstall.Instance.SetEnabled(newValue == DefaultMenuOptions.Enabled);
        });
    }
}