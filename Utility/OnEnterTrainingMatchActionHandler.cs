using System;
using System.Collections.Generic;
using HarmonyLib;
using nway.gameplay;
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

    public void AddCallback(Action callback)
    {
        Instance.callbacks.Add(callback);
    }
    
    public static void Postfix(AppState state)
    {
        if (!Data.Global.IsTrainingMatch()) return;
        foreach (var callback in Instance.callbacks)
        {
            callback();
        }
    }
}