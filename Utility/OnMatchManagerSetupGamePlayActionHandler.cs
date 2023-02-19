using System;
using System.Collections.Generic;
using HarmonyLib;
using nway.gameplay.match;

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
    private List<Action> callbacks = new();

    public void AddCallback(Action callback)
    {
        Instance.callbacks.Add(callback);
    }
    
    public static void Prefix()
    {
        foreach (var callback in Instance.callbacks)
        {
            callback();
        }
    }
}