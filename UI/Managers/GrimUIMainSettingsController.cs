using GrimbaHack.UI.Popup.MainSettings;
using GrimbaHack.Utility;
using nway.gameplay.ui;
using nway.ui;
using UnityEngine.Events;

namespace GrimbaHack.UI.Managers;

public class GrimUIMainSettingsController
{
    public static readonly GrimUIMainSettingsController Instance = new();
    private bool _enabled;
    private UISettings _uiSettings;

    private GrimUIMainSettingsController()
    {
    }

    static GrimUIMainSettingsController()
    {
    }

    public static void Init()
    {
        OnUISettingsOnHideActionHandler.Instance.AddPostfix(_ =>
        {
            Instance._enabled = false;
        });
        OnUISettingsOnShowActionHandler.Instance.AddPrefix(uiSettings =>
            {
                Instance._enabled = true;
                Instance._uiSettings = uiSettings;

                uiSettings.AddButtonCallback(MenuButton.LeftBumper, (UnityAction<ILayeredEventData>)(
                    (ILayeredEventData _) =>
                    {
                        if (uiSettings.stack.Count <= 1 && uiSettings.EventSystem.IsPriority(uiSettings.InputLayer))
                        {
                            MainSettingsPopup.Show(() =>
                            {
                                uiSettings.mainPage.GetDefaultSelection()
                                    .Select(uiSettings.EventSystem, uiSettings.InputLayer);
                                uiSettings.mainPage.SetVisible(true);
                            });
                            uiSettings.EventSystem.SetSelectedGameObject(null, uiSettings.InputLayer);
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
            case "root":
                Instance._uiSettings.buttonBarConfig.SetLocalizedText(ButtonBarItem.ButtonLB, "GrimbaHack");
                break;
            default:
                Instance._uiSettings.buttonBarConfig.ClearText(ButtonBarItem.ButtonLB);
                Instance._uiSettings.buttonBarConfig.ClearText(ButtonBarItem.ButtonRB);
                break;
        }

        nway.gameplay.ui.UIManager.Get.ButtonBar.Update(ControllerManager.GetController(0),
            UserPersistence.Get.p1ButtonMap,
            Instance._uiSettings.buttonBarConfig);
    }
}
