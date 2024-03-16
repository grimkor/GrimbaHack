using System;
using System.Collections.Generic;
using GrimbaHack.Data;
using GrimbaHack.Modules;
using GrimbaHack.Utility;
using HarmonyLib;
using nway;
using nway.gameplay.ui;
using nway.ui;
using UnityEngine;
using UnityEngine.Events;

namespace GrimbaHack.UI.Elements;

static class StageSelectOverrideSelector
{
    private static MenuPage _stageSelectMenuPage;
    private static UIPage _randomFilterPage;

    public static void Generate(MenuPage menuPage, UISettings uiSettings, ButtonBarConfig buttonBarConfig,
        UIStackedMenu stack)
    {
        // var randomSelectPage = uiSettings.transform.gameObject.AddComponent<RandomStageSelectPage>();
        // randomSelectPage.Init(uiSettings);
        // randomSelectPage.Hide();
        var stages = new Il2CppSystem.Collections.Generic.List<StageSelectOverrideOptions>();
        foreach (var stage in Data.Global.Stages)
        {
            stages.Add(stage.Key);
        }

        var selector =
            menuPage.Page.AddItem<MenuListSelector<StageSelectOverrideOptions>>("stageSelectOverrideSelector");
        selector.LocalizedText = "Stage Select Override";
        selector.Items = stages.TryCast<Il2CppSystem.Collections.Generic.IList<StageSelectOverrideOptions>>();
        selector.CurrentItem = StageSelectOverride.Stage.Key;
        selector.OnValueChanged = (Action<StageSelectOverrideOptions, StageSelectOverrideOptions>)((newValue, _) =>
        {
            var stage = Data.Global.Stages.Find(x => x.Key == newValue);
            if (stage != null)
            {
                StageSelectOverride.SetStage(stage);
                SetButtonConfigVisibility(stage.Key == StageSelectOverrideOptions.Random, buttonBarConfig);
            }
        });

        uiSettings.AddButtonCallback(MenuButton.XboxY, (UnityAction<ILayeredEventData>)((ILayeredEventData eventData) =>
        {
            if (StageSelectOverride.Stage.Key == StageSelectOverrideOptions.Random)
            {
                if (uiSettings.EventSystem.IsPriority(uiSettings.InputLayer) &&
                    uiSettings.EventSystem.CurrentSelectedByLayer(uiSettings.InputLayer).name ==
                    "stageSelectOverrideSelector")
                {
                    SetButtonConfigVisibility(false, buttonBarConfig);
                    RandomSelectPage.Show(() =>
                    {
                        menuPage.Page.SetVisible(true);
                        selector.Select(uiSettings.EventSystem, uiSettings.InputLayer);
                    });
                    uiSettings.EventSystem.SetSelectedGameObject(null, uiSettings.InputLayer);
                    menuPage.Page.SetVisible(false);
                }
            }
        }));
        selector.selectable.SetOnSelect((Action<ILayeredEventData>)(_ =>
        {
            SetButtonConfigVisibility(StageSelectOverride.Stage.Key == StageSelectOverrideOptions.Random,
                buttonBarConfig);
        }));
        selector.selectable.SetOnDeselect((Action<ILayeredEventData>)(_ =>
        {
            SetButtonConfigVisibility(false, buttonBarConfig);
        }));
    }

    public static UISpectateOptions SomeWindow { get; set; }


    private static void SetButtonConfigVisibility(bool visible, ButtonBarConfig buttonBarConfig)
    {
        if (visible)
        {
            buttonBarConfig.SetLocalizedText(ButtonBarItem.ButtonY, "Select Random Stages");
            nway.gameplay.ui.UIManager.Get.ButtonBar.Update(ControllerManager.GetController(0),
                UserPersistence.Get.p1ButtonMap,
                buttonBarConfig);
        }
        else
        {
            buttonBarConfig.ClearText(ButtonBarItem.ButtonY);
        }

        nway.gameplay.ui.UIManager.Get.ButtonBar.Update(ControllerManager.GetController(0),
            UserPersistence.Get.p1ButtonMap,
            buttonBarConfig);
    }
}

[HarmonyPatch(typeof(UISpectateOptions), nameof(UISpectateOptions.OnInitializeComponents))]
public class RandomSelectPage
{
    private static UISpectateOptions _popup;
    private static string _headerText = "Random Stage Filter";
    private static readonly List<MenuListSelector<DefaultMenuOptions>> _mapSelectors = new();
    private static bool _bulkUpdate;
    private static Action _callback;

    public static void Show(Action callback)
    {
        _bulkUpdate = false;
        _mapSelectors.Clear();
        _popup = new UISpectateOptions();
        _callback = callback;
        _popup.ShowModalWindow();
    }

    public static void Show()
    {
        _callback = null;
        _bulkUpdate = false;
        _mapSelectors.Clear();
        _popup = new UISpectateOptions();
        _popup.ShowModalWindow();
    }

    private static bool AllStagesEnabled()
    {
        return StageSelectOverride.RandomStages.Count == Data.Global.Stages.Count - 1;
    }

