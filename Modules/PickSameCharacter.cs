using System;
using GrimbaHack.Utility;
using HarmonyLib;
using nway.gameplay.ui;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;

namespace GrimbaHack.Modules;

public sealed class PickSameCharacter : CheatPrevention
{
    private PickSameCharacter()
    {
    }

    public static readonly PickSameCharacter Instance = new();
    public new bool Enabled;

    static PickSameCharacter()
    {
        OnUIHeroSelectSelectNextActionHandler.Instance.AddCallback(
            (heroCard, team) =>
            {
                if (Instance.Enabled)
                {
                    heroCard.EnableFor(team, true);
                }
            });
    }

    public static void CreateUIControls(GameObject contentRoot)
    {
        var pickSameCharacterGroup = UIFactory.CreateUIObject("PickSameCharacterGroup", contentRoot);
        UIFactory.SetLayoutElement(pickSameCharacterGroup);
        UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(pickSameCharacterGroup, false, false, true, true, padLeft: 25,
            spacing: 10, childAlignment: TextAnchor.MiddleLeft);
        UIFactory.CreateToggle(pickSameCharacterGroup, "PickSameCharacterToggle", out var pickSameCharacterToggle,
            out var pickSameCharacterToggleLabel);
        pickSameCharacterToggle.isOn = Instance.Enabled;
        pickSameCharacterToggleLabel.text = "Allow Duplicate Characters";
        pickSameCharacterToggle.onValueChanged.AddListener(new Action<bool>((value) => { Instance.Enabled = value; }));

        UIFactory.SetLayoutElement(pickSameCharacterToggle.gameObject, minHeight: 25, minWidth: 50);
    }
}