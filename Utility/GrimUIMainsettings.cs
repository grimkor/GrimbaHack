using System;
using HarmonyLib;
using nway;
using nway.gameplay.ui;
using nway.ui;
using UnityEngine;
using UnityEngine.Events;

namespace GrimbaHack.Utility;

[HarmonyPatch(typeof(UISettings), nameof(UISettings.OnShow))]
public class UIWindowGetUIPathUISettings
{
    static void Postfix(UISettings __instance)
    {
        if (!GrimUIMainSettings.IsShowing)
        {
            __instance.buttonBarConfig.SetLocalizedText(ButtonBarItem.ButtonLB, "GrimbaHack");
            __instance.AddButtonCallback(MenuButton.LeftBumper, (UnityAction<ILayeredEventData>)(
                (ILayeredEventData eventData) => { GrimUIMainSettings.Show(__instance, eventData); }));
        }
    }
}

public class GrimUIMainSettings()
{
    public static bool IsShowing
    {
        get
        {
            if (_menuPage?.Page == null) return false;
            return _menuPage.Page.Root.gameObject.active;
        }
    }

    private static UIPage Page => _menuPage.Page;
    private static MenuPage _menuPage;
    private static MenuPage _twitchPage;

    public static void Show(UISettings uiWindow, ILayeredEventData eventData)
    {
        if (IsShowing)
        {
            return;
        }

        Show_Internal(uiWindow, eventData, uiWindow.stack, () => uiWindow.GoBack());
    }

    public static void Show(UITrainingOptions uiWindow, ILayeredEventData eventData)
    {
        if (IsShowing)
        {
            return;
        }

        Show_Internal(uiWindow, eventData, uiWindow.stack, () => uiWindow.GoBack());
    }


    public static void Hide()
    {
        _menuPage?.Page.SetVisible(false);
        _twitchPage?.Page.SetVisible(false);
    }