    private static void ToggleAllStages(ILayeredEventData _)
    {
        _bulkUpdate = true;
        var state = !AllStagesEnabled();
        foreach (var selector in _mapSelectors)
        {
            if (selector.CurrentItem == DefaultMenuOptions.Enabled != state)
            {
                selector.CurrentItem = state ? DefaultMenuOptions.Enabled : DefaultMenuOptions.Disabled;
            }
        }

        _bulkUpdate = false;
        UpdateButtonBarConfig();
    }

    static void UpdateButtonBarConfig()
    {
        if (AllStagesEnabled())
        {
            _popup.buttonBarConfig.SetLocalizedText(ButtonBarItem.ButtonY, "Disable All");
        }
        else
        {
            _popup.buttonBarConfig.SetLocalizedText(ButtonBarItem.ButtonY, "Enable All");
        }

        nway.gameplay.ui.UIManager.Get.ButtonBar.Update(ControllerManager.GetController(0),
            UserPersistence.Get.p1ButtonMap, _popup.buttonBarConfig);
    }

    private static void CreateMenu(UIPage uiPage)
    {
        var stages = new Il2CppSystem.Collections.Generic.List<StageSelectOverrideOptions>();
        foreach (var stage in Data.Global.Stages)
        {
            stages.Add(stage.Key);
        }

        var itemList = new Il2CppSystem.Collections.Generic.List<DefaultMenuOptions>();
        itemList.Add(DefaultMenuOptions.Disabled);
        itemList.Add(DefaultMenuOptions.Enabled);
        var items = itemList.TryCast<Il2CppSystem.Collections.Generic.IList<DefaultMenuOptions>>();
        foreach (var item in Data.Global.Stages)
        {
            if (item.Key == StageSelectOverrideOptions.Random ||
                item.Key == StageSelectOverrideOptions.Disabled) continue;

            var selector = uiPage.AddItem<MenuListSelector<DefaultMenuOptions>>(item.Value);
            selector.Items = items;
            selector.CurrentItem = StageSelectOverride.RandomStages.Contains(item)
                ? DefaultMenuOptions.Enabled
                : DefaultMenuOptions.Disabled;
            selector.LocalizedText = item.Label;
            selector.OnValueChanged = (Action<DefaultMenuOptions, DefaultMenuOptions>)((newValue, _) =>
            {
                StageSelectOverride.Instance.UpdateRandomStageSelect(item.Value,
                    newValue == DefaultMenuOptions.Enabled);
                if (!_bulkUpdate)
                {
                    UpdateButtonBarConfig();
                }
            });
            _mapSelectors.Add(selector);
        }
    }

    static bool Prefix(UISpectateOptions __instance)
    {
        if (_popup.Pointer != __instance.Pointer) return true;

        UIMenuComponentGenerator uiMenuGenerator =
            new UIMenuComponentGenerator(__instance.transform.FindByName<Transform>("templates/menuComponents"));
        var mainPage = new UIPage(__instance.transform.FindByName<Transform>("root"), uiMenuGenerator, "buttonRoot");
        var headerText = __instance.transform.FindByName<LocalizedText>("root/buttonRoot/headerText");
        headerText.localizedText = _headerText;
        __instance.mainPage = mainPage;

        CreateMenu(__instance.mainPage);

        __instance.startingSelection = __instance.mainPage.GetDefaultSelection();
        __instance.InputLayer = new EventSystemLayer("GrimUIRandomStageFilter");
        __instance.mainPage.CreateChain(true, true, __instance.InputLayer);
        __instance.EventSystem.RegisterLayer(__instance.InputModule, __instance.InputLayer,
            __instance.EventSystem.MaxPriority + 1, __instance.Controllers);
        __instance.SetOnShowCallback((Action)(() =>
        {
            __instance.SetOnCancelCallback((Action<ILayeredEventData>)(_ => { __instance.CloseWindow(); }));
            __instance.AddButtonCallback(MenuButton.XboxY, (UnityAction<ILayeredEventData>)ToggleAllStages);
            nway.gameplay.ui.UIManager.Get.ButtonBar.Push(ControllerManager.GetController(0),
                UserPersistence.Get.p1ButtonMap, __instance.buttonBarConfig);
            __instance.buttonBarConfig.SetLocalizedText(ButtonBarItem.Cancel,
                UITextManager.Get.GetUIText("UIText_Back_01"));
            __instance.buttonBarConfig.SetLocalizedText(ButtonBarItem.NavUpDown,
                UITextManager.Get.GetUIText("UIText_Move_01"));


            UpdateButtonBarConfig();
            __instance.EventSystem.SetSelectedGameObject(__instance.startingSelection.Selectable,
                __instance.InputLayer);
        }));
        __instance.SetOnHideCallback((Action)(() =>
        {
            __instance.OnHide();
            if (_callback != null)
            {
                _callback();
            }
        }));
        __instance.SetOnCancelCallback((Action<ILayeredEventData>)(eventData =>
        {
            __instance.OnCancel(eventData);
            if (_callback != null)
            {
                _callback();
            }
        }));

        return false;
    }
}