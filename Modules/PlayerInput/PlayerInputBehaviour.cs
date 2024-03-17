using System;
using System.Collections.Generic;
using GrimbaHack.Modules.Combo;
using nway.gameplay;
using UnityEngine;

namespace GrimbaHack.Modules.PlayerInput;

public class PlayerInputBehaviour : MonoBehaviour
{
    private static InputSystem _inputSystem;
    public List<uint> Inputs = new();
    public int PlaybackCount;

    public void SetEnable(bool value)
    {
        if (enabled == value)
        {
            return;
        }

        enabled = value;
        if (enabled)
        {
            Setup();
        }
    }


    public void Setup()
    {
        Character player = null, dummy = null;
        var characters = FindObjectsOfType<Character>();
        foreach (var character in characters)
        {
            if (character.IsActiveCharacter)
            {
                if (character.team == 0)
                {
                    player = character;
                    _inputSystem = character.GetCharacterTeam().GetInputSystem();
                    PlaybackCount = 0;
                }
                else
                {
                    dummy = character;
                }
            }
        }

        if (player != null && dummy != null)
        {
            PlayerInputController.SetCharacters(player, dummy);
        }

        if (ComboTrackerController.GetState() == ComboTrackerState.Comparing)
        {
            PlayerInputController.LoadSavedCharacterPositions();
        }
    }

    private void Update()
    {
        if (PlayerInputController.GetPlayerCharacter() && PlayerInputController.GetDummyCharacter())
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
                    PlayerInputController.RecordCharacterPositions();
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