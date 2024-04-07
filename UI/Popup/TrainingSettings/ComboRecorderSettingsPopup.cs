using System;
using GrimbaHack.UI.Popup.TrainingSettings.MenuItems;
using HarmonyLib;
using nway;
using nway.gameplay.ui;
using nway.ui;
using UnityEngine;

namespace GrimbaHack.UI.Popup.TrainingSettings;

[HarmonyPatch(typeof(UISpectateOptions), nameof(UISpectateOptions.OnInitializeComponents))]
public class ComboRecorderSettingsPopup
{
    private static UISpectateOptions _popup;
    private static readonly string _headerText = "Combo Recorder Settings";
    private static bool _bulkUpdate;
    private static Action _callback;
    private static MenuPage _mainPage;
    private static UIStackedMenu stack;
    private static UIMenuComponentGenerator uiMenuGenerator;

    public static void Show(Action callback)
    {
        _bulkUpdate = false;
        var window = BaseUIManager.instance.PopupManager.CheckActiveModalWindow("UI_SpectateOptions");
        _popup = new UISpectateOptions();
        if (window == null)
        {
            nway.gameplay.ui.UIManager.Get.PopupManager.uiPopupAssetList.Remove(_popup.GetUIPath());
        }

        _callback = callback;
        _popup.ShowModalWindow();
    }

    private static void CreateMainPage(UISpectateOptions uiSpectateOptions)
    {
        _mainPage = MenuPage.Create(uiMenuGenerator, uiSpectateOptions.transform.FindByName<Transform>("root"),
            "buttonRoot");
        var headerText = uiSpectateOptions.transform.FindByName<LocalizedText>("root/buttonRoot/headerText");
        headerText.localizedText = _headerText;
        _mainPage.Size = new(480, 600);
    }

    static bool Prefix(UISpectateOptions __instance)
    {
        if (_popup?.Pointer != __instance.Pointer) return true;
        stack = new UIStackedMenu(_popup.EventSystem);
        uiMenuGenerator =
            new UIMenuComponentGenerator(_popup.transform.FindByName<Transform>("templates/menuComponents"));

        CreateMainPage(_popup);
        _popup.mainPage = _mainPage.Page;

        ComboRecorderSelector.Generate(_mainPage.Page);
        ComboRecorderExportSubmit.Generate(_mainPage.Page);

        _popup.startingSelection = _mainPage.Page.GetDefaultSelection();
        _mainPage.Page.CreateChain(true, true, _popup.InputLayer);
        _popup.SetOnShowCallback((Action)(() =>
        {
            stack.PushPageWithSelection(_mainPage.Page, _popup.InputLayer, _mainPage.Page.GetDefaultSelection(),
                _popup.InputLayer, _mainPage.Page.GetDefaultSelection());
            nway.gameplay.ui.UIManager.Get.ButtonBar.Push(ControllerManager.GetController(0),
                UserPersistence.Get.p1ButtonMap, _popup.buttonBarConfig);
            _popup.buttonBarConfig.SetLocalizedText(ButtonBarItem.Cancel,
                UITextManager.Get.GetUIText("UIText_Back_01"));
            _popup.buttonBarConfig.SetLocalizedText(ButtonBarItem.NavUpDown,
                UITextManager.Get.GetUIText("UIText_Move_01"));


            UpdateButtonBarConfig();
            _popup.EventSystem.SetSelectedGameObject(_popup.startingSelection.Selectable,
                _popup.InputLayer);
        }));
        _popup.SetOnHideCallback((Action)(() =>
        {
            _popup.OnHide();
            if (_callback != null)
            {
                _callback();
            }
        }));
        _popup.SetOnCancelCallback((Action<ILayeredEventData>)GoBack);

        return false;
    }

    private static void UpdateButtonBarConfig()
    {
        nway.gameplay.ui.UIManager.Get.ButtonBar.Update(ControllerManager.GetController(0),
            UserPersistence.Get.p1ButtonMap, _popup.buttonBarConfig);
    }

    private static void GoBack(ILayeredEventData data)
    {
        data.Use();
        if (stack.Count == 1)
        {
            _popup.CloseWindow();
        }
        else
        {
            stack.PopPage(_popup.EventSystem);
        }
    }
}
