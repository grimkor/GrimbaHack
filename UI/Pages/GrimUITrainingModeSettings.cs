using System;
using GrimbaHack.Modules;
using GrimbaHack.UI.Elements;
using GrimbaHack.UI.MenuItems;
using GrimbaHack.Utility;
using nway.gameplay.ui;
using nway.ui;

namespace GrimbaHack.UI.Pages
{
    public class GrimUITrainingModeSettings : GrimUIPage
    {
        private UIPage Page => Menu?.Page;
        private MenuRangeSelector _rangeSelector;
        private TrainingModeSelectables _currentlySelected;
        private bool _listenersSetup;

        private void SetCurrentlySelected(TrainingModeSelectables selectable)
        {
            _currentlySelected = selectable;
        }

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
            Window.AddButtonCallback(MenuButton.XboxY, (Action<ILayeredEventData>)(_ =>
                    {
                        switch (_currentlySelected)
                        {
                            case TrainingModeSelectables.GameSpeed:
                            default:
                                _rangeSelector.CurrentValue = 100;
                                _rangeSelector.slider.Set(1);
                                SimulationSpeed.SetSpeed(100);
                                break;
                            case TrainingModeSelectables.None:
                                break;
                        }
                    }
                ));
            Menu.Page.SetOnShow((Action)SetupButtonBar);
            Menu.Page.SetOnHide((Action)(() =>
            {
                ButtonBarConfig.ClearText(ButtonBarItem.ButtonY);
                nway.gameplay.ui.UIManager.Get.ButtonBar.Update(ControllerManager.GetController(0),
                    UserPersistence.Get.p1ButtonMap,
                    ButtonBarConfig);
                SetCurrentlySelected(TrainingModeSelectables.None);
            }));
        }


        protected override void Populate()
        {
            CollisionBoxViewerSelector.Generate(Menu);
            ShowFrameDataSelector.Generate(Menu);
            UnlimitedInstallSelector.Generate(Menu);
            _rangeSelector = GameSpeedSlider.Generate(Menu);
            _rangeSelector.selectable.SetOnSelect((Action<ILayeredEventData>)(eventData =>
            {
                SetCurrentlySelected(TrainingModeSelectables.GameSpeed);
                ButtonBarConfig.SetLocalizedText(ButtonBarItem.ButtonY, "Reset Value");
                nway.gameplay.ui.UIManager.Get.ButtonBar.Update(eventData.Input, UserPersistence.Get.p1ButtonMap,
                    ButtonBarConfig);
            }));
            _rangeSelector.selectable.SetOnDeselect((Action<ILayeredEventData>)(eventData =>
            {
                SetCurrentlySelected(TrainingModeSelectables.None);
                ButtonBarConfig.ClearText(ButtonBarItem.ButtonY);
                nway.gameplay.ui.UIManager.Get.ButtonBar.Update(eventData.Input, UserPersistence.Get.p1ButtonMap,
                    ButtonBarConfig);
            }));
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

        public override void Hide()
        {
            SetCurrentlySelected(TrainingModeSelectables.None);
            base.Hide();
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