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
    }

    public static OnEnterTrainingMatchActionHandler Instance = new();
    private readonly List<Action> _prefixCallbacks = new();
    private readonly List<Action> _postfixCallbacks = new();
    private readonly List<Action> _pendingPrefixCallbacks = new();
    private readonly List<Action> _pendingPostfixCallbacks = new();
    private bool _isRunningPrefixes;
    private bool _isRunningPostfixes;

    public void AddPrefix(Action callback)
    {
        if (Instance._isRunningPrefixes)
        {
            Instance._pendingPrefixCallbacks.Add(callback);
            return;
        }
        Instance._prefixCallbacks.Add(callback);
    }

    public void AddPostfix(Action callback)
    {
        if (Instance._isRunningPostfixes)
        {
            Instance._pendingPostfixCallbacks.Add(callback);
            return;
        }
        Instance._postfixCallbacks.Add(callback);
    }

    public static void Prefix(AppState state)
    {
        if (!Data.Global.IsTrainingMatch()) return;
        Instance._isRunningPrefixes = true;
        foreach (var callback in Instance._prefixCallbacks)
        {
            callback();
        }
        foreach (var callback in Instance._pendingPrefixCallbacks)
        {
            callback();
            Instance._prefixCallbacks.Add(callback);
        }

        Instance._pendingPrefixCallbacks.Clear();
        Instance._isRunningPrefixes = false;
    }

    public static void Postfix(AppState state)
    {
        if (!Data.Global.IsTrainingMatch()) return;
        Instance._isRunningPostfixes = true;
        foreach (var callback in Instance._postfixCallbacks)
        {
            callback();
        }
        foreach (var callback in Instance._pendingPostfixCallbacks)
        {
            callback();
            Instance._postfixCallbacks.Add(callback);
        }

        Instance._pendingPostfixCallbacks.Clear();
        Instance._isRunningPostfixes = false;
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
