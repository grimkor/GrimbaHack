using System;
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

    public static PickSameCharacter Instance { get; private set; }

    static PickSameCharacter()
    {
        Instance = new PickSameCharacter();
    }


    [HarmonyPatch(typeof(UIHeroSelect), nameof(UIHeroSelect.SelectNext))]
    private class PatchSelectNext
    {
        private static void Prefix(ref UIHeroSelect.UIHeroCard heroCard, UIHeroSelect.Team team)
        {
            if (Enabled)
            {
                heroCard.EnableFor(team, true);
            }
        }
    }

    public static void CreateUIControls(GameObject contentRoot)
    {
        var pickSameCharacterGroup = UIFactory.CreateUIObject("PickSameCharacterGroup", contentRoot);
        UIFactory.SetLayoutElement(pickSameCharacterGroup);
        UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(pickSameCharacterGroup, false, false, true, true, padLeft: 25,
            spacing: 10, childAlignment: TextAnchor.MiddleLeft);
        UIFactory.CreateToggle(pickSameCharacterGroup, "PickSameCharacterToggle", out var pickSameCharacterToggle,
            out var pickSameCharacterToggleLabel);
        pickSameCharacterToggle.isOn = Enabled;
        pickSameCharacterToggleLabel.text = "Enable Picking Same Character Multiple Times";
        pickSameCharacterToggle.onValueChanged.AddListener(new Action<bool>((value) => { Enabled = value; }));

        UIFactory.SetLayoutElement(pickSameCharacterToggle.gameObject, minHeight: 25, minWidth: 50);
    }
}