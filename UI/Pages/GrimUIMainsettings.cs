using System;
using GrimbaHack.Modules;
using GrimbaHack.UI.Elements;
using GrimbaHack.UI.MenuItems;
using GrimbaHack.Utility;
using nway;
using nway.gameplay.ui;
using nway.ui;
using UnityEngine;
using UnityEngine.Events;

namespace GrimbaHack.UI.Pages;

public class GrimUIMainSettings : GrimUIPage
{
    private UIPage Page => Menu.Page;
    private MenuPage _twitchPage;
    private int frameCounter = 0;
    private UISettings _uiSettings;

    public GrimUIMainSettings()
    {
        HeaderText = "GrimbaHack Settings";
    }


    public override void Init(UISettings uiSettings)
    {
        _uiSettings = uiSettings;
        base.Init(uiSettings);
    }

    public override void Hide()
    {
        Menu?.Page.SetVisible(false);
        _twitchPage?.Page.SetVisible(false);
    }

    protected override void SetupButtonListeners()
    {
    }

    protected override void Populate()
    {
        StageSelectOverrideSelector.Generate(Menu, _uiSettings, ButtonBarConfig, Stack);
        SameCharacterSelector.Generate(Menu);
        EnableCameraSelector.Generate(Menu);
        CustomTexturesSelector.Generate(Menu);
        // AddTwitchIntegrationButton(Window, Stack);
        BackButton.Create(Menu, GoBackCallback);
    }

    private void AddTwitchIntegrationButton(UIWindow uiWindow, UIStackedMenu stack)
    {
        var button = Menu.AddItem<MenuSubmit>("twitchMenuButton");
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

    private void AddTournamentModeSelector(MenuPage twitchPage)
    {
        var selector =
            twitchPage.Page.AddItem<MenuListSelector<DefaultMenuOptions>>("tournamentModeSelector");
        var items = new Il2CppSystem.Collections.Generic.List<DefaultMenuOptions>();
        items.Add(DefaultMenuOptions.Enabled);
        items.Add(DefaultMenuOptions.Disabled);
        selector.Items =
            items.TryCast<Il2CppSystem.Collections.Generic.IList<DefaultMenuOptions>>();
        selector.LocalizedText = "Tournament Mode";
        selector.CurrentItem = DefaultMenuOptions.Enabled;
    }

    private void AddTwitchLoginMenuButton(MenuPage twitchPage)
    {
        var selector =
            twitchPage.Page.AddItem<MenuSubmit>("twitchLoginPageButton");
        selector.LocalizedText = "Login";
        selector.SetOnSubmit((UnityAction<ILayeredEventData>)((ILayeredEventData _) =>
        {
            Plugin.Log.LogInfo("Clicked Twitch Login Button");
        }));
    }
}