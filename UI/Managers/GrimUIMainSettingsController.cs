using GrimbaHack.UI.Pages;
using GrimbaHack.Utility;
using nway.gameplay.ui;
using nway.ui;
using UnityEngine.Events;

namespace GrimbaHack.UI.Managers;

public class GrimUIMainSettingsController
{
    public static readonly GrimUIMainSettingsController Instance = new();
    private GrimUIMainSettings _mainSettingsPage;
    private bool _enabled;
    private UISettings _uiSettings;

    private GrimUIMainSettingsController()
    {
    }

    static GrimUIMainSettingsController()
    {
    }

    public void Init()
    {
        OnUISettingsOnHideActionHandler.Instance.AddPostfix(_ =>
        {
            _enabled = false;
            Instance._mainSettingsPage.Hide();
        });
        OnUISettingsOnShowActionHandler.Instance.AddPrefix(uiSettings =>
            {
                _enabled = true;
                _uiSettings = uiSettings;
                if (uiSettings.transform.gameObject.GetComponent<GrimUIMainSettings>() == null)
                {
                    Instance._mainSettingsPage =
                        uiSettings.transform.gameObject.AddComponent<GrimUIMainSettings>();
                }

                Instance._mainSettingsPage.Init(uiSettings);
                Instance._mainSettingsPage.Hide();
                uiSettings.AddButtonCallback(MenuButton.LeftBumper, (UnityAction<ILayeredEventData>)(
                    (ILayeredEventData eventData) =>
                    {
                        if (uiSettings.stack.Count > 1)
                        {
                            uiSettings.stack.PopPage(uiSettings.EventSystem);
                        }

                        _mainSettingsPage.Show(eventData);
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
            case nameof(GrimUIMainSettings):
                Instance._uiSettings.buttonBarConfig.ClearText(ButtonBarItem.ButtonLB);
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