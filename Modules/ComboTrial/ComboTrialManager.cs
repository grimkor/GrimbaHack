using System.Collections.Generic;
using GrimbaHack.UI.ComboTrial;
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
    public static ComboTrialManager Instance = new();
    private int _selectedHeroIndex = 1;
    public List<ComboExport> Combos;
    private int _comboIndex;
    private ComboExport _combo;
    private GameObject _playerInputGo;
    private PlayerInputPlaybackBehaviour _playbackBehaviour;
    private Character player;
    private Character dummy;
    public bool IsComboTrial;
    private bool playbackQueued;
    private bool _completed;

    static ComboTrialManager()
    {
        OnUIComboCounterOnBreakComboCallbackHandler.Instance.AddCallback((_, _) =>
        {
            if (Instance._completed) return;
            Instance.OnTrialFail();
        });
        OnEnterTrainingMatchActionHandler.Instance.AddPostfix(() =>
        {
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

            if (UIComboTrial.Instance.CheckAttack(info.attackName))
            {
                Instance.OnTrialComplete();
            }
        });
    }

    private void OnTrialComplete()
    {
        Instance._completed = true;
        UIComboTrial.Instance.Complete();
    }

    private void OnTrialFail()
    {
        UIComboTrial.Instance.Restart();
    }

    public void Init(ComboExport combo)
    {
        Instance.IsComboTrial = true;
        Instance._playerInputGo = new GameObject("playbackBehaviour");
        Object.DontDestroyOnLoad(Instance._playerInputGo);
        Instance._playbackBehaviour = Instance._playerInputGo.AddComponent<PlayerInputPlaybackBehaviour>();
        Instance._playbackBehaviour.enabled = false;
        Instance.SetCombo(combo);
        UIComboTrial.Instance.Show();
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
        dummyDriver.PushBlock = TrainingOptions.EnemyPushBlock.Never;
        //Config
        // dummyDriver.Pose = TrainingOptions.EnemyPose.Jumping;

        var teamOrderDriver = drivers.FindExtension<TeamOrderDriver>();
        teamOrderDriver.KeepCurrentCharacterOnReset = true;
    }

    private void TriggerPlayback()
    {
        Instance.playbackQueued = true;
        var resetDriver = MatchManager.instance.CombatDriver.FindExtension<MatchResetDriver>();
        resetDriver.ResetTrainingBattle();
    }

    public void Playback()
    {
        Instance._playbackBehaviour.Playback(Instance.player.GetCharacterTeam().GetInputSystem(),
            Instance._combo.Inputs, 30);
        Instance.playbackQueued = false;
    }

    private void SetCombo(ComboExport combo)
    {
        Instance._combo = combo;
        UIComboTrial.Instance.Init(combo.Combo);
    }

    private void ResetTrial()
    {
        Instance.SetTrainingModeSettings();
        Instance._playbackBehaviour.enabled = false;
        Instance._completed = false;
        Instance.GetCharacters();
        Instance.SetStartingPosition();
        UIComboTrial.Instance.Restart();
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
        UIComboTrial.Instance.Hide();
        Instance.IsComboTrial = false;
        Instance._combo = new();
        Object.Destroy(Instance._playerInputGo);
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
            if (!Instance.IsComboTrial) return true;
            Instance.TriggerPlayback();
            return false;
        }
    }

    [HarmonyPatch(typeof(CommandRecordingDriver), nameof(CommandRecordingDriver.ToggleRecordingState))]
    public class OverrideToggleRecording
    {
        static bool Prefix()
        {
            return !Instance.IsComboTrial;
        }
    }

    public void LoadTrialFromMatch()
    {
        ScreenFader.Get.StartFadeOut(Color.black, 0.5f);

        Instance.TestFlag = true;
        var loadingScreen = new UIPostMatchLoadingScreen();
        loadingScreen.ShowNonmodalWindow();
        var screen =
            GrimUIComboTrialController.CreateTutorialSelection(
                ComboTrialDataManager.GetCharacterCombos(Instance._combo.CharacterId));
        LevelManager.Get.LeaveZone(screen, new IContextWrapper(loadingScreen));
    }

    [HarmonyPatch(typeof(ScreenTutorial), nameof(ScreenTutorial.OnExit))]
    public class TestExit
    {
        public static bool Prefix()
        {
            if (Instance.TestFlag)
            {
                Instance.TestFlag = false;
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(ScreenTutorial), nameof(ScreenTutorial.OnEnter))]
    public class Test
    {
        public static bool Prefix()
        {
            if (Instance.TestFlag)
            {
                SceneManager.HideLoadingScreen();
                GrimUIComboTrialController.StartGame();
                return false;
            }

            return true;
        }
    }

    public bool TestFlag { get; set; }


    public void ReturnToTrialSelect()
    {
        var loadingScreen = new UIPostMatchLoadingScreen();
        loadingScreen.ShowNonmodalWindow();
        var screen =
            GrimUIComboTrialController.CreateTutorialSelection(
                ComboTrialDataManager.GetCharacterCombos(Instance._combo.CharacterId));
        var shit = new IContextWrapper(loadingScreen);
        LevelManager.Get.LeaveZone(screen, shit);
        Instance.Teardown();
    }

    public void SetHero(int heroIndex)
    {
        _selectedHeroIndex = heroIndex;
        LoadCharacterCombos();
    }

    private void LoadCharacterCombos()
    {
        var combos = ComboTrialDataManager.GetCharacterCombos(_selectedHeroIndex);
        if (combos.Count > 0)
        {
            Combos = combos;
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

        Instance._comboIndex = index;
    }

    public ComboExport GetCurrentCombo()
    {
        return Instance.Combos?[Instance._comboIndex];
    }

    public bool SetToPreviousTrial()
    {
        if (Instance._comboIndex - 1 >= 0)
        {
            SetComboId(Instance._comboIndex - 1);
            return true;
        }

        return false;
    }

    public bool SetToNextTrial()
    {
        if (Instance._comboIndex + 1 < Instance.Combos.Count)
        {
            SetComboId(Instance._comboIndex + 1);
            return true;
        }

        return false;
    }
}
