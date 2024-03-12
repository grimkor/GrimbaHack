using System;
using nway;
using nway.gameplay.ui;
using nway.ui;
using UnityEngine;

namespace GrimbaHack.UI.Pages;

public class GrimUIPage : MonoBehaviour
{
    private bool _initCalled;
    protected UIWindow Window;
    protected UIStackedMenu Stack;
    protected MenuPage Menu;
    internal Action GoBackCallback;
    protected ButtonBarConfig ButtonBarConfig;

    internal string HeaderText = "GrimbaHack";

    public virtual void Init(UISettings uiSettings)
    {
        _initCalled = true;
        Window = uiSettings;
        Stack = uiSettings.stack;
        ButtonBarConfig = uiSettings.buttonBarConfig;
        GoBackCallback = () => uiSettings.GoBack();
        CreateMenuPage();
        SetupButtonListeners();
    }

    public virtual void Init(UITrainingOptions uiTrainingOptions)
    {
        _initCalled = true;
        Window = uiTrainingOptions;
        Stack = uiTrainingOptions.stack;
        ButtonBarConfig = uiTrainingOptions.buttonBarConfig;
        GoBackCallback = () => uiTrainingOptions.GoBack();
        CreateMenuPage();
        SetupButtonListeners();
    }

    public void Show(ILayeredEventData eventData)
    {
        if (!_initCalled)
        {
            throw new Exception(".Init() has not been called for this UI Page");
        }

        Menu.Page.CreateChain(true, true, eventData.Layer);
        Stack.PushPageWithSelection(Menu.Page, eventData.Layer,
            Menu.Page.GetDefaultSelection().Selectable, eventData.Layer,
            Window.EventSystem.CurrentSelectedByLayer(eventData.Layer));
    }

    private void CreateMenuPage()
    {
        var pageTemplateName = "templates/pages/pageTemplate";
        var uiMenuGenerator =
            new UIMenuComponentGenerator(
                Window.transform.FindByName<Transform>("templates/menuComponents"));
        Menu = MenuPage.Create(uiMenuGenerator, Window.transform, GetType().Name,
            pageTemplateName,
            Window.transform);
        Menu.Size = new Vector2(600, 400);
        Menu.LocalizedHeaderText = HeaderText;
        Populate();
    }

    public virtual void Hide()
    {
        Menu?.Page.SetVisible(false);
    }

    protected virtual void SetupButtonListeners()
    {
        throw new NotImplementedException();
    }

    protected virtual void Populate()
    {
        throw new NotImplementedException();
    }
}