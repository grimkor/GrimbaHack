using System;
using System.Collections.Generic;
using HarmonyLib;
using nway.gameplay;
using Action = System.Action;

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
    private List<Action> callbacks = new();

    public void AddCallback(Action callback)
    {
        Instance.callbacks.Add(callback);
    }
    
    public static void Postfix(AppState state)
    {
        foreach (var callback in Instance.callbacks)
        {
            callback();
        }
    }
}