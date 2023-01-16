using System;
using System.Collections.Generic;
using System.Linq;
using GrimbaHack.Data;
using HarmonyLib;
using nway.gameplay.level;
using nway.gameplay.ui;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.UI.Panels;
using Random = System.Random;
using UIManager = GrimbaHack.UI.UIManager;

namespace GrimbaHack.Modules;

public sealed class StageSelectOverride
{
    public static StageSelectOverride Instance { get; private set; }

    static StageSelectOverride()
    {
        Instance = new StageSelectOverride();
    }

    private StageSelectOverride()
    {
    }

    public static bool Enabled { get; set; } = true;
    public static string Stage = Global.Stages[0]?.Value;
    public static List<Stage> RandomStages = Global.Stages.FindAll(x => x.Value != "RANDOM");
    public static ButtonRef RandomStageSelectButton;
    public static SelectModal randomStageSelectModal;

    public static void SetStage(int stageIndex)
    {
        if (stageIndex >= 0 && stageIndex <= Global.Stages.Count - 1)
        {
            var stage = Global.Stages[stageIndex].Value;
            if (stage != "")
            {
                Stage = stage;
            }
        }
    }

    [HarmonyPatch(typeof(SceneLoader), nameof(SceneLoader.RequestSimpleScene))]
    public class PatchRequestSimpleScene
    {
        static bool Prefix(ILoadingContext loadingContext, ref string sceneName, Action<bool> callback)
        {
            if (!Enabled) return true;
            if (Stage == "RANDOM")
            {
                var selection = new Random().Next(0, RandomStages.Count);
                sceneName = RandomStages[selection].Value;
            }
            else
            {
                sceneName = Stage;
            }

            return true;
        }
    }

    public static void CreateUIControls(GameObject ContentRoot, out Action updateCallback)
    {
        randomStageSelectModal = new SelectModal(UIManager.UIBase);
        randomStageSelectModal.Enabled = false;

        // CREATE GROUP
        GameObject stageSelectGroup = UIFactory.CreateUIObject("StageSelectGroup", ContentRoot);
        UIFactory.SetLayoutElement(stageSelectGroup);
        UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(stageSelectGroup, false, false, true, true, padLeft: 25,
            padRight: 0, spacing: 10, childAlignment: TextAnchor.MiddleLeft);

        // CREATE TOGGLE
        UIFactory.CreateToggle(stageSelectGroup, "StageSelectToggle", out Toggle stageSelectToggle,
            out Text stageSelectToggleText, checkHeight: 20, checkWidth: 20);
        stageSelectToggle.enabled = Enabled;
        stageSelectToggle.onValueChanged.AddListener(new Action<bool>(enabled => { Enabled = enabled; }));

        stageSelectToggleText.text = "Enable Stage Select Override";

        // CREATE DROPDOWN
        UIFactory.CreateDropdown(stageSelectGroup, "StageSelectDropdown",
            out Dropdown stageSelectDropdown,
            "Training",
            14,
            i =>
            {
                SetStage(i);
                if (Global.Stages[i].Value == "RANDOM")
                {
                    // TODO: Add random stage selection
                    RandomStageSelectButton.GameObject.active = true;
                }
                else
                {
                    RandomStageSelectButton.GameObject.active = false;
                }
            },
            Global.Stages.ToArray().Select(stage => stage.Label).ToArray()
        );

        // Random Stage Select Button
        RandomStageSelectButton = UIFactory.CreateButton(stageSelectGroup, "RandomStageSelectButton", "Select Stages");
        RandomStageSelectButton.OnClick += () => { randomStageSelectModal.Toggle(); };
        RandomStageSelectButton.GameObject.active = false;


        // LAYOUT ELEMENTS
        UIFactory.SetLayoutElement(stageSelectToggle.gameObject, minWidth: 50, minHeight: 25);
        UIFactory.SetLayoutElement(stageSelectDropdown.gameObject, minWidth: 150, minHeight: 25);
        UIFactory.SetLayoutElement(RandomStageSelectButton.GameObject, minWidth: 50, minHeight: 25);

        // DEFINE UPDATE CALLBACK
        updateCallback = () =>
        {
            if (Enabled && !stageSelectDropdown.gameObject.active)
            {
                stageSelectDropdown.gameObject.active = true;
            }
            else if (!Enabled && stageSelectDropdown.gameObject.active)
            {
                stageSelectDropdown.gameObject.active = false;
            }
        };
    }


    public void UpdateRandomStageSelect(string itemValue, bool value)
    {
        if (value && !RandomStages.Exists(x => x.Value == itemValue))
        {
            var stage = Global.Stages.Find(x => x.Value == itemValue);
            RandomStages.Add(stage);
        }
        else
        {
            RandomStages.Remove(RandomStages.Find(x => x.Value == itemValue));
        }
    }
}

public class SelectModal : PanelBase
{
    public SelectModal(UIBase owner) : base(owner)
    {
    }

    public override string Name => "Select Stages";
    public override int MinWidth => 250;
    public override int MinHeight => 400;
    public override Vector2 DefaultAnchorMin => new Vector2(0.5f, 0.5f);
    public override Vector2 DefaultAnchorMax => new Vector2(0.5f, 0.5f);

    protected override void ConstructPanelContent()
    {
        var verticalGroup = UIFactory.CreateUIObject("RandomStageSelectGroup", ContentRoot);
        UIFactory.SetLayoutElement(verticalGroup);
        UIFactory.SetLayoutGroup<VerticalLayoutGroup>(verticalGroup, false, false, true, true, padLeft: 25,
            padRight: 0, spacing: 5, padTop: 10, childAlignment: TextAnchor.UpperLeft);
        foreach (var item in Global.Stages.FindAll(x => x.Value != "RANDOM"))
        {
            UIFactory.CreateToggle(verticalGroup, item.Value, out Toggle toggle, out Text text);
            text.text = item.Label;

            toggle.onValueChanged.AddListener(new Action<bool>(value =>
            {
                StageSelectOverride.Instance.UpdateRandomStageSelect(item.Value, value);
            }));
            UIFactory.SetLayoutElement(toggle.gameObject, minHeight: 25);
        }
    }
}