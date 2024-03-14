using System.Collections;
using GrimbaHack.UI.Pages;
using GrimbaHack.Utility;
using nway.gameplay.ui;
using nway.ui;
using UnityEngine;
using UnityEngine.Events;

namespace GrimbaHack.UI.Managers;

public class GrimUITrainingModeController
{
    public static readonly GrimUITrainingModeController Instance = new();
    private GrimUITrainingModeSettings _trainingModePage;
    private GrimUIMainSettings _mainSettingsPage;

    private GrimUITrainingModeController()
    {
    }

    static GrimUITrainingModeController()
    {
    }

    public void Init()
    {
        OnUITrainingOptionsOnHideActionHandler.Instance.AddPostfix(_ => { Instance._trainingModePage.Hide(true); });
        OnUITrainingOptionsOnShowActionHandler.Instance.AddPostfix(uiTrainingOptions =>
            {
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
                        } else if ((bool)uiTrainingOptions.stack.stack.Peek()?.page.Root.name?.StartsWith("GrimUI"))
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
                        } else if ((bool)uiTrainingOptions.stack.stack.Peek()?.page.Root.name?.StartsWith("GrimUI"))
                        {

                            uiTrainingOptions.stack.PopPage(uiTrainingOptions.EventSystem);
                            Instance?._mainSettingsPage.Show(eventData);
                        }
                    }));
            }
        );
    }
}