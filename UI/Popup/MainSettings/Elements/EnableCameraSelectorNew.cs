using System;
using GrimbaHack.Utility;
using nway.ui;

namespace GrimbaHack.UI.Popup.MainSettings.Elements;

public static class EnableCameraSelectorNew
{
    public static void Generate(UIPage menuPage)
    {
        var selector =
            menuPage.AddItem<MenuListSelector<DefaultMenuOptions>>("enableCameraControlSelector");
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