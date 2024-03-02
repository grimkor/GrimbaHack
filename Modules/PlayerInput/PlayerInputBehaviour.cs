using System;
using System.Collections.Generic;
using nway.gameplay;
using UnityEngine;
using UnityEngine.Serialization;

namespace GrimbaHack.Modules.PlayerInput;

public class PlayerInputBehaviour : MonoBehaviour
{
    private static InputSystem _inputSystem;
    public List<uint> Inputs = new();
    public int PlaybackCount;
    private static Character _playerCharacter;
    private static Character _dummyCharacter;

    public void SetEnable(bool value)
    {
        if (enabled == value)
        {
            return;
        }
        Plugin.Log.LogInfo($"PlayerInputBehaviour.SetEnable({value})");
        enabled = value;
        if (enabled)
        {
            Setup();
        }
    }


    public void Setup()
    {
        Plugin.Log.LogInfo($"PlayerInputBehaviour.Setup()");
        var characters = FindObjectsOfType<Character>();

        foreach (var character in characters)
        {
            if (character.IsActiveCharacter)
            {
                if (character.team == 0)
                {
                    _playerCharacter = character;
                    _inputSystem = character.GetCharacterTeam().GetInputSystem();
                    PlaybackCount = 0;
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
            switch (PlayerInputController.Instance.GetState())
            {
                case PlayerInputBehaviourState.Idle:
                    break;
                case PlayerInputBehaviourState.PreRecord:
                    if (_inputSystem.GetInput() == 0)
                    {
                        return;
                    }

                    Inputs.Add(_inputSystem.GetInput());
                    PlayerInputController.Instance.Record();
                    break;
                case PlayerInputBehaviourState.Recording:
                    Inputs.Add(_inputSystem.GetCharacterInput());
                    break;
                case PlayerInputBehaviourState.Playback:
                    if (Inputs.Count > 0 && PlaybackCount <= Inputs.Count - 1)
                    {
                        _inputSystem.SetInput(Inputs[PlaybackCount++]);
                    }
                    else
                    {
                        PlayerInputController.Instance.Idle();
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}