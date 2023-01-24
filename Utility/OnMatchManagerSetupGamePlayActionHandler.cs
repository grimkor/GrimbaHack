using System;
using System.Collections.Generic;
using HarmonyLib;
using nway.gameplay;
using nway.gameplay.match;
using Action = System.Action;

namespace GrimbaHack.Utility;

[HarmonyPatch(typeof(MatchManager), nameof(MatchManager.SetupGamePlay))]
public class OnMatchManagerSetupGamePlay
{
    private OnMatchManagerSetupGamePlay()
    {
    }

    static OnMatchManagerSetupGamePlay()
    {
        Instance = new OnMatchManagerSetupGamePlay();
    }

    public static OnMatchManagerSetupGamePlay Instance { get; set; }
    private List<Action<Match, string, PlayerControllerMapping>> callbacks = new();

    public void AddCallback(Action<Match, string, PlayerControllerMapping> callback)
    {
        Instance.callbacks.Add(callback);
    }

    public static void Prefix(Match match,
        string pid,
        PlayerControllerMapping controllerMapping)
    {
        foreach (Action<Match, string, PlayerControllerMapping> callback in Instance.callbacks)
        {
            callback(match, pid, controllerMapping);
        }
    }
}