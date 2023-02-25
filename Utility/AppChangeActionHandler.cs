using System;
using System.Collections.Generic;
using HarmonyLib;
using nway.gameplay;

namespace GrimbaHack.Utility;

[HarmonyPatch(typeof(AppStateManager), nameof(AppStateManager.SetAppState))]
public class AppChangeCallbackHandler
{
    private AppChangeCallbackHandler()
    {
    }

    static AppChangeCallbackHandler()
    {
        Instance = new AppChangeCallbackHandler();
    }
    public static AppChangeCallbackHandler Instance { get; set; }
    private List<Action<AppState>> callbacks = new();

    public void AddCallback(Action<AppState> callback)
    {
        Instance.callbacks.Add(callback);
    }
    
    public static void Prefix(AppState state)
    {
        foreach (var callback in Instance.callbacks)
        {
            callback(state);
        }
    }
}