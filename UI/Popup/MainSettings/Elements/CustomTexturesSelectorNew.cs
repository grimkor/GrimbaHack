using System;
using GrimbaHack.Modules;
using GrimbaHack.Utility;
using nway.ui;

namespace GrimbaHack.UI.Popup.MainSettings.Elements;

static class CustomTexturesSelectorNew
{
    public static void Generate(UIPage menuPage)
    {
        var selector =
            menuPage.AddItem<MenuListSelector<DefaultMenuOptions>>("customTexturesSelector");
        var items = new Il2CppSystem.Collections.Generic.List<DefaultMenuOptions>();
        items.Add(DefaultMenuOptions.Enabled);
        items.Add(DefaultMenuOptions.Disabled);
        selector.Items =
            items.TryCast<Il2CppSystem.Collections.Generic.IList<DefaultMenuOptions>>();
        selector.LocalizedText = "Load Custom Textures";
        selector.CurrentItem =
            TextureLoader.Instance.Enabled ? DefaultMenuOptions.Enabled : DefaultMenuOptions.Disabled;
        selector.OnValueChanged = (Action<DefaultMenuOptions, DefaultMenuOptions>)((newValue, _) =>
        {
            TextureLoader.Instance.Enabled = newValue == DefaultMenuOptions.Enabled;
        });
    }
}