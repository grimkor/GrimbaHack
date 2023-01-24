using System;
using System.Collections.Generic;
using GrimbaHack.Modules;
using HarmonyLib;
using nway.gameplay;
using nway.gameplay.match;
using nway.gameplay.ui;
using Action = System.Action;

namespace GrimbaHack.Utility;

[HarmonyPatch(typeof(AppStateManager), nameof(AppStateManager.SetAppState))]
public class OnEnterTrainingMatchActionHandler
{
    private OnEnterTrainingMatchActionHandler()
    {
    }

    static OnEnterTrainingMatchActionHandler()
    {
        Instance = new OnEnterTrainingMatchActionHandler();
    }

    public static OnEnterTrainingMatchActionHandler Instance { get; set; }
    private List<Action> callbacks = new();
    private List<Action> prefixCallbacks = new();

    public void AddCallback(Action callback)
    {
        Instance.callbacks.Add(callback);
    }

    public void AddPrefixCallback(Action callback)
    {
        Instance.prefixCallbacks.Add(callback);
    }

    public static void Postfix(AppState state)
    {
        if (!Data.Global.IsTrainingMatch()) return;
        foreach (var callback in Instance.callbacks)
        {
            callback();
        }
    }

    public static void Prefix(AppState state)
    {
        if (!Data.Global.IsTrainingMatch()) return;
        foreach (var callback in Instance.prefixCallbacks)
        {
            callback();
        }
    }
}

[HarmonyPatch(typeof(MatchManager), nameof(MatchManager.CreateTrainingMatch))]
public class OnMatchManagerCreateTrainingNextActionHandler
{
    private OnMatchManagerCreateTrainingNextActionHandler()
    {
    }

    static OnMatchManagerCreateTrainingNextActionHandler()
    {
        Instance = new OnMatchManagerCreateTrainingNextActionHandler();
    }

    public static OnMatchManagerCreateTrainingNextActionHandler Instance { get; private set; }
    private List<Action<TeamHeroSelection, TeamHeroSelection, string>> callbacks = new();

    public void AddCallback(Action<TeamHeroSelection, TeamHeroSelection, string> callback)
    {
        Plugin.Log.LogInfo("Adding callback");
        Instance.callbacks.Add(callback);
    }

    public static void Prefix(
        TeamHeroSelection p1,
        TeamHeroSelection p2,
        string arenaIdOverride = null
    )
    {
        Plugin.Log.LogInfo($"Running callbacks: {Instance.callbacks.Count}");
        foreach (var callback in Instance.callbacks)
        {
            callback(p1, p2, arenaIdOverride);
        }

    }
}