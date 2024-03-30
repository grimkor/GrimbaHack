using System.Collections.Generic;
using nway.gameplay;
using UnityEngine;

namespace GrimbaHack.Modules.ComboTrial;

public class PlayerInputPlaybackBehaviour : MonoBehaviour
{
    private List<uint> _inputs;
    private int _inputFrame;
    private InputSystem _inputSystem;

    private PlayerInputPlaybackBehaviour()
    {
    }

    public void Playback(InputSystem inputSystem, List<uint> comboInputs)
    {
        _inputSystem = inputSystem;
        _inputs = comboInputs;
        _inputFrame = 0;
        enabled = true;
    }

    public void Stop()
    {
        enabled = false;
        _inputs = null;
        _inputFrame = 0;
    }

    private void Update()
    {
        if (_inputs == null) return;
        if (_inputs.Count < 1 || _inputSystem == null)
        {
            return;
        }

        if (_inputFrame > _inputs.Count -1)
        {
            Stop();
            return;
        }

        _inputSystem.SetInput(_inputs[_inputFrame]);
        _inputFrame++;
    }
}
