using System;
using GrimbaHack.UI.Elements;
using GrimbaHack.UI.MenuItems;
using nway.gameplay.ui;
using nway.ui;

namespace GrimbaHack.UI.Pages
{
    public class GrimUITrainingModeSettings : GrimUIPage
    {
        private UIPage Page => Menu?.Page;
        private bool _listenersSetup;

        public GrimUITrainingModeSettings()
        {
            HeaderText = "Extra Training Settings";
        }

        protected override void SetupButtonListeners()
        {
            if (_listenersSetup)
            {
                return;
            }

            _listenersSetup = true;
            Menu.Page.SetOnShow((Action)SetupButtonBar);
            Menu.Page.SetOnHide((Action)(() =>
            {
                ButtonBarConfig.ClearText(ButtonBarItem.ButtonY);
                nway.gameplay.ui.UIManager.Get.ButtonBar.Update(ControllerManager.GetController(0),
                    UserPersistence.Get.p1ButtonMap,
                    ButtonBarConfig);
            }));
        }


        protected override void Populate()
        {
            CollisionBoxViewerSelector.Generate(Menu);
            ShowFrameDataSelector.Generate(Menu);
            UnlimitedInstallSelector.Generate(Menu);
            GameSpeedSlider.Generate(Menu, ButtonBarConfig, Window);
            BackButton.Create(Menu, () =>
            {
                ButtonBarConfig.SetLocalizedText(ButtonBarItem.ButtonRB, "Extra Training Options");
                ButtonBarConfig.ClearText(ButtonBarItem.ButtonX);
                ButtonBarConfig.ClearText(ButtonBarItem.ButtonY);
                nway.gameplay.ui.UIManager.Get.ButtonBar.Update(ControllerManager.GetController(0),
                    UserPersistence.Get.p1ButtonMap,
                    ButtonBarConfig);
                GoBackCallback();
            });
        }

        public void Hide(bool parentClosing)
        {
            if (parentClosing)
            {
                _listenersSetup = false;
            }

            Hide();
        }

        public void SetupButtonBar()
        {
            ButtonBarConfig.ClearText(ButtonBarItem.ButtonX);
            ButtonBarConfig.ClearText(ButtonBarItem.ButtonY);
            nway.gameplay.ui.UIManager.Get.ButtonBar.Update(ControllerManager.GetController(0),
                UserPersistence.Get.p1ButtonMap,
                ButtonBarConfig);
        }
    }
}