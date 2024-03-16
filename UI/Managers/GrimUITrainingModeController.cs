using GrimbaHack.UI.Pages;
using GrimbaHack.UI.Popup.MainSettings;
using GrimbaHack.Utility;
using nway.gameplay.ui;
using nway.ui;
using UnityEngine.Events;

namespace GrimbaHack.UI.Managers;

public class GrimUITrainingModeController
{
    public static readonly GrimUITrainingModeController Instance = new();
    private GrimUITrainingModeSettings _trainingModePage;
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
        OnEnterTrainingMatchActionHandler.Instance.AddPostfix(() => _enabled = true);
        OnEnterMainMenuActionHandler.Instance.AddCallback(() => _enabled = false);
        OnUITrainingOptionsOnHideActionHandler.Instance.AddPostfix(_ => { Instance._trainingModePage.Hide(true); });
        OnUITrainingOptionsOnShowActionHandler.Instance.AddPostfix(uiTrainingOptions =>
            {
                Instance._uiTrainingOptions = uiTrainingOptions;
                Instance._trainingModePage =
                    uiTrainingOptions.transform.gameObject.AddComponent<GrimUITrainingModeSettings>();
                Instance._trainingModePage.Init(uiTrainingOptions);
                Instance._trainingModePage.Hide(false);
                uiTrainingOptions.buttonBarConfig.SetLocalizedText(ButtonBarItem.ButtonRB,
                    "Extra Training Options");
                uiTrainingOptions.AddButtonCallback(MenuButton.RightBumper, (UnityAction<ILayeredEventData>)(
                    (ILayeredEventData eventData) =>
                    {
                        // Command list and button configurations are modals and not part of the stack
                        if (uiTrainingOptions.stack.Count <= 1 && nway.gameplay.ui.UIManager.instance.PopupManager
                                .activeModalUINameStack.Count <= 1)
                        {
                            Instance?._trainingModePage.Show(eventData);
                        }
                        else if ((bool)uiTrainingOptions.stack.stack.Peek()?.page.Root.name?.StartsWith("GrimUI"))
                        {
                            uiTrainingOptions.stack.PopPage(uiTrainingOptions.EventSystem);
                            Instance?._trainingModePage.Show(eventData);
                        }
                    }));

                uiTrainingOptions.buttonBarConfig.SetLocalizedText(ButtonBarItem.ButtonLB, "GrimbaHack");
                uiTrainingOptions.AddButtonCallback(MenuButton.LeftBumper, (UnityAction<ILayeredEventData>)(
                    (ILayeredEventData _) =>
                    {
                        if (uiTrainingOptions.stack.Count <= 1 && uiTrainingOptions.EventSystem.IsPriority(uiTrainingOptions.InputLayer))
                        {
                            MainSettingsPopup.Show(() =>
                            {
                                uiTrainingOptions.mainPage.GetDefaultSelection()
                                    .Select(uiTrainingOptions.EventSystem, uiTrainingOptions.InputLayer);
                                uiTrainingOptions.mainPage.SetVisible(true);
                            });
                            uiTrainingOptions.EventSystem.SetSelectedGameObject(null, uiTrainingOptions.InputLayer);
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
        if (!_enabled) return;

        switch (pageName)
        {
            case "mainRoot":
                Instance._uiTrainingOptions.buttonBarConfig.SetLocalizedText(ButtonBarItem.ButtonRB,
                    "Extra Training Options");
                Instance._uiTrainingOptions.buttonBarConfig.SetLocalizedText(ButtonBarItem.ButtonLB, "GrimbaHack");
                break;
            case nameof(GrimUITrainingModeSettings):
                Instance._uiTrainingOptions.buttonBarConfig.ClearText(ButtonBarItem.ButtonRB);
                Instance._uiTrainingOptions.buttonBarConfig.SetLocalizedText(ButtonBarItem.ButtonLB, "GrimbaHack");
                break;
            case nameof(GrimUIMainSettings):
                Instance._uiTrainingOptions.buttonBarConfig.ClearText(ButtonBarItem.ButtonLB);
                Instance._uiTrainingOptions.buttonBarConfig.SetLocalizedText(ButtonBarItem.ButtonRB,
                    "Extra Training Options");
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