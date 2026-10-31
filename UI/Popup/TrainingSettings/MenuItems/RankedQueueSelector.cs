using System;
using GrimbaHack.Modules.TrainingModeQueue;
using nway.ui;

namespace GrimbaHack.UI.Popup.TrainingSettings.MenuItems;

static class RankedQueueSelector
{
    private const string LabelIdle = "Queue Online Ranked";
    private const string LabelSearching = "Searching for ranked opponent...";

    public static void Generate(UIPage menuPage)
    {
        var selector = menuPage.AddItem<MenuListSelector<DefaultMenuOptions>>("rankedQueueSelector");
        var items = new Il2CppSystem.Collections.Generic.List<DefaultMenuOptions>();
        items.Add(DefaultMenuOptions.Disabled);
        items.Add(DefaultMenuOptions.Enabled);
        selector.Items =
            items.TryCast<Il2CppSystem.Collections.Generic.IList<DefaultMenuOptions>>();
        selector.LocalizedText = TrainingModeQueue.IsEnabled() ? LabelSearching : LabelIdle;
        selector.CurrentItem = TrainingModeQueue.IsEnabled() ? DefaultMenuOptions.Enabled : DefaultMenuOptions.Disabled;
        selector.OnValueChanged = (Action<DefaultMenuOptions, DefaultMenuOptions>)((newValue, _) =>
        {
            switch (newValue)
            {
                case DefaultMenuOptions.Enabled:
                    TrainingModeQueue.StartQueue();
                    break;
                case DefaultMenuOptions.Disabled:
                    TrainingModeQueue.CancelSearch();
                    break;
            }
        });
    }

}
