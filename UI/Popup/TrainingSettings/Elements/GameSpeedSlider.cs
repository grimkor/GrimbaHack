using System;
using GrimbaHack.Modules;
using GrimbaHack.UI.MenuItems;
using nway.gameplay.ui;
using nway.ui;
using UnityEngine.Events;

namespace GrimbaHack.UI.Elements;

public static class GameSpeedSlider
{
    private static bool _selected;

    public static void Generate(MenuPage menuPage, ButtonBarConfig buttonBarConfig, UIWindow window)
    {
        var rangeSelector =
            menuPage.Page.AddItem<BetterRangeSelector>("gameSpeedRangeSelector");
        rangeSelector.LocalizedText = "Game Speed (%)";
        rangeSelector.MinValue = 1;
        rangeSelector.MaxValue = 100;
        rangeSelector.CurrentValue = SimulationSpeed.GetSpeed();
        rangeSelector.OnValueChanged = (Action<int, int>)((newValue, _) => { SimulationSpeed.Instance.SetSpeed(newValue); });
        rangeSelector.selectable.SetOnSelect((Action<ILayeredEventData>)(eventData =>
        {
            _selected = true;
            buttonBarConfig.SetLocalizedText(ButtonBarItem.ButtonY, "Reset Value");
            nway.gameplay.ui.UIManager.Get.ButtonBar.Update(eventData.Input, UserPersistence.Get.p1ButtonMap,
                buttonBarConfig);
        }));
        rangeSelector.selectable.SetOnDeselect((Action<ILayeredEventData>)(eventData =>
        {
            _selected = false;
            buttonBarConfig.ClearText(ButtonBarItem.ButtonY);
            nway.gameplay.ui.UIManager.Get.ButtonBar.Update(eventData.Input, UserPersistence.Get.p1ButtonMap,
                buttonBarConfig);
        }));
        window.AddButtonCallback(MenuButton.XboxY, (UnityAction<ILayeredEventData>)((ILayeredEventData eventData) =>
        {
            if (_selected)
            {
                rangeSelector.CurrentValue = 100;
                rangeSelector.slider.Set(1);
                SimulationSpeed.Instance.SetSpeed(100);
            }
        }));
    }
}
