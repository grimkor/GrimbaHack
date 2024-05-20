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
    private int PreviousFrame;

    private static void Postfix(int simulationFrameCount, int slot, int input)
    {
        if (!Instance._enabled || slot != 0) return;
        if (Instance.PreviousFrame == -1)
        {
            Instance.PreviousFrame = simulationFrameCount;
        }

        // add buffer to compensate for skipped frames
        for (int i = 1; i < simulationFrameCount - Instance.PreviousFrame; i++)
        {
            Instance.Inputs.Add(Instance.Inputs[^1]);
        }
        Instance.Inputs.Add(input);
        Instance.PreviousFrame = simulationFrameCount;
    }

    public void SetEnabled(bool enabled)
    {
        Instance._enabled = enabled;
    }

    public void Clean()
    {
        Instance.PreviousFrame = -1;
        Instance.Inputs.Clear();
    }

}
