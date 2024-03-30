using System;
using System.Diagnostics;
using GrimbaHack.Utility;
using Il2CppInterop.Runtime.Injection;
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
    public FrameDataBehaviour Behaviour { get; set; }

    static FrameDataManager()
    {
        Instance = new FrameDataManager();
        ClassInjector.RegisterTypeInIl2Cpp<FrameDataBehaviour>();
        GameObject go = new GameObject("FrameDataBehaviour");
        go.hideFlags = HideFlags.HideAndDontSave;
        GameObject.DontDestroyOnLoad(go);
        Instance.Behaviour = go.AddComponent<FrameDataBehaviour>();
        Instance.Behaviour.enabled = false;
        OnEnterTrainingMatchActionHandler.Instance.AddPostfix(() => Instance.Enabled = Instance._enabled);
        OnEnterMainMenuActionHandler.Instance.AddCallback(() => Instance.Behaviour.SetEnable(false));
        OnSimulationInitializeActionHandler.Instance.AddPostfix(() =>
        {
            if (Instance.Enabled)
                Instance.Behaviour.SetupUpdateOverlayTargets();
        });
    }

    private bool _enabled;

    public bool Enabled
    {
        get => Behaviour.enabled;
        set
        {
            Behaviour.SetEnable(value);
            _enabled = value;
        }
    }

    public static void CreateUIControls(GameObject contentRoot)
    {
        var toggle = UIFactory.CreateToggle(contentRoot, "Show frame data for attacks", out var frameDataToggle,
            out var frameDataToggleLabel);
        frameDataToggle.isOn = false;
        frameDataToggle.onValueChanged.AddListener(new Action<bool>((value) => { Instance.Enabled = value; }));
        frameDataToggleLabel.text = "Show frame data for attacks";
        UIFactory.SetLayoutElement(frameDataToggle.gameObject, minHeight: 25, minWidth: 50);
    }
}

public class FrameDataBehaviour : MonoBehaviour
{
    public FrameDataBehaviour()
    {
        OnApplyBlockAndHitStunActionHandler.Instance.AddCallback((bool isHitStun, int originalFrames) =>
        {
            if (!TimeAnimation.IsRunning) return;

            if (_currentFrameData != null)
            {
                var startup = (int)Math.Round(TimeAnimation.ElapsedMilliseconds / 16.67);
                _currentFrameData.StartupFrames = startup;
            }
        });

        OnCharacterGetOffenseInfoActionHandler.Instance.AddCallback((OffenseInfo result) =>
        {
            if (!enabled) return;
            if (_currentFrameData?.AttackName != result.attackName && _currentFrameData.StartupFrames == 0)
            {
                _currentFrameData = new FrameData
                {
                    AttackName = result.attackName,
                    HitstunFrames = result.hitStunFrames,
                    BlockstunFrames = result.blockStunFrames,
                    BaseDamage = result.baseDamage,
                    BlockedDamagePercent = result.blockedDamagePercent,
                    LaunchHeight = result.launchHeight
                };
            }
        });
    }

    public void SetEnable(bool value)
    {
        if (_startupOverlay != null)
            _startupOverlay.Enable = value;
        if (_frameAdvantageOverlay != null)
            _frameAdvantageOverlay.Enable = value;
        enabled = value;
        if (enabled)
            SetupUpdateOverlayTargets();
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
}