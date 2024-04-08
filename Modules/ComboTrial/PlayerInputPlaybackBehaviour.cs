using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using nway.gameplay.simulation;

namespace GrimbaHack.Modules.ComboTrial;

[HarmonyPatch(typeof(SimulationManager), nameof(SimulationManager.UpdateInput))]
public class PlayerInputPlaybackBehaviour
{
    public static PlayerInputPlaybackBehaviour Instance = new();
    private List<int> _inputs = new();
    private int _inputFrame;
    private bool _enabled;


    private static void Prefix(int simulationFrameCount, int slot, ref int input)
    {
        if (!Instance._enabled || slot != 0) return;
        if (Instance._inputs.Count < 1 || Instance._inputFrame > Instance._inputs.Count -1)
        {
            Instance.Stop();
            return;
        }
        input = Instance._inputs[Instance._inputFrame++];
    }

    public void Playback(List<int> comboInputs, int delay = 0)
    {
        var inputs = comboInputs.ToList();
        for (int i = 0; i < delay; i++)
        {
            inputs.Insert(0,0);
        }

        Instance._inputs = inputs;
        Instance._inputFrame = 0;
        Instance._enabled = true;
    }

    public void Stop()
    {
        Instance._enabled = false;
        Instance._inputs.Clear();
        Instance._inputFrame = 0;
    }
}
