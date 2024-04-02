using System.Collections.Generic;
using HarmonyLib;
using nway.gameplay;
using nway.gameplay.match;
using nway.gameplay.simulation;
using Action = System.Action;

namespace GrimbaHack.Utility;

[HarmonyPatch(typeof(SimulationManager), nameof(SimulationManager.Initialize))]
public class OnSimulationInitializeActionHandler
{
    private OnSimulationInitializeActionHandler()
    {
    }

    static OnSimulationInitializeActionHandler()
    {
        Instance = new OnSimulationInitializeActionHandler();
    }

    public static OnSimulationInitializeActionHandler Instance { get; private set; }
    private List<Action> postfixCallbacks = new();
    private List<Action> prefixCallbacks = new();

    public void AddPrefix(Action callback)
    {
        Instance.postfixCallbacks.Add(callback);
    }

    public static void Prefix()
    {
        Plugin.Log.LogInfo($"Simulation prefix count: {Instance.prefixCallbacks.Count}");
        foreach (var callback in Instance.prefixCallbacks)
        {
            callback();
        }
    }
    public void AddPostfix(Action callback)
    {
        Instance.postfixCallbacks.Add(callback);
    }

    public static void Postfix()
    {
        Plugin.Log.LogInfo($"Simulation postfix count: {Instance.postfixCallbacks.Count}");
        foreach (var callback in Instance.postfixCallbacks)
        {
            callback();
        }
    }
}
