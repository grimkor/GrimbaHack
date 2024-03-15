using System;
using System.Collections.Generic;
using GrimbaHack.Data;
using GrimbaHack.Modules;
using GrimbaHack.UI.Pages;
using GrimbaHack.Utility;
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
        var randomSelectPage = uiSettings.transform.gameObject.AddComponent<RandomStageSelectPage>();
        randomSelectPage.Init(uiSettings);
        randomSelectPage.Hide();
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
                if (stack.stack.Peek().page.Root.name == menuPage.Page.Root.name)
                {
                    SetButtonConfigVisibility(false, buttonBarConfig);
                    randomSelectPage.Show(eventData);
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

public class RandomStageSelectPage : GrimUIPage
{
    private readonly List<MenuListSelector<DefaultMenuOptions>> _mapSelectors = new();
    private bool _bulkUpdate;

    protected override void SetupButtonListeners()
    {
    }

    public override void Init(UISettings uiSettings)
    {
        base.Init(uiSettings);
        Menu.Page.onShow += (Action)(UpdateButtonBarConfig);
    }

    public override void Init(UITrainingOptions uiTrainingOptions)
    {
        base.Init(uiTrainingOptions);
        Menu.Page.onShow += (Action)(UpdateButtonBarConfig);
    }

    protected override void Populate()
    {
        var itemList = new Il2CppSystem.Collections.Generic.List<DefaultMenuOptions>();
        itemList.Add(DefaultMenuOptions.Disabled);
        itemList.Add(DefaultMenuOptions.Enabled);
        var items = itemList.TryCast<Il2CppSystem.Collections.Generic.IList<DefaultMenuOptions>>();
        foreach (var item in Data.Global.Stages)
        {
            if (item.Key == StageSelectOverrideOptions.Random ||
                item.Key == StageSelectOverrideOptions.Disabled) continue;

            var selector = Menu.AddItem<MenuListSelector<DefaultMenuOptions>>(item.Value);
            selector.Items = items;
            selector.CurrentItem = StageSelectOverride.RandomStages.Contains(item)
                ? DefaultMenuOptions.Enabled
                : DefaultMenuOptions.Disabled;
            selector.LocalizedText = item.Label;
            selector.OnValueChanged = (Action<DefaultMenuOptions, DefaultMenuOptions>)((newValue, _) =>
            {
                StageSelectOverride.Instance.UpdateRandomStageSelect(item.Value,
                    newValue == DefaultMenuOptions.Enabled);
                if (!_bulkUpdate)
                {
                    Plugin.Log.LogInfo($"bulkUPdate: {_bulkUpdate}");
                    UpdateButtonBarConfig();
                }
            });
            _mapSelectors.Add(selector);
        }

        Window.AddButtonCallback(MenuButton.XboxY, (UnityAction<ILayeredEventData>)ToggleAllStages);
    }


    // -1 for Random in Global.Stages
    private bool AllStagesEnabled()
    {
        Plugin.Log.LogInfo($"{StageSelectOverride.RandomStages.Count} == {Data.Global.Stages.Count - 1}");
        return StageSelectOverride.RandomStages.Count == Data.Global.Stages.Count - 1;
    }

    private void ToggleAllStages(ILayeredEventData _)
    {
        if (Stack.stack.Peek().page.Root.name != Menu.Page.Root.name)
        {
            return;
        }

        _bulkUpdate = true;
        var state = !AllStagesEnabled();
        Plugin.Log.LogInfo($"Next state is: {state}");
        foreach (var selector in _mapSelectors)
        {
            if (selector.CurrentItem == DefaultMenuOptions.Enabled != state)
            {
                Plugin.Log.LogInfo($"State: {selector.CurrentItem} -> {!state}");
                selector.CurrentItem = state ? DefaultMenuOptions.Enabled : DefaultMenuOptions.Disabled;
            }
        }

        _bulkUpdate = false;
        UpdateButtonBarConfig();
    }

    void UpdateButtonBarConfig()
    {
        if (AllStagesEnabled())
        {
            ButtonBarConfig.SetLocalizedText(ButtonBarItem.ButtonY, "Disable All");
        }
        else
        {
            ButtonBarConfig.SetLocalizedText(ButtonBarItem.ButtonY, "Enable All");
        }

        nway.gameplay.ui.UIManager.Get.ButtonBar.Update(ControllerManager.GetController(0),
            UserPersistence.Get.p1ButtonMap,
            ButtonBarConfig);
    }

    public override void Hide()
    {
        ButtonBarConfig?.ClearText(ButtonBarItem.ButtonY);
        base.Hide();
    }
}