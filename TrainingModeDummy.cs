using BepInEx.Logging;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using nway.gameplay.ai;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

namespace GrimbaHack;

public class TrainingModeDummy : MonoBehaviour
{
    private static ManualLogSource _log = Plugin.Log;
    private static CommandRecordingDriver RecordController;
    private static CommandRecordingDriver.RecordingState DummyRecorder;
    private static Character DummyCharacter;
    private static bool DummyIsStunned = false;
    private static bool Ready = false;
    private static bool EnableVariablePushblock = false;
    private static SceneStartup sceneStartup;
    private static int _startingNumber = 0;
    private static int _exDelay = 0;

    private static List<CommandRecordingDriver.InputChange>
        _nextInputs = new List<CommandRecordingDriver.InputChange>();

    private static List<CommandRecordingDriver.InputChange> _inputs =
        new List<CommandRecordingDriver.InputChange>();

    private static bool _ExTriggered = false;

    public void Setup(ManualLogSource logger)
    {
        _log = Plugin.Log;
    }

    private void Update()
    {
        if (Keyboard.current.f4Key.isPressed)
        {
            sceneStartup = FindObjectOfType<SceneStartup>();

            var characters = FindObjectsOfType<Character>();

            foreach (var character in characters)
            {
                if (character.IsActiveCharacter)
                {
                    if (character.team != 0)
                    {
                        DummyCharacter = character;
                    }
                }
            }

            if (sceneStartup && DummyCharacter)
            {
                RecordController = sceneStartup.GamePlay?.recorder;
                DummyRecorder = RecordController?.dummyRecorder;
                if (DummyRecorder != null && RecordController != null)
                {
                    // DummyRecorder.PrepareRecording();
                    // DummyRecorder.RecordInput(5243152); //EX
                    // DummyRecorder.FinishRecording();
                    // _inputs = DummyRecorder.inputs;
                    Ready = true;
                    _log.LogInfo("Dummy is prepared!");
                }
            }
        }

        if (Ready)
        {
            if (_ExTriggered && DummyRecorder is { isPlaying: true } &&
                RecordController.GetCurrentFrame() == DummyRecorder.LatestFrame())
            {
                RecordController.StopPlayback();
                RecordController.Reset();
                _ExTriggered = false;
            }

            if (DummyIsStunned && !DummyCharacter.InBlockStun && !DummyCharacter.InHitStun)
            {
                RecordController.StopPlayback();
                DummyRecorder.Rewind();
                DummyRecorder.PrepareRecording();
                DummyRecorder.RecordInput(5243152); //EX
                DummyRecorder.FinishRecording();
                RecordController.StartPlayback();

                DummyIsStunned = false;
                _ExTriggered = true;
            }

            if (!DummyIsStunned && (DummyCharacter.InBlockStun || DummyCharacter.InHitStun))
            {
                DummyIsStunned = true;
            }
        }

        // if (EnableVariablePushblock)
        // {
        //     if (DummyIsStunned && !DummyCharacter.InBlockStun && !DummyCharacter.InHitStun)
        //     {
        //         // RecordController.StopPlayback();
        //     }
        // }
        //
        // if (Keyboard.current.f5Key.isPressed)
        // {
        //     EnableVariablePushblock = true;
        //     // Ready = false;
        // }
        //
        // if (Keyboard.current.f6Key.isPressed)
        // {
        //     EnableVariablePushblock = false;
        //     // Ready = true;
        // }

        if (Keyboard.current.f7Key.isPressed)
        {
            // RecordController.StopPlayback();
            // RecordController.Reset();
            Ready = false;
        }

        // if (Keyboard.current.f8Key.isPressed)
        // {
        //     _log.LogInfo(DummyRecorder.inputs.Count);
        //     _log.LogInfo(DummyRecorder.inputs[^1]);
        //     _startingNumber = 0;
        //     DummyRecorder.inputs =
        //         new List<CommandRecordingDriver.InputChange>();
        //     // DummyRecorder.inputs.Add(new CommandRecordingDriver.InputChange(_startingNumber, 0));
        //     DummyRecorder.inputs.Add(new CommandRecordingDriver.InputChange(_startingNumber + 1, 1048832));
        //     DummyRecorder.inputs.Add(new CommandRecordingDriver.InputChange(_startingNumber + 2, 0));
        //     DummyRecorder.inputs.Add(new CommandRecordingDriver.InputChange(_startingNumber + 12, 272)); // 5S
        //     DummyRecorder.inputs.Add(new CommandRecordingDriver.InputChange(_startingNumber + 500, 0));
        // }
        //
        // if (Keyboard.current.homeKey.isPressed)
        // {
        //     _startingNumber += 1;
        //     _log.LogInfo($"_startingNumber: {_startingNumber}");
        // }
        //
        // if (Keyboard.current.endKey.isPressed)
        // {
        //     _startingNumber -= 1;
        //     _log.LogInfo($"_startingNumber: {_startingNumber}");
        // }
        //
        // if (Keyboard.current.pageUpKey.isPressed)
        // {
        //     _exDelay += 1;
        //     _log.LogInfo($"_exDelay: {_exDelay}");
        // }
        //
        // if (Keyboard.current.pageDownKey.isPressed)
        // {
        //     _exDelay -= 1;
        //     _log.LogInfo($"_exDelay: {_exDelay}");
        // }
    }

