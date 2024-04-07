using System;
using GrimbaHack.Modules.ComboRecorder;
using nway.ui;

namespace GrimbaHack.UI.Popup.TrainingSettings.MenuItems;

static class ComboRecorderSelector
{
    public static void Generate(UIPage menuPage)
    {
        var selector =
            menuPage.AddItem<MenuListSelector<DefaultMenuOptions>>("enableComboRecorderSelector");
        var items = new Il2CppSystem.Collections.Generic.List<DefaultMenuOptions>();
        items.Add(DefaultMenuOptions.Enabled);
        items.Add(DefaultMenuOptions.Disabled);
        selector.Items =
            items.TryCast<Il2CppSystem.Collections.Generic.IList<DefaultMenuOptions>>();
        selector.LocalizedText = "Enable Combo Recorder";

        selector.CurrentItem = ComboRecorderManager.Instance.Enabled
            ? DefaultMenuOptions.Enabled
            : DefaultMenuOptions.Disabled;
        selector.OnValueChanged = (Action<DefaultMenuOptions, DefaultMenuOptions>)((newValue, _) =>
        {
            ComboRecorderManager.Instance.Enabled = newValue == DefaultMenuOptions.Enabled;
            ;
        });
    }
}