    private static void Show_Internal(UIWindow uiWindow, ILayeredEventData eventData, UIStackedMenu stack,
        Action goBackCallback = null)
    {
        var pageTemplateName = "templates/pages/pageTemplate";
        var uiMenuGenerator =
            new UIMenuComponentGenerator(uiWindow.transform.FindByName<Transform>("templates/menuComponents"));
        _menuPage = MenuPage.Create(uiMenuGenerator, uiWindow.transform, "customMenu", pageTemplateName,
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

    private static void Populate(UIWindow uiWindow, UIStackedMenu stack, Action onBackCallback)
    {
        _menuPage.Size = new Vector2(600, 400);
        _menuPage.LocalizedHeaderText = "GrimbaHack General Settings";

        AddStageSelectOverrideSelector();
        AddSameCharacterSelectSelector();
        AddEnableCameraSelector();
        AddCustomTexturesSelector();
        AddTwitchIntegrationButton(uiWindow, stack);
        AddBackButton(onBackCallback);
    }

    private static void AddTwitchIntegrationButton(UIWindow uiWindow, UIStackedMenu stack)
    {
        var button = _menuPage.AddItem<MenuSubmit>("twitchMenuButton");
        button.LocalizedText = "Twitch Integration";
        button.SetOnSubmit((UnityAction<ILayeredEventData>)((ILayeredEventData eventData) =>
        {
            var pageTemplateName = "templates/pages/pageTemplate";
            var uiMenuGenerator =
                new UIMenuComponentGenerator(uiWindow.transform.FindByName<Transform>("templates/menuComponents"));
            _twitchPage = MenuPage.Create(uiMenuGenerator, uiWindow.transform, "twitchMenu", pageTemplateName,
                uiWindow.transform);
            _twitchPage.Size = new Vector2(600, 400);

            AddTwitchLoginMenuButton(_twitchPage);
            AddTournamentModeSelector(_twitchPage);

            var twitchLayer = new EventSystemLayer("twitch");
            uiWindow.EventSystem.RegisterLayer(uiWindow.InputModule, twitchLayer, uiWindow.EventSystem.MaxPriority,
                uiWindow.Controllers);
            _twitchPage.Page.CreateChain(true, true, twitchLayer);
            _twitchPage.Page.SetOnHide((Action)(() => { uiWindow.EventSystem.RemoveLayer(twitchLayer); }));
            stack.PushPageWithSelection(_twitchPage.Page, twitchLayer,
                _twitchPage.Page.GetDefaultSelection().Selectable, eventData.Layer,
                uiWindow.EventSystem.CurrentSelectedByLayer(eventData.Layer));
        }));
    }

    private static void AddTournamentModeSelector(MenuPage twitchPage)
    {
        var selector =
            twitchPage.Page.AddItem<MenuListSelector<DefaultMenuOptions>>("xsameCharacterSelectSelector");
        var itemsx = new Il2CppSystem.Collections.Generic.List<DefaultMenuOptions>();
        itemsx.Add(DefaultMenuOptions.Enabled);
        itemsx.Add(DefaultMenuOptions.Disabled);
        selector.Items =
            itemsx.TryCast<Il2CppSystem.Collections.Generic.IList<DefaultMenuOptions>>();
        selector.LocalizedText = "Tournament Mode";
        selector.CurrentItem = DefaultMenuOptions.Enabled;
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

    private static void AddEnableCameraSelector()
    {
        var selector =
            Page.AddItem<MenuListSelector<DefaultMenuOptions>>("enableCameraControlSelector");
        var items = new Il2CppSystem.Collections.Generic.List<DefaultMenuOptions>();
        items.Add(DefaultMenuOptions.Enabled);
        items.Add(DefaultMenuOptions.Disabled);
        selector.Items =
            items.TryCast<Il2CppSystem.Collections.Generic.IList<DefaultMenuOptions>>();
        selector.LocalizedText = "Enable Camera Control";
        selector.CurrentItem = DefaultMenuOptions.Disabled;
    }

    private static void AddCustomTexturesSelector()
    {
        var selector =
            Page.AddItem<MenuListSelector<DefaultMenuOptions>>("customTexturesSelector");
        var items = new Il2CppSystem.Collections.Generic.List<DefaultMenuOptions>();
        items.Add(DefaultMenuOptions.Enabled);
        items.Add(DefaultMenuOptions.Disabled);
        selector.Items =
            items.TryCast<Il2CppSystem.Collections.Generic.IList<DefaultMenuOptions>>();
        selector.LocalizedText = "Load Custom Textures";
        selector.CurrentItem = DefaultMenuOptions.Disabled;
    }

    private static void AddSameCharacterSelectSelector()
    {
        var selector =
            Page.AddItem<MenuListSelector<DefaultMenuOptions>>("sameCharacterSelectSelector");
        var items = new Il2CppSystem.Collections.Generic.List<DefaultMenuOptions>();
        items.Add(DefaultMenuOptions.Enabled);
        items.Add(DefaultMenuOptions.Disabled);
        selector.Items =
            items.TryCast<Il2CppSystem.Collections.Generic.IList<DefaultMenuOptions>>();
        selector.LocalizedText = "Allow Same Characters";
        selector.CurrentItem = DefaultMenuOptions.Enabled;
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

    private static void AddStageSelectOverrideSelector()
    {
        var selector =
            _menuPage.AddItem<MenuListSelector<StageSelectOverrideMenuOptions>>("stageOverrideSelector");
        var items = new Il2CppSystem.Collections.Generic.List<StageSelectOverrideMenuOptions>();
        items.Add(StageSelectOverrideMenuOptions.Disabled);
        items.Add(StageSelectOverrideMenuOptions.Enabled);
        selector.Items =
            items.TryCast<Il2CppSystem.Collections.Generic.IList<StageSelectOverrideMenuOptions>>();
        selector.LocalizedText = "Stage Select Override";
        selector.CurrentItem = StageSelectOverrideMenuOptions.Disabled;
        selector.OnValueChanged =
            new Action<StageSelectOverrideMenuOptions, StageSelectOverrideMenuOptions>((x, y) =>
            {
                Plugin.Log.LogInfo($"{x} > {y}");
            });
    }
}