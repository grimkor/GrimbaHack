using System;
using System.Collections.Generic;
using HarmonyLib;
using nway.gameplay.ui;

namespace GrimbaHack.Utility;

[HarmonyPatch(typeof(UIHeroSelect), nameof(UIHeroSelect.OnZordSelected))]
public class OnZordSelectedActionHandler
{
    private OnZordSelectedActionHandler()
    {
    }

    static OnZordSelectedActionHandler()
    {
        Instance = new OnZordSelectedActionHandler();
    }

    public static OnZordSelectedActionHandler Instance { get; private set; }
    private List<Action<UIHeroSelect, UIHeroSelect.Team, int>> callbacks = new();

    public void AddPrefixCallback(Action<UIHeroSelect, UIHeroSelect.Team, int> callback)
    {
        Instance.callbacks.Add(callback);
    }

    public static void Prefix(ref UIHeroSelect __instance, UIHeroSelect.Team team, int zordIndex)
    {
        foreach (Action<UIHeroSelect, UIHeroSelect.Team, int> callback in Instance.callbacks)
        {
            callback(__instance, team, zordIndex);
        }
    }
}