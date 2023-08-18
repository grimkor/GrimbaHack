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
    private List<Action> callbacks = new();

    public void AddCallback(Action callback)
    {
        Instance.callbacks.Add(callback);
    }

    public static void Postfix()
    {
        foreach (var callback in Instance.callbacks)
        {
            callback();
        }
    }
}