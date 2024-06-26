using System.Collections.Generic;
using GrimbaHack.Modules.ComboRecorder;
using GrimbaHack.Modules.ComboTrial.UI;
using GrimbaHack.UI.TrainingMode;
using GrimbaHack.Utility;
using HarmonyLib;
using Il2CppSystem;
using nway.gameplay;
using nway.gameplay.ai;
using nway.gameplay.match;
using nway.gameplay.ui;
using nway.ui;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GrimbaHack.Modules.ComboTrial;

public class ComboTrialManager
{
    public static readonly ComboTrialManager Instance = new();
    private int _selectedHeroIndex = 1;
    public List<ComboExport> Combos;
    public int ComboIndex;
    private ComboExport _combo;
    private GameObject _playerInputGo;
    private PlayerInputPlaybackController _playbackController;
    private Character player;
    private Character dummy;
    public bool IsComboTrial;
    private bool playbackQueued;
    private bool _completed;
    private readonly LabelValueOverlayText _statusOverlay = new("Status", "Playback", new Vector3(240, 240, 1));
    public bool IsPaused;

    static ComboTrialManager()
    {
        Instance._statusOverlay.Enabled = false;
        OnUIComboCounterOnBreakComboCallbackHandler.Instance.AddCallback((_, _) =>
        {
            if (Instance._completed) return;
            Instance.OnTrialFail();
        });
        OnEnterTrainingMatchActionHandler.Instance.AddPostfix(() =>
        {
            if (Instance.IsPaused)
            {
                Instance.IsPaused = false;
                return;
            }

            if (!Instance.IsComboTrial) return;
            Instance.ResetTrial();
            if (Instance.playbackQueued)
            {
                Instance.Playback();
            }
        });
        OnArmorTakeDamageCallbackHandler.Instance.AddPostfix(info =>
        {
            if (!Instance.IsComboTrial) return;
            if (info.isBlocked) return;

            if (ComboTrialOverlay.Instance.CheckAttack(info.attackName))
            {
                Instance.OnTrialComplete();
            }
        });
        OnEnterMainMenuActionHandler.Instance.AddCallback(() =>
        {
            if (!Instance.IsComboTrial) return;
            Instance.Teardown();
        });
    }

    private void OnTrialComplete()
    {
        Instance._completed = true;
        ComboTrialOverlay.Instance.Complete();
    }

    private void OnTrialFail()
    {
        ComboTrialOverlay.Instance.Restart();
    }

    public void SetupComboTrial(ComboExport combo)
    {
        Instance.IsComboTrial = true;
        Instance._playbackController = PlayerInputPlaybackController.Instance;
        Instance.SetCombo(combo);
        ComboTrialOverlay.Instance.Show();
        ComboTrialTitleOverlay.Instance.Show();
    }

    private void SetTrainingModeSettings()
    {
        var drivers = MatchManager.Get.CombatDriver;

        var trainingMeterDriver = drivers.FindExtension<TrainingMeterDriver>();
        trainingMeterDriver.InstantAssistCooldown = true;
        trainingMeterDriver.RefillSuperMeter = true;
        trainingMeterDriver.RemotePlayerRegenHealth = HealthRegen.FullHp;
        trainingMeterDriver.RecoverySpeed = TrainingOptions.HealthRecoverySpeed.Instant;
        // Config
        trainingMeterDriver.LocalPlayerSuperRefillLevel = _combo.SuperMeter;
        trainingMeterDriver.LocalPlayerMZMeterLevel = _combo.MzMeter;

        var dummyDriver = drivers.FindExtension<TrainingDummyDriver>();
        dummyDriver.Behavior = TrainingOptions.EnemyBehavior.Dummy;
        dummyDriver.Recovery = TrainingOptions.EnemyRecovery.InPlace;
        dummyDriver.Guard = TrainingOptions.EnemyGuard.NoGuard;
        dummyDriver.PushBlock = TrainingOptions.EnemyPushBlock.Never;
        dummyDriver.Pose = TrainingOptions.EnemyPose.Standing;

        var teamOrderDriver = drivers.FindExtension<TeamOrderDriver>();
        teamOrderDriver.KeepCurrentCharacterOnReset = false;
    }

    private void TriggerPlayback()
    {
        Instance.playbackQueued = true;
        var resetDriver = MatchManager.instance.CombatDriver.FindExtension<MatchResetDriver>();
        resetDriver.ResetTrainingBattle();
    }

    public void Playback()
    {
        Instance._statusOverlay.Enabled = true;
        PlaybackOverlayText.Enabled = false;
        Instance._playbackController.Playback(Instance._combo.Inputs);
        Instance.playbackQueued = false;
    }

    private void SetCombo(ComboExport combo)
    {
        Instance._combo = combo;
        ComboTrialOverlay.Instance.Init(combo.Combo);
        ComboTrialTitleOverlay.Instance.Init(combo.Title);
    }

    private void ResetTrial()
    {
        Instance._statusOverlay.Enabled = false;
        if (!PlaybackOverlayText.Enabled)
        {
            PlaybackOverlayText.Setup();
        }

        Instance.SetTrainingModeSettings();
        Instance._playbackController.Stop();
        Instance._completed = false;
        Instance.GetCharacters();
        Instance.SetStartingPosition();
        ComboTrialOverlay.Instance.Restart();
    }

