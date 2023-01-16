using System;
using System.Diagnostics;
using epoch.db;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using nway.gameplay.match;
using nway.gameplay.simulation;
using UnityEngine;
using UniverseLib.UI;

namespace GrimbaHack.Modules;

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

public class FrameDataManager : ModuleBase
{
    private FrameDataManager()
    {
    }

    public static FrameDataManager Instance { get; private set; }

    static FrameDataManager()
    {
        Instance = new FrameDataManager();
        Instance.Init();
    }

    private void Init()
    {
        FrameDataBehaviour.Setup();
    }

    public bool Enabled
    {
        get => FrameDataBehaviour.Instance.enabled;
        set => FrameDataBehaviour.Instance.SetEnable(value);
    }

    public static void CreateUIControls(GameObject contentRoot)
    {
        var toggle = UIFactory.CreateToggle(contentRoot, "Show frame data for attacks", out var frameDataToggle,
            out var frameDataToggleLabel);
        frameDataToggle.onValueChanged.AddListener(new Action<bool>((value) => { Instance.Enabled = value; }));
        frameDataToggleLabel.text = "Show frame data for attacks";
        UIFactory.SetLayoutElement(frameDataToggle.gameObject, minHeight: 25, minWidth: 50);
    }
    
    [HarmonyPatch(typeof(SimulationManager), nameof(SimulationManager.Initialize))]
    public class PatchSimulationInitialize
    {
        public static void Postfix()
        {
            if (Instance.Enabled)
                FrameDataBehaviour.Instance.SetupUpdateOverlayTargets();
        }
    }
}

public class FrameDataBehaviour : MonoBehaviour
{
    internal static FrameDataBehaviour Instance { get; private set; }


    public void SetEnable(bool value)
    {
        if (_startupOverlay != null)
            _startupOverlay.Enable = value;
        if (_frameAdvantageOverlay != null)
            _frameAdvantageOverlay.Enable = value;
        enabled = value;
    }

    internal static void Setup()
    {
        ClassInjector.RegisterTypeInIl2Cpp<FrameDataBehaviour>();
        GameObject gameObject = new GameObject("FrameDataBehaviour");
        DontDestroyOnLoad(gameObject);
        gameObject.hideFlags = HideFlags.HideAndDontSave;
        Instance = gameObject.AddComponent<FrameDataBehaviour>();
    }

    private LabelValueOverlayText _frameAdvantageOverlay;
    private LabelValueOverlayText _startupOverlay;

    // Character refs
    private static Character _playerCharacter;
    private static Character _dummyCharacter;

    // Frame tracker
    private static readonly Stopwatch TimeAnimation = new();
    private static double _playerCharacterTime;
    private static double _dummyCharacterTime;
    private static string _playerState;
    private static double _startupAnimation;
    private static bool _testStateBool;

    private static FrameData _currentFrameData = new()
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


    private void ResetTracker()
    {
        _playerCharacterTime = 0;
        _dummyCharacterTime = 0;
        _playerState = "";
        _startupAnimation = 0;
        _testStateBool = false;
        TimeAnimation.Reset();
        _currentFrameData = new()
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
    }

    public void SetupUpdateOverlayTargets()
    {
        if (_frameAdvantageOverlay == null)
        {
            _startupOverlay = new("Startup", "0", new Vector3(240, 240, 1));
            _frameAdvantageOverlay = new("Advantage", "0", new Vector3(240, 210, 1));
        }

        ResetTracker();
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
        }
    }

    private void Update()
    {
        // Only work in training mode
        if (MatchManager.instance?.matchType != MatchType.Training) return;

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
                    var plusOrMinus = _currentFrameData.Advantage >= 0 ? "+" : "";
                    _frameAdvantageOverlay.Value = $"{plusOrMinus}{_currentFrameData.Advantage}";
                    _startupOverlay.Value = $"{_currentFrameData.StartupFrames}";
                    ResetTracker();
                }
            }
        }
    }

    [HarmonyPatch(typeof(Character), nameof(Character.GetOffenseInfo))]
    public class PatchGetOffenseInfo
    {
        public static void Postfix(ref OffenseInfo __result)
        {
            if (_currentFrameData?.AttackName != __result.attackName && _currentFrameData.StartupFrames == 0)
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