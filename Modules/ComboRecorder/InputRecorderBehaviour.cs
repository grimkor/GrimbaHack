using System.Collections.Generic;
using nway.gameplay;
using UnityEngine;

namespace GrimbaHack.Modules.ComboRecorder;

public class InputRecorderBehaviour : MonoBehaviour
{
    public InputSystem InputSystem;
    public List<uint> Inputs = new();

    public void Clean()
    {
        if (!ComboRecorderManager.Instance.Enabled) return;
        Inputs.Clear();
        var characters = FindObjectsOfType<Character>();
        foreach (var character in characters)
        {
            if (character.IsActiveCharacter)
            {
                if (character.team == 0)
                {
                    InputSystem = character.GetCharacterTeam().GetInputSystem();
                    break;
                }
            }
        }
    }


    private void OnEnable()
    {
        if (!ComboRecorderManager.Instance.Enabled) return;
        Clean();
    }

    private void Update()
    {
        Inputs.Add(InputSystem.GetCharacterInput());
    }
}
