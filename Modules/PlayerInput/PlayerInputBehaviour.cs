using System.Collections.Generic;
using nway.gameplay;
using UnityEngine;

namespace GrimbaHack.Modules.PlayerInput;

public class PlayerInputBehaviour : MonoBehaviour
{
    public PlayerInputBehaviour()
    {
    }

    private static InputSystem _inputSystem;
    public List<uint> _inputs = new();
    private int _playbackCount;
    private static Character _playerCharacter;
    private static Character _dummyCharacter;
    private bool recording;
    private bool playback;
    private bool prepareRecording;

    public void SetEnable(bool value)
    {
        enabled = value;
        if (enabled)
        {
            Setup();
        }
    }


    public void Reset()
    {
        _playbackCount = 0;
        prepareRecording = false;
        recording = false;
        playback = false;
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
            if (playback)
            {
                //playback
                if (_inputs.Count > 0 && _playbackCount <= _inputs.Count - 1)
                {
                    _inputSystem.SetInput(_inputs[_playbackCount++]);
                }
                else
                {
                    playback = false;
                }
            }

            else if (recording)
            {
                _inputs.Add(_inputSystem.GetCharacterInput());
            }

            if (prepareRecording && _inputSystem.GetInput() != 0 && _inputs.Count == 0)
            {
                prepareRecording = false;
                recording = true;
                _inputs.Add(_inputSystem.GetInput());
            }
        }
    }

    public void Record()
    {
        _inputs = new();
        prepareRecording = true;
        recording = false;
        playback = false;
    }

    public void Playback()
    {
        _playbackCount = 0;
        prepareRecording = false;
        recording = false;
        playback = true;
    }
}