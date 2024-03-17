using System;
using Cpp2IL.Core.Extensions;
using GrimbaHack.Modules;
using HarmonyLib;
using JetBrains.Annotations;
using nway;
using nway.gameplay.ui;
using nway.ui;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GrimbaHack.Utility;

[HarmonyPatch(typeof(UITrainingOptions), nameof(UITrainingOptions.OnShow))]
public class UITrainingOptionsOnShow
{
    static void Postfix(UITrainingOptions __instance)
    {
        GrimUIMainSettings.Hide();
        GrimUITrainingModeSettings.Hide();
        if (!GrimUITrainingModeSettings.IsShowing && !GrimUIMainSettings.IsShowing)
        {
            __instance.buttonBarConfig.SetLocalizedText(ButtonBarItem.ButtonLB, "GrimbaHack");
            __instance.AddButtonCallback(MenuButton.LeftBumper, (UnityAction<ILayeredEventData>)(
                (ILayeredEventData eventData) =>
                {
                    if (__instance.stack.Count > 1)
                    {
                        __instance.stack.PopPage(__instance.EventSystem);
                    }

                    GrimUIMainSettings.Show(__instance, eventData);
                }));

            __instance.buttonBarConfig.SetLocalizedText(ButtonBarItem.ButtonRB, "Extra Training Options");
            __instance.AddButtonCallback(MenuButton.RightBumper, (UnityAction<ILayeredEventData>)(
                (ILayeredEventData eventData) =>
                {
                    if (__instance.stack.Count > 1)
                    {
                        __instance.stack.PopPage(__instance.EventSystem);
                    }

                    GrimUITrainingModeSettings.Show(__instance, eventData);
                }));
        }
    }
}

public class GrimUITrainingModeSettings
{
    public static bool IsShowing
    {
        get
        {
            if (_menuPage?.Page == null) return false;
            return _menuPage.Page.Root.gameObject.active;
        }
    }

    private static UIPage Page => _menuPage?.Page;
    private static MenuPage _menuPage;

    public static void Show(UITrainingOptions uiWindow, ILayeredEventData eventData)
    {
        if (IsShowing)
        {
            return;
        }

        Show_Internal(uiWindow, eventData, uiWindow.stack, () => uiWindow.GoBack());
    }

    private static void Show_Internal(UIWindow uiWindow, ILayeredEventData eventData, UIStackedMenu stack,
        Action goBackCallback = null)
    {
        var pageTemplateName = "templates/pages/pageTemplate";
        var uiMenuGenerator =
            new UIMenuComponentGenerator(uiWindow.transform.FindByName<Transform>("templates/menuComponents"));
        _menuPage = MenuPage.Create(uiMenuGenerator, uiWindow.transform, "GrimUITrainingModeSettings", pageTemplateName,
            uiWindow.transform);
        if (goBackCallback != null)
        {
            Populate(uiWindow, stack, goBackCallback);
        }

        Page.CreateChain(true, true, eventData.Layer);
        stack.PushPageWithSelection(Page, eventData.Layer,
            Page.GetDefaultSelection().Selectable, eventData.Layer,
            uiWindow.EventSystem.CurrentSelectedByLayer(eventData.Layer));
    }

    static void Populate(UIWindow uiWindow, UIStackedMenu stack, Action onBackCallback)
    {
        if (_menuPage != null)
        {
            _menuPage.Size = new Vector2(600, 400);
            _menuPage.LocalizedHeaderText = "GrimbaHack Training Mode";
        }

        AddCollisionBoxViewerSelector();
        AddShowFrameDataSelector();
        AddUnlimitedInstallTimeSelector();
        AddBackButton(onBackCallback);
    }


    private static void AddTwitchLoginMenuButton(MenuPage twitchPage)
    {
        var selector =
            twitchPage.Page.AddItem<MenuSubmit>("twitchLoginPageButton");
        selector.LocalizedText = "Login";
        selector.SetOnSubmit((UnityAction<ILayeredEventData>)((ILayeredEventData _) =>
        {
            Plugin.Log.LogInfo("Clicked Twitch Login Button");
        }));
    }

    private static void AddUnlimitedInstallTimeSelector()
    {
        var selector =
            Page.AddItem<MenuListSelector<DefaultMenuOptions>>("unlimitedInstallTimeSelector");
        var items = new Il2CppSystem.Collections.Generic.List<DefaultMenuOptions>();
        items.Add(DefaultMenuOptions.Enabled);
        items.Add(DefaultMenuOptions.Disabled);
        selector.Items =
            items.TryCast<Il2CppSystem.Collections.Generic.IList<DefaultMenuOptions>>();
        selector.LocalizedText = "Unlimited Install (Adam/Eric)";
        selector.CurrentItem = DefaultMenuOptions.Disabled;
    }

    private static void AddShowFrameDataSelector()
    {
        var selector =
            Page.AddItem<MenuListSelector<DefaultMenuOptions>>("showFrameDataToggle");
        var items = new Il2CppSystem.Collections.Generic.List<DefaultMenuOptions>();
        items.Add(DefaultMenuOptions.Enabled);
        items.Add(DefaultMenuOptions.Disabled);
        selector.Items =
            items.TryCast<Il2CppSystem.Collections.Generic.IList<DefaultMenuOptions>>();
        selector.LocalizedText = "Show Frame Data";
        selector.CurrentItem = DefaultMenuOptions.Disabled;
    }

    private static void AddBackButton(Action onBackCallback)
    {
        var backButton = _menuPage.AddItem<MenuSubmit>("backButton");
        backButton.LocalizedText = "Back";
        backButton.SetOnSubmit((UnityAction<ILayeredEventData>)((ILayeredEventData _) =>
        {
            if (onBackCallback != null)
            {
                onBackCallback();
            }
        }));
    }

    private static void AddCollisionBoxViewerSelector()
    {
        var selector =
            _menuPage.AddItem<MenuListSelector<DefaultMenuOptions>>("collisionBoxViewerSelector");
        var items = new Il2CppSystem.Collections.Generic.List<DefaultMenuOptions>();
        items.Add(DefaultMenuOptions.Disabled);
        items.Add(DefaultMenuOptions.Enabled);
        selector.Items =
            items.TryCast<Il2CppSystem.Collections.Generic.IList<DefaultMenuOptions>>();
        selector.LocalizedText = "Enable Collision Box Viewer";
        selector.CurrentItem = DefaultMenuOptions.Disabled;
        selector.OnValueChanged =
            new Action<DefaultMenuOptions, DefaultMenuOptions>((newValue, oldValue) =>
            {
                Plugin.Log.LogInfo($"{newValue} > {oldValue}");
                CollisionBoxViewerController.Enabled = newValue == DefaultMenuOptions.Enabled;
            });
    }

    public static void Hide()
    {
        _menuPage?.Page.SetVisible(false);
    }
}