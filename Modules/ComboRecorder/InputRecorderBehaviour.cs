using System.Collections.Generic;
using HarmonyLib;
using nway.gameplay.simulation;

namespace GrimbaHack.Modules.ComboRecorder;

[HarmonyPatch(typeof(SimulationManager), nameof(SimulationManager.UpdateInput))]
public class InputRecorderBehaviour
{
    public List<int> Inputs = new();
    private bool _enabled;
    public static InputRecorderBehaviour Instance = new();

    private static void Postfix(int simulationFrameCount, int slot, int input)
    {
        if (!Instance._enabled || slot != 0) return;
        Instance.Inputs.Add(input);
    }

    public void SetEnabled(bool enabled)
    {
        _enabled = enabled;
    }

    public void Clean()
    {
        Inputs.Clear();
    }

}
