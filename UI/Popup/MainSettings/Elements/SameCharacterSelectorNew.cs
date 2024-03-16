using System;
using GrimbaHack.Modules;
using GrimbaHack.Utility;
using nway.ui;

namespace GrimbaHack.UI.Popup.MainSettings.Elements;

static class SameCharacterSelectorNew
{
    public static void Generate(UIPage menuPage)
    {
        var selector =
            menuPage.AddItem<MenuListSelector<DefaultMenuOptions>>("sameCharacterSelectSelector");
        var items = new Il2CppSystem.Collections.Generic.List<DefaultMenuOptions>();
        items.Add(DefaultMenuOptions.Enabled);
        items.Add(DefaultMenuOptions.Disabled);
        selector.Items =
            items.TryCast<Il2CppSystem.Collections.Generic.IList<DefaultMenuOptions>>();
        selector.LocalizedText = "Allow Same Characters";
        selector.CurrentItem =
            PickSameCharacter.Instance.Enabled ? DefaultMenuOptions.Enabled : DefaultMenuOptions.Disabled;
        selector.OnValueChanged = (Action<DefaultMenuOptions, DefaultMenuOptions>)((newValue, _) =>
        {
            PickSameCharacter.Instance.Enabled = newValue == DefaultMenuOptions.Enabled;
        });
    }
}