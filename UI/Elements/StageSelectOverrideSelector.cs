using System;
using GrimbaHack.Data;
using GrimbaHack.Modules;
using GrimbaHack.UI.Popup.MainSettings;
using nway.gameplay.ui;
using nway.ui;
using UnityEngine.Events;

namespace GrimbaHack.UI.Elements;

static class StageSelectOverrideSelector
{
    private static MenuPage _stageSelectMenuPage;
    private static UIPage _randomFilterPage;

    public static void Generate(MenuPage menuPage, UISettings uiSettings, ButtonBarConfig buttonBarConfig,
        UIStackedMenu stack)
    {
        // var randomSelectPage = uiSettings.transform.gameObject.AddComponent<RandomStageSelectPage>();
        // randomSelectPage.Init(uiSettings);
        // randomSelectPage.Hide();
        var stages = new Il2CppSystem.Collections.Generic.List<StageSelectOverrideOptions>();
        foreach (var stage in Data.Global.Stages)
        {
            stages.Add(stage.Key);
        }

        var selector =
            menuPage.Page.AddItem<MenuListSelector<StageSelectOverrideOptions>>("stageSelectOverrideSelector");
        selector.LocalizedText = "Stage Select Override";
        selector.Items = stages.TryCast<Il2CppSystem.Collections.Generic.IList<StageSelectOverrideOptions>>();
        selector.CurrentItem = StageSelectOverride.Stage.Key;
        selector.OnValueChanged = (Action<StageSelectOverrideOptions, StageSelectOverrideOptions>)((newValue, _) =>
        {
            var stage = Data.Global.Stages.Find(x => x.Key == newValue);
            if (stage != null)
            {
                StageSelectOverride.SetStage(stage);
                SetButtonConfigVisibility(stage.Key == StageSelectOverrideOptions.Random, buttonBarConfig);
            }
        });

        uiSettings.AddButtonCallback(MenuButton.XboxY, (UnityAction<ILayeredEventData>)((ILayeredEventData eventData) =>
        {
            if (StageSelectOverride.Stage.Key == StageSelectOverrideOptions.Random)
            {
                if (uiSettings.EventSystem.IsPriority(uiSettings.InputLayer) &&
                    uiSettings.EventSystem.CurrentSelectedByLayer(uiSettings.InputLayer).name ==
                    "stageSelectOverrideSelector")
                {
                    SetButtonConfigVisibility(false, buttonBarConfig);
                    MainSettingsPopup.Show(() =>
                    {
                        menuPage.Page.SetVisible(true);
                        selector.Select(uiSettings.EventSystem, uiSettings.InputLayer);
                    });
                    uiSettings.EventSystem.SetSelectedGameObject(null, uiSettings.InputLayer);
                    menuPage.Page.SetVisible(false);
                }
            }
        }));
        selector.selectable.SetOnSelect((Action<ILayeredEventData>)(_ =>
        {
            SetButtonConfigVisibility(StageSelectOverride.Stage.Key == StageSelectOverrideOptions.Random,
                buttonBarConfig);
        }));
        selector.selectable.SetOnDeselect((Action<ILayeredEventData>)(_ =>
        {
            SetButtonConfigVisibility(false, buttonBarConfig);
        }));
    }

    public static UISpectateOptions SomeWindow { get; set; }


    private static void SetButtonConfigVisibility(bool visible, ButtonBarConfig buttonBarConfig)
    {
        if (visible)
        {
            buttonBarConfig.SetLocalizedText(ButtonBarItem.ButtonY, "Select Random Stages");
            nway.gameplay.ui.UIManager.Get.ButtonBar.Update(ControllerManager.GetController(0),
                UserPersistence.Get.p1ButtonMap,
                buttonBarConfig);
        }
        else
        {
            buttonBarConfig.ClearText(ButtonBarItem.ButtonY);
        }

        nway.gameplay.ui.UIManager.Get.ButtonBar.Update(ControllerManager.GetController(0),
            UserPersistence.Get.p1ButtonMap,
            buttonBarConfig);
    }
}
