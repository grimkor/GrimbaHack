using System;
using GrimbaHack.Modules;
using GrimbaHack.Utility;
using nway.ui;

namespace GrimbaHack.UI.Elements;

static class StageSelectOverrideSelector
{
    public static void Generate(MenuPage menuPage)
    {
    var selector =
        menuPage.AddItem<MenuListSelector<DefaultMenuOptions>>("stageOverrideSelector");

    var items = new Il2CppSystem.Collections.Generic.List<DefaultMenuOptions>();
    items.Add(DefaultMenuOptions.Disabled);
    items.Add(DefaultMenuOptions.Enabled);
    selector.Items = items.TryCast<Il2CppSystem.Collections.Generic.IList<DefaultMenuOptions>>();
    selector.CurrentItem = StageSelectOverride.Enabled? DefaultMenuOptions.Enabled : DefaultMenuOptions.Disabled;
    selector.LocalizedText = "Stage Select Override";
    selector.OnValueChanged = (Action<DefaultMenuOptions, DefaultMenuOptions>)((newValue, _) =>
    {
        StageSelectOverride.Enabled = newValue == DefaultMenuOptions.Enabled;
    });
    }

}