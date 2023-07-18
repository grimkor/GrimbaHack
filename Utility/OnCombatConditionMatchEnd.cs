using System;
using System.Collections.Generic;
using ArenaProtocol;
using HarmonyLib;
using nway.gameplay;

namespace GrimbaHack.Utility;

[HarmonyPatch(typeof(MatchEndInfo), "Create", new Type[] { typeof(MatchResult) })]
public class OnCombatConditionMatchEndActionHandler
{
    private OnCombatConditionMatchEndActionHandler()
    {
    }

    static OnCombatConditionMatchEndActionHandler()
    {
        Instance = new OnCombatConditionMatchEndActionHandler();
    }
    public static OnCombatConditionMatchEndActionHandler Instance { get; set; }
    private readonly List<Action<MatchResult>> _callbacks = new();

    public void AddCallback(Action<MatchResult> callback)
    {
        Instance._callbacks.Add(callback);
    }
    
    public static void Prefix(MatchResult matchResult)
    {
        foreach (var callback in Instance._callbacks)
        {
            callback(matchResult);
        }
    }
}