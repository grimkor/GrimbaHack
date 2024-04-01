using GrimbaHack.Modules.ComboTrial;
using GrimbaHack.UI.Popup.MainSettings;
using GrimbaHack.UI.Popup.TrainingSettings;
using GrimbaHack.Utility;
using nway.gameplay.ui;
using nway.ui;
using UnityEngine.Events;

namespace GrimbaHack.UI.Managers;

public class GrimUITrainingModeController
{
    public static readonly GrimUITrainingModeController Instance = new();
    private UITrainingOptions _uiTrainingOptions;
    private bool _enabled;

    private GrimUITrainingModeController()
    {
    }

    static GrimUITrainingModeController()
    {
    }

    public void Init()
    {
        OnEnterTrainingMatchActionHandler.Instance.AddPostfix(() =>
            Instance._enabled = !ComboTrialManager.Instance.IsComboTrial);
        OnEnterMainMenuActionHandler.Instance.AddCallback(() => Instance._enabled = false);
        OnUITrainingOptionsOnShowActionHandler.Instance.AddPostfix(uiTrainingOptions =>
            {
                Instance._uiTrainingOptions = uiTrainingOptions;
                uiTrainingOptions.buttonBarConfig.SetLocalizedText(ButtonBarItem.ButtonRB,
                    "Extra Training Options");
                uiTrainingOptions.AddButtonCallback(MenuButton.RightBumper, (UnityAction<ILayeredEventData>)(
                    (ILayeredEventData _) =>
                    {
                        if (Instance._enabled)
                        {
                            // Command list and button configurations are modals and not part of the stack
                            if (uiTrainingOptions.stack.Count <= 1 &&
                                uiTrainingOptions.EventSystem.IsPriority(uiTrainingOptions.InputLayer))
                            {
                                TrainingSettingsPopup.Show(() =>
                                {
                                    uiTrainingOptions.mainPage.GetDefaultSelection()
                                        .Select(uiTrainingOptions.EventSystem, uiTrainingOptions.InputLayer);
                                    uiTrainingOptions.mainPage.SetVisible(true);
                                });
                                uiTrainingOptions.EventSystem.SetSelectedGameObject(null, uiTrainingOptions.InputLayer);
                            }
                        }
                    }));
                uiTrainingOptions.buttonBarConfig.SetLocalizedText(ButtonBarItem.ButtonY,
                    "Combo Recorder");
                uiTrainingOptions.AddButtonCallback(MenuButton.XboxY, (UnityAction<ILayeredEventData>)(
                    (ILayeredEventData _) =>
                    {
                        if (Instance._enabled)
                        {
                            // Command list and button configurations are modals and not part of the stack
                            if (uiTrainingOptions.stack.Count <= 1 &&
                                uiTrainingOptions.EventSystem.IsPriority(uiTrainingOptions.InputLayer))
                            {
                                ComboRecorderSettingsPopup.Show(() =>
                                {
                                    uiTrainingOptions.mainPage.GetDefaultSelection()
                                        .Select(uiTrainingOptions.EventSystem, uiTrainingOptions.InputLayer);
                                    uiTrainingOptions.mainPage.SetVisible(true);
                                });
                                uiTrainingOptions.EventSystem.SetSelectedGameObject(null, uiTrainingOptions.InputLayer);
                            }
                        }
                    }));

                uiTrainingOptions.buttonBarConfig.SetLocalizedText(ButtonBarItem.ButtonLB, "GrimbaHack");
                uiTrainingOptions.AddButtonCallback(MenuButton.LeftBumper, (UnityAction<ILayeredEventData>)(
                    (ILayeredEventData _) =>
                    {
                        if (Instance._enabled)
                        {
                            if (uiTrainingOptions.stack.Count <= 1 &&
                                uiTrainingOptions.EventSystem.IsPriority(uiTrainingOptions.InputLayer))
                            {
                                MainSettingsPopup.Show(() =>
                                {
                                    uiTrainingOptions.mainPage.GetDefaultSelection()
                                        .Select(uiTrainingOptions.EventSystem, uiTrainingOptions.InputLayer);
                                    uiTrainingOptions.mainPage.SetVisible(true);
                                });
                                uiTrainingOptions.EventSystem.SetSelectedGameObject(null, uiTrainingOptions.InputLayer);
                            }
                        }
                    }));
            }
        );

        OnUIStackedMenuPushPage.Instance.AddPostfix((page, _, _) => { Instance.UpdateButtonBar(page.Root.name); });

        OnUIStackedMenuPopPage.Instance.AddPostfix((stackedMenu, _) =>
        {
            Instance.UpdateButtonBar(stackedMenu.stack.Peek()?.page.Root.name);
        });
    }


    private void UpdateButtonBar(string pageName)
    {
        if (!Instance._enabled) return;

        switch (pageName)
        {
            case "mainRoot":
                Instance._uiTrainingOptions.buttonBarConfig.SetLocalizedText(ButtonBarItem.ButtonRB,
                    "Extra Training Options");
                Instance._uiTrainingOptions.buttonBarConfig.SetLocalizedText(ButtonBarItem.ButtonLB, "GrimbaHack");
                break;
            default:
                Instance._uiTrainingOptions.buttonBarConfig.ClearText(ButtonBarItem.ButtonLB);
                Instance._uiTrainingOptions.buttonBarConfig.ClearText(ButtonBarItem.ButtonRB);
                break;
        }

        nway.gameplay.ui.UIManager.Get.ButtonBar.Update(ControllerManager.GetController(0),
            UserPersistence.Get.p1ButtonMap,
            Instance._uiTrainingOptions.buttonBarConfig);
    }
}
