using System;
using GrimbaHack.Utility;
using nway.ui;

namespace GrimbaHack.UI.Elements;

public static class EnableCameraSelector
{
    public static void Generate(MenuPage menuPage)
    {
        var selector =
            menuPage.Page.AddItem<MenuListSelector<DefaultMenuOptions>>("enableCameraControlSelector");
        var items = new Il2CppSystem.Collections.Generic.List<DefaultMenuOptions>();
        items.Add(DefaultMenuOptions.Enabled);
        items.Add(DefaultMenuOptions.Disabled);
        selector.Items =
            items.TryCast<Il2CppSystem.Collections.Generic.IList<DefaultMenuOptions>>();
        selector.LocalizedText = "Enable Camera Control";
        selector.CurrentItem =
            CameraControl.Instance.Enabled ? DefaultMenuOptions.Enabled : DefaultMenuOptions.Disabled;
        selector.OnValueChanged = (Action<DefaultMenuOptions, DefaultMenuOptions>)((newValue, _) =>
        {
            CameraControl.Instance.Enabled = newValue == DefaultMenuOptions.Enabled;
        });
    }
}