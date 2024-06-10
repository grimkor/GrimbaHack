using System;
using System.Collections.Generic;
using HarmonyLib;
using nway.gameplay;

namespace GrimbaHack.Utility;

[HarmonyPatch(typeof(AppStateManager), nameof(AppStateManager.SetAppState))]
public class OnEnterMatchActionHandler
{
    private OnEnterMatchActionHandler()
    {
    }

    static OnEnterMatchActionHandler()
    {
        Instance = new OnEnterMatchActionHandler();
    }

    public static OnEnterMatchActionHandler Instance { get; set; }
    private List<Action<AppState>> callbacks = new();
    private List<Action<AppState>> prefixCallbacks = new();

    public void AddCallback(Action<AppState> callback)
    {
        Instance.callbacks.Add(callback);
    }

    public void AddPrefixCallback(Action<AppState> callback)
    {
        Instance.prefixCallbacks.Add(callback);
    }

    public static void Prefix(AppState state)
    {
        foreach (Action<AppState> callback in Instance.prefixCallbacks)
        {
            callback(state);
        }
    }

    public static void Postfix(AppState state)
    {
        foreach (var callback in Instance.callbacks)
        {
            callback(state);
        }
    }
}