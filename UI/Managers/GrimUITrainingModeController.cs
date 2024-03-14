using GrimbaHack.UI.Pages;
using GrimbaHack.Utility;
using nway.gameplay.ui;
using nway.ui;
using UnityEngine;
using UnityEngine.Events;

namespace GrimbaHack.UI.Managers;

public class GrimUITrainingModeController : MonoBehaviour
{
    public static readonly GrimUITrainingModeController Instance = new();
    private GrimUITrainingModeSettings _trainingModePage;
    private GrimUIMainSettings _mainSettingsPage;
    private UITrainingOptions _uiTrainingOptions;

    private GrimUITrainingModeController()
    {
    }

    static GrimUITrainingModeController()
    {
    }

    private void Awake()
    {
        OnUITrainingOptionsOnHideActionHandler.Instance.AddPostfix(_ =>
        {
            Instance._trainingModePage.Hide(true);
            enabled = false;
        });
        OnUITrainingOptionsOnShowActionHandler.Instance.AddPostfix(uiTrainingOptions =>
            {
                Instance._uiTrainingOptions = uiTrainingOptions;
                Instance._mainSettingsPage =
                    uiTrainingOptions.transform.gameObject.AddComponent<GrimUIMainSettings>();
                Instance._trainingModePage =
                    uiTrainingOptions.transform.gameObject.AddComponent<GrimUITrainingModeSettings>();
                Instance._trainingModePage.Init(uiTrainingOptions);
                Instance._mainSettingsPage.Init(uiTrainingOptions);
                Instance._trainingModePage.Hide(false);
                Instance._mainSettingsPage.Hide();
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
                    (ILayeredEventData eventData) =>
                    {
                        // Command list and button configurations are modals and not part of the stack
                        if (uiTrainingOptions.stack.Count <= 1 && nway.gameplay.ui.UIManager.instance.PopupManager
                                .activeModalUINameStack.Count <= 1)
                        {
                            Instance?._mainSettingsPage.Show(eventData);
                        }
                        else if ((bool)uiTrainingOptions.stack.stack.Peek()?.page.Root.name?.StartsWith("GrimUI"))
                        {
                            uiTrainingOptions.stack.PopPage(uiTrainingOptions.EventSystem);
                            Instance?._mainSettingsPage.Show(eventData);
                        }
                    }));
                enabled = true;
            }
        );
        enabled = false;
    }

    private void Update()
    {
        if (Instance._uiTrainingOptions.stack.Count == 1)
        {
            Instance._uiTrainingOptions.buttonBarConfig.SetLocalizedText(ButtonBarItem.ButtonLB, "GrimbaHack");
            Instance._uiTrainingOptions.buttonBarConfig.SetLocalizedText(ButtonBarItem.ButtonRB,
                "Extra Training Options");
            Instance.UpdateButtonBar();
            return;
        }

        switch (Instance._uiTrainingOptions.stack.stack.Peek().page.Root.name)
        {
            case "GrimUIMainSettings":
                Instance._uiTrainingOptions.buttonBarConfig.ClearText(ButtonBarItem.ButtonLB);
                Instance._uiTrainingOptions.buttonBarConfig.SetLocalizedText(ButtonBarItem.ButtonRB,
                    "Extra Training Options");
                Instance.UpdateButtonBar();
                break;
            case "GrimUITrainingModeSettings":
                Instance._uiTrainingOptions.buttonBarConfig.ClearText(ButtonBarItem.ButtonRB);
                Instance._uiTrainingOptions.buttonBarConfig.SetLocalizedText(ButtonBarItem.ButtonLB, "GrimbaHack");
                Instance.UpdateButtonBar();
                break;
            default:
                Instance._uiTrainingOptions.buttonBarConfig.ClearText(ButtonBarItem.ButtonRB);
                Instance._uiTrainingOptions.buttonBarConfig.ClearText(ButtonBarItem.ButtonLB);
                Instance.UpdateButtonBar();
                break;
        }
    }

    private void UpdateButtonBar()
    {
        nway.gameplay.ui.UIManager.Get.ButtonBar.Update(ControllerManager.GetController(0),
            UserPersistence.Get.p1ButtonMap,
            Instance._uiTrainingOptions.buttonBarConfig);
    }
}