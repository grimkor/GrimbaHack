using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using BepInEx.Logging;
using HarmonyLib;
using nway.gameplay;
using nway.gameplay.ai;
using nway.gameplay.simulation;
using nway.gameplay.ui;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

namespace GrimbaHack;

public class Exploration : MonoBehaviour
{
    // private static ManualLogSource _log;
    // private static readonly Stopwatch Time = new Stopwatch();
    // private static readonly Stopwatch TimeAnimation = new Stopwatch();
    // private static Dictionary<string, FrameData> _frameDataList = new Dictionary<string, FrameData>();
    // private static FrameData _frameDataInstance;
    // private static double _startup;
    // private static int _playerJumpTime;
    // private static int _dummyJumpTime;
    // private static int _playerJumpHeight = 0;
    // private static int _dummyJumpHeight = 0;
    // private static bool _isHitStun = false;
    //
    //
    //
    // private static bool _showWindow = false;
    // private static bool _testStateBool = false;
    // private static string _attackName = "";
    //
    // public void Setup(ManualLogSource logger)
    // {
    //     _log = logger;
    // }
    //
    //
    //
    // private void Update()
    // {
    //     if (Keyboard.current.backquoteKey.isPressed)
    //     {
    //         _log.LogInfo(JsonSerializer.Serialize(_frameDataList));
    //     }
    //
    //     if (Keyboard.current.f1Key.isPressed)
    //     {
    //         _frameDataList = new Dictionary<string, FrameData>();
    //         _playerJumpTime = 0;
    //         _dummyJumpTime = 0;
    //         _frameDataInstance = null;
    //         _playerJumpHeight = 0;
    //         _dummyJumpHeight = 0;
    //         _startup = 0;
    //         Time.Stop();
    //         Time.Reset();
    //     }
    //
    //
    //
    //     if (_dummyCharacter)
    //     {
    //         if (!_testStateBool && _dummyCharacter.InHitStun || _dummyCharacter.InBlockStun)
    //         {
    //             _testStateBool = true;
    //         }
    //         else if (_testStateBool && !_dummyCharacter.InHitStun && !_dummyCharacter.InBlockStun)
    //         {
    //             _testStateBool = false;
    //         }
    //     }
    //
    //     if (_playerCharacter && _dummyCharacter)
    //     {
    //         if (!TimeAnimation.IsRunning)
    //         {
    //             if (_playerCharacter.stateMachine.InAttackAnim)
    //             {
    //                 _playerState = _playerCharacter.StateMachine.currentStateName;
    //                 TimeAnimation.Start();
    //             }
    //         }
    //         else
    //         {
    //             if (_dummyCharacter.stateMachine.InHitReaction && _startupAnimation == 0)
    //             {
    //                 _startupAnimation = TimeAnimation.ElapsedMilliseconds;
    //             }
    //
    //             if (!_dummyCharacter.StateMachine.InHitReaction && _dummyCharacterTime == 0 && _startupAnimation > 0)
    //             {
    //                 _dummyCharacterTime = TimeAnimation.ElapsedMilliseconds;
    //             }
    //
    //             if (_playerCharacter.stateMachine.currentStateName != _playerState && _playerCharacterTime == 0)
    //             {
    //                 _playerCharacterTime = TimeAnimation.ElapsedMilliseconds;
    //             }
    //
    //             if (_playerCharacterTime > 0 && (_dummyCharacterTime > 0 || _startupAnimation == 0))
    //             {
    //                 TimeAnimation.Stop();
    //                 _log.LogInfo($"PlayerTime (ms): {_playerCharacterTime}");
    //                 _log.LogInfo($"DummyTime (ms): {_dummyCharacterTime}");
    //                 _log.LogInfo($"Startup (ms): {_startupAnimation / 16.67}");
    //                 _log.LogInfo($"Startup (frames): {_startupAnimation / 16.67}");
    //                 _log.LogInfo($"Difference (ms): {_dummyCharacterTime - _playerCharacterTime}");
    //                 _log.LogInfo($"Difference (frames): {(_dummyCharacterTime - _playerCharacterTime) / 16.67}");
    //                 _playerCharacterTime = 0;
    //                 _dummyCharacterTime = 0;
    //                 _startupAnimation = 0;
    //                 TimeAnimation.Reset();
    //             }
    //         }
    //     }
    // }
    //
    // [HarmonyPatch(typeof(CommandHistoryItem), nameof(CommandHistoryItem.UpdateActionButton))]
    // public class PatchUpdateActionButton
    // {
    //     public static void Postfix(RemappedButton button,
    //         PlayerButton mask,
    //         PlayerButton input,
    //         IController controller,
    //         ButtonMap buttonMap, ref bool __result)
    //     {
    //         if (!Time.IsRunning && __result)
    //         {
    //             Time.Start();
    //         }
    //     }
    // }
    //
    // [HarmonyPatch(typeof(Character), nameof(Character.GetOffenseInfo))]
    // public class PatchGetOffenseInfo
    // {
    //     public static void Postfix(ref OffenseInfo __result)
    //     {
    //         if (_frameDataInstance?.AttackName != __result.attackName)
    //         {
    //             _frameDataInstance = new FrameData
    //             {
    //                 AttackName = __result.attackName,
    //                 HitstunFrames = __result.hitStunFrames,
    //                 BlockstunFrames = __result.blockStunFrames,
    //                 BaseDamage = __result.baseDamage,
    //                 BlockedDamagePercent = __result.blockedDamagePercent,
    //                 LaunchHeight = __result.launchHeight
    //             };
    //         }
    //     }
    // }
    //
    // [HarmonyPatch(typeof(Character), nameof(Character.ApplyBlockAndHitStun))]
    // public class ApplyBlockAndHitStun
    // {
    //     public static void Prefix(bool isHitStun, int originalFrames)
    //     {
    //         if (!Time.IsRunning) return;
    //
    //         _isHitStun = isHitStun;
    //         if (_frameDataInstance != null)
    //         {
    //             var startup = (int)Math.Round(Time.ElapsedMilliseconds / 16.67);
    //             _frameDataInstance.StartupFrames = startup - _frameDataInstance?.StartupFrames ?? 0;
    //         }
    //     }
    // }
    //
    //
    // [HarmonyPatch(typeof(Character), nameof(Character.ApplyJumping))]
    // public class ApplyJumping
    // {
    //     public static void Prefix(int jumpHeight)
    //     {
    //         var elapsed = (int)Math.Round(Time.ElapsedMilliseconds / 16.67);
    //         if (_playerJumpHeight == 0)
    //         {
    //             _playerJumpHeight = jumpHeight;
    //             _log.LogInfo($"*** Player Jump height recorded at {jumpHeight}, now record the Dummy jumping.");
    //             return;
    //         }
    //
    //         if (_dummyJumpHeight == 0)
    //         {
    //             _dummyJumpHeight = jumpHeight;
    //             _log.LogInfo($"*** Dummy Jump height recorded at {jumpHeight}, time to nerd out!");
    //             return;
    //         }
    //
    //         // if (_frameDataInstance?.LaunchHeight == jumpHeight)
    //         if (jumpHeight != _playerJumpHeight && jumpHeight != _dummyJumpHeight)
    //         {
    //             return;
    //         }
    //
    //         if (!Time.IsRunning || _frameDataInstance == null) return;
    //         if (jumpHeight == _playerJumpHeight & _playerJumpTime == 0)
    //         {
    //             _playerJumpTime = elapsed;
    //         }
    //         else if (_dummyJumpTime == 0)
    //         {
    //             _dummyJumpTime = elapsed;
    //         }
    //
    //         if (_playerJumpTime > 0 && _dummyJumpTime > 0)
    //         {
    //             var recovery = (int)Math.Round(Time.ElapsedMilliseconds / 16.67);
    //             _frameDataInstance.TotalRecovery = recovery;
    //             var frameData = _frameDataList.ContainsKey(_frameDataInstance.AttackName)
    //                 ? _frameDataList[_frameDataInstance.AttackName]
    //                 : _frameDataInstance;
    //             if (_isHitStun)
    //             {
    //                 frameData.AdvantageOnHit = _dummyJumpTime - _playerJumpTime;
    //             }
    //             else
    //             {
    //                 frameData.AdvantageOnBlock = _dummyJumpTime - _playerJumpTime;
    //             }
    //
    //             _frameDataList.Remove(_frameDataInstance.AttackName);
    //             _frameDataList.Add(_frameDataInstance.AttackName, frameData);
    //
    //             var str = _isHitStun ? "Hit" : "Block";
    //             _log.LogInfo(
    //                 $"{frameData.AttackName} is {_dummyJumpTime - _playerJumpTime} on {str}");
    //             _frameDataInstance = null;
    //             Time.Reset();
    //             _playerJumpTime = 0;
    //             _dummyJumpTime = 0;
    //         }
    //     }
    // }
    //
    //
    // [HarmonyPatch(typeof(SimulationManager), nameof(SimulationManager.Initialize))]
    // public class PatchSimulationInitialize
    // {
    //     public static void Prefix()
    //     {
    //         if (Time.IsRunning)
    //         {
    //             Time.Stop();
    //             Time.Reset();
    //         }
    //     }
    // }
}

/* 

See game state:
AppStateManager.state == Combat/Menu

MatchManager.createXMatch to see what match is being made to enable/disable stuff
*/