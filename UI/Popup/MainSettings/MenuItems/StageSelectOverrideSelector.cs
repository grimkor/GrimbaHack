using System;
using System.Collections.Generic;
using GrimbaHack.Data;
using GrimbaHack.Modules;
using nway.gameplay.ui;
using nway.ui;
using UnityEngine.Events;

namespace GrimbaHack.UI.Popup.MainSettings.MenuItems;

public class StageSelectOverrideSelectorNew
{
    private static bool _bulkUpdate;
    private static List<MenuListSelector<DefaultMenuOptions>> _mapSelectors = new();
    private static readonly Il2CppSystem.Collections.Generic.List<StageSelectOverrideOptions> Stages;
    private readonly UIMenuComponentGenerator _uiGenerator;
    private readonly UISpectateOptions _uiSpectateOptions;
    private readonly UIPage _page;
    private readonly UIStackedMenu _stack;

    static StageSelectOverrideSelectorNew()
    {
        Stages = new Il2CppSystem.Collections.Generic.List<StageSelectOverrideOptions>();
        foreach (var stage in Data.Global.Stages)
        {
            Stages.Add(stage.Key);
        }
    }

    public StageSelectOverrideSelectorNew(UIMenuComponentGenerator uiMenuGenerator, UISpectateOptions __instance,
        UIPage page, UIStackedMenu stack)
    {
        _uiGenerator = uiMenuGenerator;
        _uiSpectateOptions = __instance;
        _page = page;
        _stack = stack;
    }

    public MenuPage Generate()
    {
        var stageOverridePage = MenuPage.Create(_uiGenerator, _uiSpectateOptions.transform, "randomSelectOptionsPage",
            MainSettingsPopup.PageTemplateName, _uiSpectateOptions.transform);
        stageOverridePage.HeaderText = "Random Stage Filter";
        stageOverridePage.Size = new(600, 800);
        CreateRandomSelectOptions(stageOverridePage.Page);
        stageOverridePage.Page.CreateChain(true, true, _uiSpectateOptions.InputLayer);
        stageOverridePage.Page.SetVisible(false);
        stageOverridePage.Page.onShow += (Action)UpdateButtonBarConfig;

        var selector = _page.AddItem<MenuListSelector<StageSelectOverrideOptions>>("stageSelectOverrideSelector");
        selector.LocalizedText = "Stage Select Override";
        selector.Items = Stages.TryCast<Il2CppSystem.Collections.Generic.IList<StageSelectOverrideOptions>>();
        selector.CurrentItem = StageSelectOverride.Stage.Key;
        selector.OnValueChanged = (Action<StageSelectOverrideOptions, StageSelectOverrideOptions>)((newValue, _) =>
        {
            var stage = Data.Global.Stages.Find(x => x.Key == newValue);
            if (stage != null)
            {
                StageSelectOverride.SetStage(stage);
                SetButtonConfigVisibility(stage.Key == StageSelectOverrideOptions.Random);
            }
        });

        selector.selectable.SetOnSelect((Action<ILayeredEventData>)(_ =>
        {
            SetButtonConfigVisibility(StageSelectOverride.Stage.Key == StageSelectOverrideOptions.Random);
        }));
        selector.selectable.SetOnDeselect((Action<ILayeredEventData>)(_ => { SetButtonConfigVisibility(false); }));

        _uiSpectateOptions.AddButtonCallback(MenuButton.XboxY,
            (UnityAction<ILayeredEventData>)((ILayeredEventData eventData) =>
            {
                if (_stack.stack.Peek().page.Root.name == stageOverridePage.Page.Root.name)
                {
                    ToggleAllStages();
                }

                else if (StageSelectOverride.Stage.Key == StageSelectOverrideOptions.Random)
                {
                    if (_uiSpectateOptions.EventSystem.IsPriority(_uiSpectateOptions.InputLayer) &&
                        _uiSpectateOptions.EventSystem.CurrentSelectedByLayer(_uiSpectateOptions.InputLayer).name ==
                        "stageSelectOverrideSelector")
                    {
                        _stack.PushPageWithSelection(stageOverridePage.Page, eventData.Layer,
                            stageOverridePage.Page.GetDefaultSelection().Selectable, eventData.Layer,
                            _uiSpectateOptions.EventSystem.CurrentSelectedByLayer(_uiSpectateOptions.InputLayer));
                    }
                }
            }));
        return stageOverridePage;
    }


    private void SetButtonConfigVisibility(bool visible)
    {
        if (visible)
        {
            _uiSpectateOptions.buttonBarConfig.SetLocalizedText(ButtonBarItem.ButtonY, "Select Random Stages");
            nway.gameplay.ui.UIManager.Get.ButtonBar.Update(ControllerManager.GetController(0),
                UserPersistence.Get.p1ButtonMap,
                _uiSpectateOptions.buttonBarConfig);
        }
        else
        {
            _uiSpectateOptions.buttonBarConfig.ClearText(ButtonBarItem.ButtonY);
        }

        nway.gameplay.ui.UIManager.Get.ButtonBar.Update(ControllerManager.GetController(0),
            UserPersistence.Get.p1ButtonMap,
            _uiSpectateOptions.buttonBarConfig);
    }

    private void CreateRandomSelectOptions(UIPage uiPage)
    {
        var stages = new Il2CppSystem.Collections.Generic.List<StageSelectOverrideOptions>();
        foreach (var stage in Data.Global.Stages)
        {
            stages.Add(stage.Key);
        }

        var itemList = new Il2CppSystem.Collections.Generic.List<DefaultMenuOptions>();
        itemList.Add(DefaultMenuOptions.Disabled);
        itemList.Add(DefaultMenuOptions.Enabled);
        var items = itemList.TryCast<Il2CppSystem.Collections.Generic.IList<DefaultMenuOptions>>();
        foreach (var item in Data.Global.Stages)
        {
            if (item.Key == StageSelectOverrideOptions.Random ||
                item.Key == StageSelectOverrideOptions.Disabled) continue;

            var selector = uiPage.AddItem<MenuListSelector<DefaultMenuOptions>>(item.Value);
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
                    UpdateButtonBarConfig();
                }
            });
            _mapSelectors.Add(selector);
        }
    }

    void UpdateButtonBarConfig()
    {
        if (AllStagesEnabled())
        {
            _uiSpectateOptions.buttonBarConfig.SetLocalizedText(ButtonBarItem.ButtonY, "Disable All");
        }
        else
        {
            _uiSpectateOptions.buttonBarConfig.SetLocalizedText(ButtonBarItem.ButtonY, "Enable All");
        }

        nway.gameplay.ui.UIManager.Get.ButtonBar.Update(ControllerManager.GetController(0),
            UserPersistence.Get.p1ButtonMap, _uiSpectateOptions.buttonBarConfig);
    }

    private bool AllStagesEnabled()
    {
        return StageSelectOverride.RandomStages.Count == Data.Global.Stages.Count - 1;
    }

    private void ToggleAllStages()
    {
        _bulkUpdate = true;
        var state = !AllStagesEnabled();
        foreach (var selector in _mapSelectors)
        {
            if (selector.CurrentItem == DefaultMenuOptions.Enabled != state)
            {
                selector.CurrentItem = state ? DefaultMenuOptions.Enabled : DefaultMenuOptions.Disabled;
            }
        }

        _bulkUpdate = false;
        UpdateButtonBarConfig();
    }
}