    // [HarmonyPatch(typeof(Character), nameof(Character.ApplyBlockAndHitStun))]
    // public class PatchApplyBlockAndHitStun
    // {
    //     public static void Prefix(bool isHitStun, int originalFrames)
    //     {
    //         if (!isHitStun && EnableVariablePushblock && originalFrames > 0)
    //             // {
    //             //     DummyIsStunned = true;
    //             // var random = new Random();
    //             //     
    //             //     DummyRecorder.PrepareRecording();
    //             // var pushblockFrame = random.Next(0, originalFrames -2); // Compensate for delay
    //             //     while (DummyRecorder.IncrementFrame(pushblockFrame, false))
    //             //     {
    //             //     }
    //             //
    //             //     DummyRecorder.RecordInput(1048832); // 5S
    //             //     DummyRecorder.IncrementFrame(10000, false);
    //             //     DummyRecorder.RecordInput(4194304); // Let go of 5S
    //             //     DummyRecorder.FinishRecording();
    //             //     RecordController.Reset();
    //             //     RecordController.StartPlayback();
    //             // }
    //         {
    //             DummyIsStunned = true;
    //             // var random = new Random();
    //             // RecordController.StopPlayback();
    //             DummyRecorder.inputs = _nextInputs;
    //             RecordController.StartPlayback();
    //             // _startingNumber += 1;
    //             _nextInputs =
    //                 new List<CommandRecordingDriver.InputChange>();
    //             DummyRecorder.inputs.Add(new CommandRecordingDriver.InputChange(_startingNumber, 1048832));
    //             DummyRecorder.inputs.Add(new CommandRecordingDriver.InputChange(_startingNumber + 1, 0));
    //             DummyRecorder.inputs.Add(new CommandRecordingDriver.InputChange(_startingNumber + _exDelay, 272));
    //             DummyRecorder.inputs.Add(new CommandRecordingDriver.InputChange(_startingNumber + 500, 0));
    //             _log.LogInfo($"_startingNumber: {_startingNumber}");
    //             _log.LogInfo($"_exDelay: {_exDelay}");
    //         }
    //         // DummyRecorder.Rewind();
    //         // DummyRecorder.PrepareRecording();
    //         // var pushblockFrame = random.Next(1, originalFrames -5); // Compensate for delay
    //         // while (DummyRecorder.IncrementFrame(_startingNumber, false))
    //         // {
    //         // }
    //         // DummyRecorder.RecordInput(1048836); // 5S
    //         // DummyRecorder.IncrementFrame(_startingNumber +2, false);
    //         // DummyRecorder.RecordInput(4);
    //         // while (DummyRecorder.IncrementFrame(_startingNumber + 6, false))
    //         // {
    //         // }
    //         // DummyRecorder.RecordInput(4194304); // Let go of 5S
    //         // DummyRecorder.IncrementFrame(_startingNumber +8, false);
    //         // DummyRecorder.RecordInput(4194304); // Let go of 5S
    //         // DummyRecorder.IncrementFrame(_startingNumber +8, false);
    //         // DummyRecorder.RecordInput(4);
    //         // while (DummyRecorder.IncrementFrame(_startingNumber +11, false))
    //         // {
    //         // }
    //         // DummyRecorder.RecordInput(276); //EX
    //         // DummyRecorder.IncrementFrame(_startingNumber +100, false);
    //         // DummyRecorder.RecordInput(4);
    //         // DummyRecorder.FinishRecording();
    //         // RecordController.StartPlayback();
    //         // foreach (var input in DummyRecorder.inputs)
    //         // {
    //         // _log.LogInfo($"frame: {input.frame} input: {input.input}");
    //         // }
    //         // _log.LogInfo($"original: {originalFrames}, pushblock: {_startingNumber}");
    //     }
    // }
}