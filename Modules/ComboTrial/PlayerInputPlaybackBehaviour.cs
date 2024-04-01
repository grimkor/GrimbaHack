using System.Collections.Generic;
using System.Linq;
using nway.gameplay;
using UnityEngine;

namespace GrimbaHack.Modules.ComboTrial;

public class PlayerInputPlaybackBehaviour : MonoBehaviour
{
    private List<uint> _inputs;
    private int _inputFrame;
    private InputSystem _inputSystem;
    private IController _controller;

    public void Playback(InputSystem inputSystem, List<uint> comboInputs, int delay = 0)
    {
        var inputs = comboInputs.ToList();
        for (int i = 0; i < delay; i++)
        {
            inputs.Insert(0,0);
        }

        _controller = ControllerManager.GetController(0);
        _inputSystem = inputSystem;
        _inputs = inputs;
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

        if (_controller.GetPlayerButtonInput().Buttons != 0)
        {
            Stop();
            return;
        }

        _inputSystem.SetInput(_inputs[_inputFrame]);
        _inputFrame++;
    }
}
