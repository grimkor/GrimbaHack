using System;
using System.Collections.Generic;
using HarmonyLib;
using nway.gameplay;
using nway.gameplay.match;
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
    private List<Action> postfixCallbacks = new();
    private List<Action> prefixCallbacks = new();

    public void AddPostfix(Action callback)
    {
        Instance.postfixCallbacks.Add(callback);
    }

    public void AddPrefix(Action callback)
    {
        Instance.prefixCallbacks.Add(callback);
    }

    public static void Postfix(AppState state)
    {
        if (!Data.Global.IsTrainingMatch()) return;
        foreach (var callback in Instance.postfixCallbacks)
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
        Instance.callbacks.Add(callback);
    }

    public static void Prefix(
        TeamHeroSelection p1,
        TeamHeroSelection p2,
        string arenaIdOverride = null
    )
    {
        foreach (var callback in Instance.callbacks)
        {
            callback(p1, p2, arenaIdOverride);
        }

    }
}
