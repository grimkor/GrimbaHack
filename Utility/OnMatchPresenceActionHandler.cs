using System;
using System.Collections.Generic;
using HarmonyLib;
using nway.gameplay.match;

namespace GrimbaHack.Utility;

[HarmonyPatch(typeof(MatchManager), nameof(MatchManager.CreateOnlinePvPMatch))]
public class OnMatchManagerActionHandler
{
    private OnMatchManagerActionHandler()
    {
    }

    static OnMatchManagerActionHandler()
    {
        Instance = new OnMatchManagerActionHandler();
    }
    public static OnMatchManagerActionHandler Instance { get; set; }
    private readonly List<Action<Match>> _callbacks = new();

    public void AddCallback(Action<Match> callback)
    {
        Instance._callbacks.Add(callback);
    }
    
    public static void Prefix(Match onlineMatch)
    {
        foreach (var callback in Instance._callbacks)
        {
            callback(onlineMatch);
        }
    }
}