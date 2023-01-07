using System;
using System.Diagnostics;
using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GrimbaHack;

public class FrameData
{
    public string AttackName { get; set; }
    public int StartupFrames { get; set; }
    public int HitstunFrames { get; set; }
    public int BlockstunFrames { get; set; }
    public int BaseDamage { get; set; }
    public int BlockedDamagePercent { get; set; }
    public int TotalRecovery { get; set; }
    public int LaunchHeight { get; set; }
    public int Advantage { get; set; }
    // public int AdvantageOnHit { get; set; }
    // public int AdvantageOnBlock { get; set; }
}

public class FrameDataModal : MonoBehaviour
{
    // Character refs
    private static Character _playerCharacter;
    private static Character _dummyCharacter;

    // Frame tracker
    private static readonly Stopwatch TimeAnimation = new Stopwatch();
    private static double _playerCharacterTime;
    private static double _dummyCharacterTime;
    private static string _playerState;
    private static double _startupAnimation;
    private static FrameData _currentFrameData = new FrameData
    {
        AttackName = "combat_unknown",
        StartupFrames = 0,
        HitstunFrames = 0,
        BlockstunFrames = 0,
        BaseDamage = 0,
        BlockedDamagePercent = 0,
        TotalRecovery = 0,
        LaunchHeight = 0,
        Advantage = 0
    };

    // Window Properties
    private static bool _showWindow = false;
    private Rect _windowRect = new Rect(20, 20, 350, 150);

    private static bool _testStateBool = false;

    private void windowRenderer(int windowID)
    {
        var splitString = _currentFrameData.AttackName.ToLower().Split("combat_")[1];
        GUI.Label(new Rect(25, 20, 100, 30), "Attack:");
        GUI.Label(new Rect(135, 20, 350 - 135, 30), splitString);
        GUI.Label(new Rect(25, 40, 100, 30), "Base Damage:");
        GUI.Label(new Rect(135, 40, 100, 30), $"{_currentFrameData.BaseDamage}");
        GUI.Label(new Rect(25, 60, 100, 30), "Startup:");
        GUI.Label(new Rect(135, 60, 100, 30), $"{_currentFrameData.StartupFrames}f");
        GUI.Label(new Rect(25, 80, 100, 30), "Blockstun:");
        GUI.Label(new Rect(135, 80, 100, 30), $"{_currentFrameData.BlockstunFrames}f");
        GUI.Label(new Rect(25, 100, 100, 30), "Hitstun:");
        GUI.Label(new Rect(135, 100, 100, 30), $"{_currentFrameData.HitstunFrames}f");
        GUI.Label(new Rect(25, 120, 100, 30), "Advantage:");
        GUI.Label(new Rect(135, 120, 100, 30), $"{_currentFrameData.Advantage}f");
        GUI.DragWindow(new Rect(0, 0, 10000, 20));
    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }

    private void OnGUI()
    {
        if (!_showWindow) return;
        GUI.backgroundColor = Color.black;

        var currentStyle = new GUIStyle(GUI.skin.box)
        {
            normal =
            {
                background = MakeTex(2, 2, new Color(0f, 1f, 0f, .8f))
            }
        };
        _windowRect = GUI.Window(0, _windowRect, (GUI.WindowFunction)windowRenderer, _playerCharacter.name.Split("(")[0], currentStyle);
    }


    private void Update()
    {
        if (Keyboard.current.f2Key.isPressed)
        {
            _playerCharacterTime = 0;
            _dummyCharacterTime = 0;
            _startupAnimation = 0;
            TimeAnimation.Reset();
            var characters = FindObjectsOfType<Character>();

            foreach (var character in characters)
            {
                if (character.IsActiveCharacter)
                {
                    if (character.team == 0)
                    {
                        _playerCharacter = character;
                    }
                    else
                    {
                        _dummyCharacter = character;
                    }
                }

                _showWindow = true;
            }
        }

        if (Keyboard.current.f3Key.isPressed)
        {
            _showWindow = false;
        }


        if (_dummyCharacter)
        {
            if (!_testStateBool && _dummyCharacter.InHitStun || _dummyCharacter.InBlockStun)
            {
                _testStateBool = true;
            }
            else if (_testStateBool && !_dummyCharacter.InHitStun && !_dummyCharacter.InBlockStun)
            {
                _testStateBool = false;
            }
        }

        if (_playerCharacter && _dummyCharacter)
        {
            if (!TimeAnimation.IsRunning)
            {
                if (_playerCharacter.stateMachine.InAttackAnim)
                {
                    _playerState = _playerCharacter.StateMachine.currentStateName;
                    TimeAnimation.Start();
                }
            }
            else
            {
                if (_dummyCharacter.stateMachine.InHitReaction && _startupAnimation == 0)
                {
                    _startupAnimation = TimeAnimation.ElapsedMilliseconds;
                }

                if (!_dummyCharacter.StateMachine.InHitReaction && _dummyCharacterTime == 0 && _startupAnimation > 0)
                {
                    _dummyCharacterTime = TimeAnimation.ElapsedMilliseconds;
                }

                if (_playerCharacter.stateMachine.currentStateName != _playerState && _playerCharacterTime == 0)
                {
                    _playerCharacterTime = TimeAnimation.ElapsedMilliseconds;
                }

                if (_playerCharacterTime > 0 && (_dummyCharacterTime > 0 || _startupAnimation == 0))
                {
                    TimeAnimation.Stop();
                    _currentFrameData.Advantage = (int)((_dummyCharacterTime - _playerCharacterTime) / 16.67);
                    _playerCharacterTime = 0;
                    _dummyCharacterTime = 0;
                    _startupAnimation = 0;
                    TimeAnimation.Reset();
                }
            }
        }
    }

    [HarmonyPatch(typeof(Character), nameof(Character.GetOffenseInfo))]
    public class PatchGetOffenseInfo
    {
        public static void Postfix(ref OffenseInfo __result)
        {
            if (_currentFrameData?.AttackName != __result.attackName)
            {
                _currentFrameData = new FrameData
                {
                    AttackName = __result.attackName,
                    HitstunFrames = __result.hitStunFrames,
                    BlockstunFrames = __result.blockStunFrames,
                    BaseDamage = __result.baseDamage,
                    BlockedDamagePercent = __result.blockedDamagePercent,
                    LaunchHeight = __result.launchHeight
                };
            }
        }
    }
    [HarmonyPatch(typeof(Character), nameof(Character.ApplyBlockAndHitStun))]
    public class ApplyBlockAndHitStun
    {
        public static void Prefix(bool isHitStun, int originalFrames)
        {
            if (!TimeAnimation.IsRunning) return;
    
            if (_currentFrameData != null)
            {
                var startup = (int)Math.Round(TimeAnimation.ElapsedMilliseconds / 16.67);
                _currentFrameData.StartupFrames = startup;
            }
        }
    }
}