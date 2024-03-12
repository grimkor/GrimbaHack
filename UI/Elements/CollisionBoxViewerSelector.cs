using System;
using GrimbaHack.Modules;
using GrimbaHack.Utility;
using nway.ui;

namespace GrimbaHack.UI.Elements;

static class CollisionBoxViewerSelector
{
    public static void Generate(MenuPage menuPage)
    {
        var selector =
            menuPage.AddItem<MenuListSelector<DefaultMenuOptions>>("collisionBoxViewerSelector");
        var items = new Il2CppSystem.Collections.Generic.List<DefaultMenuOptions>();
        items.Add(DefaultMenuOptions.Disabled);
        items.Add(DefaultMenuOptions.Enabled);
        selector.Items =
            items.TryCast<Il2CppSystem.Collections.Generic.IList<DefaultMenuOptions>>();
        selector.LocalizedText = "Enable Collision Box Viewer";
        selector.CurrentItem = CollisionBoxViewer.Instance.Enabled
            ? DefaultMenuOptions.Enabled
            : DefaultMenuOptions.Disabled;
        selector.OnValueChanged =
            new Action<DefaultMenuOptions, DefaultMenuOptions>((newValue, _) =>
            {
                CollisionBoxViewer.Instance.Enabled = newValue == DefaultMenuOptions.Enabled;
            });
    }
}