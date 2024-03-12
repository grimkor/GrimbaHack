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

    private GrimUIMainSettingsController()
    {
    }

    static GrimUIMainSettingsController()
    {
    }

    public void Init()
    {
        OnUISettingsOnHideActionHandler.Instance.AddPostfix(_ => { Instance._mainSettingsPage.Hide(); });
        OnUISettingsOnShowActionHandler.Instance.AddPrefix(uiSettings =>
            {
                if (uiSettings.transform.gameObject.GetComponent<GrimUIMainSettings>() == null)
                {
                    Instance._mainSettingsPage =
                        uiSettings.transform.gameObject.AddComponent<GrimUIMainSettings>();
                }

                Instance._mainSettingsPage.Init(uiSettings);
                Instance._mainSettingsPage.Hide();
                uiSettings.buttonBarConfig.SetText(ButtonBarItem.ButtonLB, "GrimbaHack");
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
    }
}