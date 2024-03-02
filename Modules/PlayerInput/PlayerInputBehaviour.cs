using System;
using System.Collections.Generic;
using nway.gameplay;
using UnityEngine;

namespace GrimbaHack.Modules.PlayerInput;

public class PlayerInputBehaviour : MonoBehaviour
{
    private static InputSystem _inputSystem;
    public List<uint> inputs = new();
    private int _playbackCount;
    private static Character _playerCharacter;
    private static Character _dummyCharacter;
    private PlayerInputBehaviourState _state;

    public void SetEnable(bool value)
    {
        enabled = value;
        if (enabled)
        {
            Setup();
        }
    }

    public void SetState(PlayerInputBehaviourState state)
    {
        switch (state)
        {
            case PlayerInputBehaviourState.Idle:
                Reset();
                break;
            case PlayerInputBehaviourState.PreRecord:
                Record();
                break;
            case PlayerInputBehaviourState.Recording:
                _state = PlayerInputBehaviourState.Recording;
                break;
            case PlayerInputBehaviourState.Playback:
                Playback();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    public void Reset()
    {
        _playbackCount = 0;
        _state = PlayerInputBehaviourState.Idle;
    }

    public void Setup()
    {
        Reset();
        var characters = FindObjectsOfType<Character>();

        foreach (var character in characters)
        {
            if (character.IsActiveCharacter)
            {
                if (character.team == 0)
                {
                    _playerCharacter = character;
                    _inputSystem = character.GetCharacterTeam().GetInputSystem();
                    _playbackCount = 0;
                }
                else
                {
                    _dummyCharacter = character;
                }
            }
        }
    }

    private void Update()
    {
        if (_playerCharacter && _dummyCharacter)
        {
            if (_state == PlayerInputBehaviourState.Playback)
            {
                if (inputs.Count > 0 && _playbackCount <= inputs.Count - 1)
                {
                    _inputSystem.SetInput(inputs[_playbackCount++]);
                }
                else
                {
                    SetState(PlayerInputBehaviourState.Idle);
                }
            }

            else if (_state == PlayerInputBehaviourState.Recording)
            {
                inputs.Add(_inputSystem.GetCharacterInput());
            }

            if (_state == PlayerInputBehaviourState.PreRecord && _inputSystem.GetInput() != 0)
            {
                inputs.Add(_inputSystem.GetInput());
                SetState(PlayerInputBehaviourState.Recording);
            }
        }
    }

    public void Record()
    {
        inputs = new();
        _state = PlayerInputBehaviourState.PreRecord;
    }

    public void Playback()
    {
        _playbackCount = 0;
        _state = PlayerInputBehaviourState.Playback;
    }
}