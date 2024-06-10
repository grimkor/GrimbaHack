using System;
using HarmonyLib;
using nway;
using nway.gameplay.ui;
using nway.ui;
using UnityEngine;
using UnityEngine.Events;

namespace GrimbaHack.Modules.ComboTrial.UI;

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

        var pageTemplateName = "templates/pages/pageTemplate";

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


        var buttonSettingsButton = uit.mainPage.AddItem<MenuSubmit>("buttonSettingsButton");
        buttonSettingsButton.LocalizedText = UITextManager.Get.GetUIText("UIText_TrainingSettings_ButtonConfig_01");
        buttonSettingsButton.SetOnSubmit((UnityAction<ILayeredEventData>)((ILayeredEventData x) =>
        {
            var args = SceneStartup.Get.GamePlay.controllerMapping.GetButtonMapperArgs(x.Input);
            if (args.ShowP1Controller || args.ShowP2Controller)
            {
                var remap = new UIButtonMapper();
                remap.PopulateSettings(args);
                remap.ShowModalWindow();
            }
        }));

        var infoDisplay = UIManager.Get.UICombatHUD.GetInfoDisplay();
        uit.infoSettingsPage =
            MenuPage.Create(uiMenuGenerator, uit.transform, "infoPage", pageTemplateName, uit.transform);
        uit.infoSettingsPage.Size = new Vector2(500, 500);
        uit.infoSettingsPage.LocalizedHeaderText =
            UITextManager.Get.GetUIText("UIText_TrainingSettings_InfoDisplay_01");

        var infoSettingsButton = uit.mainPage.AddItem<MenuPageNavigator>("infoSettingsButton");
        infoSettingsButton.LocalizedText =
        UITextManager.Get.GetUIText("UIText_TrainingSettings_InfoDisplaySettings_01");
        var displayCommandHistory = uit.infoSettingsPage.AddItem<MenuToggleSelector>("displayCommandHistoryToggle");
        displayCommandHistory.LocalizedText = UITextManager.Get.GetUIText("UIText_TrainingSettings_InfoDisplay_Cmd_01");
        // displayCommandHistory.StateToString = (Func<bool, string>)((x) =>
        //         UITextManager.Get.GetUIText(x
        //             ? "UIText_TrainingSettings_TrainingSettings_ReversePos_On_01"
        //             : "UIText_TrainingSettings_TrainingSettings_ReversePos_Off_01"
        //         )
        //     );
        // displayCommandHistory.StateToString = (Func<bool, string>)((x) =>
        //         UITextManager.Get.GetUIText(x
        //             ? "UIText_TrainingSettings_TrainingSettings_ReversePos_On_01"
        //             : "UIText_TrainingSettings_TrainingSettings_ReversePos_Off_01")
        //     );
        displayCommandHistory.OnValueChanged = (Action<bool, bool>)((current, _) =>
        {
            infoDisplay.ShowCommandHistory(current);
            UserPersistence.Get.Training.displayCommandHistory = current;
        });
        displayCommandHistory.CurrentItem = infoDisplay.IsShowingCommandHistory;


        var displayDamageInfo = uit.infoSettingsPage.AddItem<MenuToggleSelector>("displayDamageInfoToggle");
        displayDamageInfo.LocalizedText = UITextManager.Get.GetUIText("UIText_TrainingSettings_InfoDisplay_Dmg_01");
        // displayDamageInfo.StateToString = (Func<bool, string>)(x =>
        //     UITextManager.Get.GetUIText(x
        //         ? "UIText_TrainingSettings_TrainingSettings_ReversePos_On_01"
        //         : "UIText_TrainingSettings_TrainingSettings_ReversePos_Off_01"));
        displayDamageInfo.OnValueChanged = (Action<bool, bool>)((current, previous) =>
        {
            infoDisplay.ShowDamageInfo(current);
            UserPersistence.Get.Training.displayDamageInfo = current;
        });
        displayDamageInfo.CurrentItem = infoDisplay.IsShowingDamageInfo;


        var displayStunToggle = uit.infoSettingsPage.AddItem<MenuToggleSelector>("displayStunToggle");
        displayStunToggle.LocalizedText = UITextManager.Get.GetUIText("UIText_TrainingSettings_InfoDisplay_Stun_04");
        // displayStunToggle.StateToString = (Func<bool, string>)(x =>
        //     UITextManager.Get.GetUIText(x
        //         ? "UIText_TrainingSettings_TrainingSettings_ReversePos_On_01"
        //         : "UIText_TrainingSettings_TrainingSettings_ReversePos_Off_01"));
        displayStunToggle.OnValueChanged = (Action<bool, bool>)((current, previous) =>
        {
            infoDisplay.ShowStun(current);
            UserPersistence.Get.Training.displayStun = current;
        });
        displayStunToggle.CurrentItem = infoDisplay.IsShowingStun;
        uit.infoSettingsPage.Page.CreateChain(true, true, uit.InputLayer);
        uit.infoSettingsPage.Page.SetVisible(false);
        infoSettingsButton.SetTargetWithSelection(uit.stack, uit.infoSettingsPage.Page,
            uit.infoSettingsPage.Page.GetDefaultSelection(), infoSettingsButton);


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
        uit.infoSettingsPage.Page.SetVisible(false);
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
