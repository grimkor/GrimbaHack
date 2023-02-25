using System.Collections.Generic;
using HarmonyLib;
using nway.gameplay;
using Action = System.Action;

namespace GrimbaHack.Utility;

[HarmonyPatch(typeof(AppStateManager), nameof(AppStateManager.SetAppState))]
public class OnEnterPremadeMatchActionHandler
{
    private OnEnterPremadeMatchActionHandler()
    {
    }

    static OnEnterPremadeMatchActionHandler()
    {
        Instance = new OnEnterPremadeMatchActionHandler();
    }
    public static OnEnterPremadeMatchActionHandler Instance { get; set; }
    private List<Action> callbacks = new();

    public void AddCallback(Action callback)
    {
        Instance.callbacks.Add(callback);
    }
    
    public static void Postfix(AppState state)
    {
        if (!Data.Global.IsPremadeMatch()) return;
        foreach (var callback in Instance.callbacks)
        {
            callback();
        }
    }
}