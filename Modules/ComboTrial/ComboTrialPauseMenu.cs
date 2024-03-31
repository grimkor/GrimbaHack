using System;
using HarmonyLib;
using nway;
using nway.gameplay.ui;
using nway.ui;
using UnityEngine;
using UnityEngine.Events;

namespace GrimbaHack.Modules.ComboTrial;

[HarmonyPatch(typeof(UITrainingOptions), nameof(UITrainingOptions.OnInitializeComponents))]
public class ComboTrialPauseMenu
{
    private static UITrainingOptions uit;

    static bool Prefix(UITrainingOptions __instance)
    {
        if (!ComboTrialManager.Instance.IsComboTrial) return true;
        uit = __instance;

        foreach (var transform in uit.transform.GetComponentsInChildren<Transform>())
        {
            if (transform.name.Contains("health"))
            {
                transform.SetVisible(false);
            }
        }

        uit.stack = new UIStackedMenu(uit.EventSystem);
        uit.SetOnShowCallback((Action)OnShow);
        uit.SetOnHideCallback((Action)OnHide);
        var uiMenuGenerator =
            new UIMenuComponentGenerator(uit.transform.FindByName<Transform>("templates/menuComponents"));
        uit.mainPage = new UIPage(uit.transform.FindByName<Transform>("mainRoot"), uiMenuGenerator, "buttonRoot");

        var resumeButton = GenerateResumeButton();
        GenerateNextTrialButton();
        GeneratePreviousTrialButton();
        GenerateReturnToTrialSelectButton();
        GenerateExitButton();

        uit.mainPage.CreateChain(true, true, uit.InputLayer);
        uit.mainPage.SetVisible(false);
        uit.startingSelection = resumeButton;

        return false;
    }

    private static MenuSubmit GenerateNextTrialButton()
    {
        var nextTrialButton =
            uit.mainPage.AddItem<MenuSubmit>("nextTrialButton");
        nextTrialButton.LocalizedText = "Next Trial";
        nextTrialButton.SetOnSubmit((UnityAction<ILayeredEventData>)((ILayeredEventData data) =>
        {
            data.Use();
            var changed = ComboTrialManager.Instance.SetToNextTrial();
            if (changed)
            {
                uit.CloseWindow();
                GameManager.Get.RequestUnpauseApp();
                ComboTrialManager.Instance.LoadTrialFromMatch();
            }
        }));
        return nextTrialButton;
    }

    private static MenuSubmit GeneratePreviousTrialButton()
    {
        var previousTrialButton =
            uit.mainPage.AddItem<MenuSubmit>("previousTrialButton");
        previousTrialButton.LocalizedText = "Previous Trial";
        previousTrialButton.SetOnSubmit((UnityAction<ILayeredEventData>)((ILayeredEventData data) =>
        {
            data.Use();
            var changed = ComboTrialManager.Instance.SetToPreviousTrial();
            if (changed)
            {
                uit.CloseWindow();
                GameManager.Get.RequestUnpauseApp();
                ComboTrialManager.Instance.LoadTrialFromMatch();
            }
        }));
        return previousTrialButton;
    }

    private static MenuSubmit GenerateResumeButton()
    {
        var resumeButton =
            uit.mainPage.AddItem<MenuSubmit>("ResumeButton");
        resumeButton.LocalizedText = UITextManager.Get.GetUIText("UIText_TrainingSettings_Resume_01");
        resumeButton.SetOnSubmit((UnityAction<ILayeredEventData>)((ILayeredEventData data) =>
        {
            uit.OnSubmitResume(data);
        }));
        return resumeButton;
    }

    private static MenuSubmit GenerateReturnToTrialSelectButton()
    {
        var returnButton =
            uit.mainPage.AddItem<MenuSubmit>("ReturnButton");
        returnButton.LocalizedText = "Return To Trial Select";
        returnButton.SetOnSubmit((UnityAction<ILayeredEventData>)((ILayeredEventData _) => { ReturnToTrialSelect(); }));
        return returnButton;
    }

    private static MenuSubmit GenerateExitButton()
    {
        var exitButton =
            uit.mainPage.AddItem<MenuSubmit>("ExitButton");
        exitButton.LocalizedText = UITextManager.Get.GetUIText("UIText_TrainingSettings_Exit_01");
        exitButton.SetOnSubmit(
            (UnityAction<ILayeredEventData>)((ILayeredEventData data) => { uit.OnSubmitExit(data); }));
        return exitButton;
    }

    private static void OnShow()
    {
        uit.openFrame = Time.frameCount;
        uit.mainPage.SetVisible(false);
        uit.SetOnCancelCallback((UnityAction<ILayeredEventData>)uit.OnCancelPressed);
        uit.SetOnPauseCallback((UnityAction<ILayeredEventData>)uit.OnPausePressed);
        uit.stack.PushPageWithSelection(uit.mainPage, uit.InputLayer, uit.startingSelection, uit.InputLayer,
            uit.mainPage.GetDefaultSelection());
        uit.buttonBarConfig.SetLocalizedText(ButtonBarItem.NavUpDown,
            UITextManager.Get.GetUIText("UIText_Move_01"));
        uit.buttonBarConfig.SetLocalizedText(ButtonBarItem.Confirm,
            UITextManager.Get.GetUIText("UIText_Confirm_01"));
        uit.buttonBarConfig.SetLocalizedText(ButtonBarItem.Cancel, UITextManager.Get.GetUIText("UIText_Back_01"));
        UIManager.Get.ButtonBar.Push(uit.controller, UserPersistence.Get.P1ButtonMap, uit.buttonBarConfig);
        ControllerManager.EnterMenu();
    }

    private static void OnHide()
    {
        uit.stack.Clear();
        UIManager.Get.ButtonBar.Pop(uit.controller, UserPersistence.Get.P1ButtonMap, uit.buttonBarConfig);
        ControllerManager.ExitMenu();
    }

    public static void Teardown()
    {
        var window = BaseUIManager.instance.PopupManager.CheckActiveModalWindow("UI_TrainingOptions");
        if (window == null)
        {
            UIManager.Get.PopupManager.uiPopupAssetList.Remove("UI_TrainingOptions");
        }
    }

    private static void ReturnToTrialSelect()
    {
        uit.CloseWindow();
        GameManager.Get.RequestUnpauseApp();
        ComboTrialManager.Instance.ReturnToTrialSelect();
    }
}

public class IContextWrapper : ILoadingContext
{
    private UILoadingScreenBase _loadingScreenBase;
    public IContextWrapper(IntPtr ptr) : base(ptr)
    {
    }

    public IContextWrapper(UILoadingScreenBase loadingScreenBase) : base(loadingScreenBase.Pointer)
    {
        _loadingScreenBase = loadingScreenBase;
    }

    public override bool IsActive()
    {
        return _loadingScreenBase.IsActive();
    }

    public override void ShowLoadingScreen()
    {
        _loadingScreenBase.ShowLoadingScreen();
    }

    public override void HideLoadingScreen()
    {
        _loadingScreenBase.HideLoadingScreen();
    }

    public override void SetProgress(float progress)
    {
        _loadingScreenBase.SetProgress(progress);
    }

    public override void WaitForStart(Il2CppSystem.Action callback)
    {
        _loadingScreenBase.WaitForStart(callback);
    }

    public override bool ShouldShowLoadingCircle { get; }
}
