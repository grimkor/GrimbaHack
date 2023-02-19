using System;
using System.Collections.Generic;
using HarmonyLib;
using nway.gameplay;
using Action = System.Action;

namespace GrimbaHack.Utility;

[HarmonyPatch(typeof(AppStateManager), nameof(AppStateManager.SetAppState))]
public class StartMainMenuActionHandler
{
    private StartMainMenuActionHandler()
    {
    }

    static StartMainMenuActionHandler()
    {
        Instance = new StartMainMenuActionHandler();
    }
    public static StartMainMenuActionHandler Instance { get; set; }
    private List<Action> callbacks = new();

    public void AddCallback(Action callback)
    {
        Instance.callbacks.Add(callback);
    }
    
    public static void Postfix(AppState state)
    {
        if (state == AppState.Menu)
        foreach (var callback in Instance.callbacks)
        {
            callback();
        }
    }
}