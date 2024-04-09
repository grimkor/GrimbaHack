using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using nway.gameplay.simulation;

namespace GrimbaHack.Modules.ComboTrial;

[HarmonyPatch(typeof(SimulationManager), nameof(SimulationManager.UpdateInput))]
public class PlayerInputPlaybackController
{
    public static PlayerInputPlaybackController Instance = new();
    private List<int> _inputs = new();
    private int _inputFrame;
    private bool _enabled;
    private int? _startingFrameCount;


    private static void Prefix(int simulationFrameCount, int slot, ref int input)
    {
        if (!Instance._enabled || slot != 0) return;
        if (Instance._startingFrameCount == null)
        {
            Instance._startingFrameCount = simulationFrameCount;
        }

        var frame = simulationFrameCount - (int)Instance._startingFrameCount;
        if (Instance._inputs.Count < 1 || frame > Instance._inputs.Count -1)
        {
            Instance.Stop();
            return;
        }

        if (Instance._startingFrameCount == null)
        {
            Instance._startingFrameCount = simulationFrameCount;
        }
        input = Instance._inputs[simulationFrameCount - (int)Instance._startingFrameCount];
    }

    public void Playback(List<int> comboInputs)
    {

        Instance._inputs = comboInputs.ToList();
        Instance._inputFrame = 0;
        Instance._enabled = true;
    }

    public void Stop()
    {
        Instance._enabled = false;
        Instance._inputs.Clear();
        Instance._inputFrame = 0;
        Instance._startingFrameCount = null;
    }
}