    private void GetCharacters()
    {
        Instance.player = null;
        Instance.dummy = null;
        SceneStartup.Get.GamePlay._playerList.ForEach((Action<Character>)((Character character) =>
        {
            if (character.team == 0 && character.IsActiveCharacter)
            {
                Instance.player = character;
            }
            else if (character.team == 1 && character.IsActiveCharacter)

            {
                Instance.dummy = character;
            }
        }));
    }

    public void SetStartingPosition()
    {
        Instance.player.SetPosition(new Vector3F(Instance._combo.PlayerPosition[0], Instance._combo.PlayerPosition[1],
            Instance._combo.PlayerPosition[2]));
        Instance.dummy.SetPosition(new Vector3F(Instance._combo.DummyPosition[0], Instance._combo.DummyPosition[1],
            Instance._combo.DummyPosition[2]));
    }

    public void Teardown()
    {
        Instance._statusOverlay.Enabled = false;
        PlaybackOverlayText.Teardown();
        ComboTrialOverlay.Instance.Teardown();
        Instance.IsComboTrial = false;
        Instance._combo = new();
        if (Instance._playerInputGo)
        {
            Object.Destroy(Instance._playerInputGo);
        }

        Instance._playerInputGo = null;
        Instance.dummy = null;
        Instance.player = null;
        ComboTrialPauseMenu.Teardown();
    }

    [HarmonyPatch(typeof(SceneStartup), nameof(SceneStartup.AbortZone))]
    public class AbortZoneTrigger
    {
        static void Postfix()
        {
            Instance.Teardown();
        }
    }

    [HarmonyPatch(typeof(CommandRecordingDriver), nameof(CommandRecordingDriver.TogglePlaybackState))]
    public class OverrideTogglePlayback
    {
        static bool Prefix()
        {
            if (!Instance.IsComboTrial || ComboRecorderManager.Instance.Enabled) return true;
            Instance.TriggerPlayback();
            return false;
        }
    }

    [HarmonyPatch(typeof(CommandRecordingDriver), nameof(CommandRecordingDriver.ToggleRecordingState))]
    public class OverrideToggleRecording
    {
        static bool Prefix()
        {
            if (ComboRecorderManager.Instance.Enabled) return true;
            return !Instance.IsComboTrial;
        }
    }

    public void LoadTrialFromMatch()
    {
        ScreenFader.Get.StartFadeOut(Color.black, 0.5f);

        Instance._isQuickSwitchingTrial = true;
        var loadingScreen = new UIPostMatchLoadingScreen();
        loadingScreen.ShowNonmodalWindow();
        var screen =
            ComboTrialMenuManager.CreateTutorialSelection(
                ComboTrialDataManager.GetCharacterCombos(Instance._combo.CharacterId));
        LevelManager.Get.LeaveZone(screen, new IContextWrapper(loadingScreen));
    }


    private bool _isQuickSwitchingTrial { get; set; }


    public void ReturnToTrialSelect()
    {
        var loadingScreen = new UIPostMatchLoadingScreen();
        loadingScreen.ShowNonmodalWindow();
        var screen =
            ComboTrialMenuManager.CreateTutorialSelection(
                ComboTrialDataManager.GetCharacterCombos(Instance._combo.CharacterId));
        var shit = new IContextWrapper(loadingScreen);
        LevelManager.Get.LeaveZone(screen, shit);
        Instance.Teardown();
    }

    public void SetHero(int heroIndex)
    {
        Instance._selectedHeroIndex = heroIndex;
        LoadCharacterCombos();
    }

    private void LoadCharacterCombos()
    {
        var combos = ComboTrialDataManager.GetCharacterCombos(_selectedHeroIndex);
        if (combos.Count > 0)
        {
            Combos = combos;
            Instance.ComboIndex = 0;
        }
    }

    public void SetComboId(int index)
    {
        if (index > Combos.Count - 1)
        {
            Plugin.Log.LogInfo(
                $"[ComboTrialManager.SetComboId]: Index greater than combo list ({index} > {Combos.Count - 1}");
            return;
        }

        Instance.ComboIndex = index;
    }

    public ComboExport GetCurrentCombo()
    {
        return Instance.Combos?[Instance.ComboIndex];
    }

    public bool SetToPreviousTrial()
    {
        if (Instance.ComboIndex - 1 >= 0)
        {
            SetComboId(Instance.ComboIndex - 1);
            return true;
        }

        return false;
    }

    public bool SetToNextTrial()
    {
        if (Instance.ComboIndex + 1 < Instance.Combos.Count)
        {
            SetComboId(Instance.ComboIndex + 1);
            return true;
        }

        return false;
    }

    [HarmonyPatch(typeof(ScreenTutorial), nameof(ScreenTutorial.OnExit))]
    public class ScreenTutorialOnExit
    {
        public static bool Prefix()
        {
            if (Instance._isQuickSwitchingTrial)
            {
                Instance._isQuickSwitchingTrial = false;
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(ScreenTutorial), nameof(ScreenTutorial.OnEnter))]
    public class ScreenTutorialOnEnter
    {
        public static bool Prefix()
        {
            if (Instance._isQuickSwitchingTrial)
            {
                SceneManager.HideLoadingScreen();
                ComboTrialMenuManager.StartGame();
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(GameManager), nameof(GameManager.RequestPauseApp))]
    public class GMRequestPause
    {
        public static void Postfix(
            PauseType reason,
            string message,
            bool manualPausePanel = false,
            string logMessage = null
        )
        {
            if (!Instance.IsComboTrial) return;
            Instance.IsPaused = true;
        }
    }
}
