using System;
using nway.ui;
using UnityEngine.Events;

namespace GrimbaHack.UI.MenuItems;

public static class BackButton
{
    public static void Create(MenuPage menuPage, Action onBackCallback)
    {
        var backButton = menuPage.AddItem<MenuSubmit>("backButton");
        backButton.LocalizedText = "Back";
        backButton.SetOnSubmit((UnityAction<ILayeredEventData>)((ILayeredEventData _) =>
        {
            if (onBackCallback != null)
            {
                onBackCallback();
            }
        }));
    }
}