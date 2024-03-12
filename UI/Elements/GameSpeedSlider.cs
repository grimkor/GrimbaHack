using System;
using GrimbaHack.Modules;
using GrimbaHack.UI.MenuItems;
using nway.ui;

namespace GrimbaHack.UI.Elements;

public static class GameSpeedSlider
{
    public static BetterRangeSelector Generate(MenuPage menuPage)
    {
        var rangeSelector =
            menuPage.Page.AddItem<BetterRangeSelector>("gameSpeedRangeSelector");
        rangeSelector.LocalizedText = "Game Speed (%)";
        rangeSelector.MinValue = 1;
        rangeSelector.MaxValue = 100;
        rangeSelector.CurrentValue = SimulationSpeed.GetSpeed();
        rangeSelector.OnValueChanged = (Action<int, int>)((newValue, _) => { SimulationSpeed.SetSpeed(newValue); });
        return rangeSelector;
    }
}