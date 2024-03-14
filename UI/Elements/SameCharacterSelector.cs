using System;
using GrimbaHack.Modules;
using GrimbaHack.Utility;
using nway.ui;

namespace GrimbaHack.UI.Elements;

static class SameCharacterSelector
{
    public static void Generate(MenuPage menuPage)
    {
        var selector =
            menuPage.Page.AddItem<MenuListSelector<DefaultMenuOptions>>("sameCharacterSelectSelector");
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