using System;
using System.Collections.Generic;
using epoch.db;
using GrimbaHack.Data;
using GrimbaHack.UI.Popup.TrainingSettings.MenuItems;
using GrimbaHack.Utility;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using nway.gameplay;
using nway.gameplay.match;
using nway.gameplay.online;
using nway.gameplay.ui;
using nway.network;
using nway.ui;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GrimbaHack.Modules.TrainingModeQueue;

public static class TrainingModeQueue
{
    private static Match _queuedFromMatch;
    private static UISearchingOpponent _listenerWindow;
    private static bool _searchActive;
    private static bool _matchStarted;
    private static GameObject _watchdog;
    private static RankedQueueSpinnerBehaviour Spinner { get; set; }
    private static bool _returnToTraining;
    private static Match _returnToTrainingMatch;
    private static TeamHeroSelection _trainingP1;
    private static TeamHeroSelection _trainingP2;
    private static string _trainingArena;
    private static bool _autoRequeue;
    private static bool _enabled;


    static TrainingModeQueue()
    {
        OnSceneStartupOnDestroyActionHandler.Instance.AddCallback(Reset);
        OnUIHeroSelectSelectNextActionHandler.Instance.AddCallback((_, _) =>
        {
            Reset();
            _autoRequeue = false;
        });
        OnEnterTrainingMatchActionHandler.Instance.AddPostfix(() =>
        {
            if (_autoRequeue)
            {
                StartQueue();
                _autoRequeue = false;
            }
        });

        OnMatchManagerSetupGamePlay.Instance.AddCallback((match, _, _) =>
        {
            if (match != null && _returnToTrainingMatch != null && match.Pointer == _returnToTrainingMatch.Pointer)
            {
                Plugin.Log.LogInfo(
                    _returnToTraining
                        ? $"[RankedQueue] SetupGamePlay of the queue-launched ranked match - one-shot exemption consumed; return-to-training state kept (player team={(_trainingP1 != null ? "yes" : "NO")} dummy team={(_trainingP2 != null ? "yes" : "NO")} arena={_trainingArena ?? "none"}) until the post-match screen consumes it."
                        : "[RankedQueue] SetupGamePlay of the armed-for match - no return-to-training state to protect.");
                _returnToTrainingMatch = null;
                return;
            }

            if (_returnToTraining)
            {
                Plugin.Log.LogInfo("[RankedQueue] Clearing the return-to-training flag on a new match setup.");
            }

            _returnToTraining = false;
            _returnToTrainingMatch = null;
            _trainingP1 = null;
            _trainingP2 = null;
            _trainingArena = null;
        });
    }

    public static void SetEnabled(bool value)
    {
        _enabled = value;
    }

    private static void ShowSpinner(bool visible)
    {
        if (Spinner == null)
        {
            Spinner = RankedQueueSpinnerBehaviour.Init();
        }
        Spinner.SetEnabled(visible);
    }

    public static bool IsEnabled()
    {
        return _enabled;
    }


    internal static void Reset()
    {
        if (_searchActive)
        {
            _searchActive = false;
            _queuedFromMatch = null;
            try
            {
                GameManager.Get.onlineServices.OnlineMatch.CancelFindingMatch(false);
            }
            catch (Exception e)
            {
                Plugin.Log.LogWarning("[RankedQueue] Failed to request the cancel: " + e);
            }
        }

        StopWatchdog();
        Spinner?.Clear();

        _queuedFromMatch = null;
        _listenerWindow = null;
        _searchActive = false;
        _matchStarted = false;
        _enabled = false;
    }


    public static void StartQueue()
    {
        _matchStarted = false;
        _returnToTraining = false;
        _returnToTrainingMatch = null;
        _trainingP1 = null;
        _trainingP2 = null;
        _trainingArena = null;

        var match = MatchManager.Get.match;
        if (match == null)
        {
            Plugin.Log.LogWarning("[RankedQueue] No active match found - cannot queue for ranked.");
            return;
        }

        if (!Global.IsTrainingMatch())
        {
            Plugin.Log.LogWarning(
                $"[RankedQueue] Not in a training match (matchType={MatchManager.instance.matchType}) - cannot queue for ranked.");
            return;
        }

        var userList = match.GetMatchUserList();
        if (userList == null || userList.Count < 2)
        {
            Plugin.Log.LogWarning("[RankedQueue] The match has no player list - cannot queue for ranked.");
            return;
        }

        var localSlot = MatchManager.Get.GetLocalPlayerSlot();
        if (localSlot < 0 || localSlot >= userList.Count)
        {
            localSlot = userList.FindIndex(new Func<PlayerInfo, bool>(p => p.pid == UserPersistence.Get.PID));
        }

        if (localSlot < 0 || localSlot >= userList.Count)
        {
            var sb = new System.Text.StringBuilder();
            for (var i = 0; i < userList.Count; ++i)
            {
                sb.Append(i).Append(":['").Append(userList[i].pid ?? "null").Append("'] ");
            }

            Plugin.Log.LogWarning(
                $"[RankedQueue] Could not identify the local player in the match. userPids=[{sb}] " +
                $"UserPersistence.PID=[{(UserPersistence.Get.PID ?? "null")}] " +
                $"profile.PlayerID=[{(GameManager.instance?.primaryUser?.playerProfile?.PlayerID ?? "null")}] " +
                "localSlot=" + MatchManager.Get.GetLocalPlayerSlot() + " - cannot queue.");
            return;
        }

        var localPlayer = userList[localSlot].GetTeamSelection();
        if (localPlayer == null)
        {
            Plugin.Log.LogWarning($"[RankedQueue] No team selection for the local player (slot {localSlot}) - cannot queue.");
            return;
        }

        Action<bool> serviceGate = (eligible) =>
        {
            if (!eligible)
            {
                Plugin.Log.LogWarning("[RankedQueue] Online services check failed - not queueing.");
                return;
            }

            if (GameManager.Get.onlineServices.Episode.ShouldDisableRankedMode(Timer.Server.GetCurrentTime()))
            {
                Plugin.Log.LogWarning("[RankedQueue] Ranked mode is currently disabled (episode) - not queueing.");
                return;
            }

            try
            {
                var win = new UISearchingOpponent(UISearchingOpponent.Mode.MainMenu, MatchRule.CreateDefault());
                win.matchType = MatchType.Ranked;
                win.p1 = localPlayer;
                win.rematch = false;

                _listenerWindow = win;
                _queuedFromMatch = match;
                _searchActive = true;
                var findListener = new IFindMatchListener(win.Pointer);
                var connectListener = new IMatchConnectListener(win.Pointer);
                GameManager.Get.onlineServices.OnlineMatch.FindMatch(
                    localPlayer, MatchType.Ranked, false, null, null, MatchRule.CreateDefault(),
                    findListener, connectListener, null);
                Plugin.Log.LogInfo(
                    "[RankedQueue] Searching for a ranked opponent - you can keep playing in the training match.");
            }
            catch (Exception e)
            {
                Plugin.Log.LogError("[RankedQueue] Failed to start the ranked queue: " + e);
                _searchActive = false;
                _queuedFromMatch = null;
            }
        };

        GameManager.Get.CheckOnlineServiceAndProceed(true, serviceGate);
    }

    public static void CancelSearch()
    {
        if (!_searchActive)
        {
            return;
        }

        _searchActive = false;
        _queuedFromMatch = null;
        try
        {
            GameManager.Get.onlineServices.OnlineMatch.CancelFindingMatch(false);
        }
        catch (Exception e)
        {
            Plugin.Log.LogWarning("[RankedQueue] Failed to request the cancel: " + e);
        }
        Plugin.Log.LogInfo("[RankedQueue] Cancelled search.");
    }
    
    private static bool StaleQueue()
    {
        if (_queuedFromMatch == null)
        {
            return true;
        }

        var current = MatchManager.Get.match;
        if (current == null || _queuedFromMatch.Pointer != current.Pointer)
        {
            Plugin.Log.LogWarning("[RankedQueue] The original match is gone - ignoring the search result.");
            EndSearch();
            return true;
        }

        return false;
    }

    private static void EndSearch()
    {
        _searchActive = false;
        _queuedFromMatch = null;
        CancelSearch();
    }

    internal static void HandleFindResult(MatchMakingResult result, HTTPResultCode code)
    {
        if (!_searchActive)
        {
            return;
        }

        if (StaleQueue())
        {
            return;
        }

        if (result == MatchMakingResult.FoundMatch)
        {
            Plugin.Log.LogInfo("[RankedQueue] Opponent found - connecting to the match server...");
            return;
        }

        Plugin.Log.LogWarning($"[RankedQueue] Search ended without a match ({result}, {code}).");
        EndSearch();
        StartQueue();
    }

    internal static void HandleConnectResult(MatchConnectingResult result, PvPMatch match)
    {
        if (!_searchActive)
        {
            return;
        }

        if (StaleQueue())
        {
            return;
        }

        if (result == MatchConnectingResult.Success && match != null)
        {
            _searchActive = false;
            _listenerWindow = null;
            _queuedFromMatch = null;

            Plugin.Log.LogInfo("[RankedQueue] Connected - ending the training match to start the ranked match...");
            StartMatchTransition(match);
            return;
        }

        Plugin.Log.LogWarning($"[RankedQueue] Did not join the found match ({result}).");
        EndSearch();
    }

    private static void StartMatchTransition(PvPMatch foundMatch)
    {
        try
        {
            try
            {
                var match = MatchManager.Get.match;
                var userList = match?.GetMatchUserList();
                if (userList != null && userList.Count == 2)
                {
                    var slot = MatchManager.Get.GetLocalPlayerSlot();
                    if (slot < 0 || slot >= userList.Count)
                    {
                        slot = userList.FindIndex(new Func<PlayerInfo, bool>(p => p.pid == UserPersistence.Get.PID));
                    }

                    if (slot >= 0 && slot < userList.Count)
                    {
                        _trainingArena = match.GetArenaId();
                        _trainingP1 = CopyTeam(userList[slot].GetTeamSelection());
                        _trainingP2 = CopyTeam(userList[1 - slot].GetTeamSelection());

                        if (_trainingP2 == null && _trainingP1 != null)
                        {
                            Plugin.Log.LogInfo("[RankedQueue] The dummy had no team of its own - it will fight a copy of the player's team.");
                            _trainingP2 = CopyTeam(userList[slot].GetTeamSelection());
                        }

                        Plugin.Log.LogInfo(
                            $"[RankedQueue] Captured return-to-training state: arena={_trainingArena} " +
                            $"player team={(_trainingP1 != null ? "yes" : "NO")} dummy team={(_trainingP2 != null ? "yes" : "NO")}.");
                    }
                }
            }
            catch (Exception e)
            {
                Plugin.Log.LogWarning("[RankedQueue] Could not capture the training match teams/arena for the Find Match return: " + e);
            }

            StopWatchdog();

            MatchManager.Get.CompleteMatch();

            AudioManager.Get.StopAllSFX();
            LevelManager.Get.UnloadScene();
            UIManager.Get.SetLayerScreenSpaceOverlay(UILayerType.BackgroundOrthographic);
            UserPersistence.Get.TrackedStats.RefreshStatCache();
            UserPersistence.Get.TrackedStats.UpdateUnlockedAchievements();

            var menuScreen = new ScreenMainMenu(MainMenuSelection.None);
            Action<ILoadingContext> onMenuLoaded = _ => OnMainMenuLoaded(foundMatch);
            SceneManager.LeaveCombatScene(menuScreen, onMenuLoaded);

            _watchdog = RankedQueueTransitionWatchdog.Start();
        }
        catch (Exception e)
        {
            Plugin.Log.LogError("[RankedQueue] Failed to leave the training match to start the ranked match: " + e);
            StopWatchdog();
            EndSearch();
        }
    }

    private static void OnMainMenuLoaded(PvPMatch foundMatch)
    {
        if (_matchStarted)
        {
            return;
        }

        _matchStarted = true;
        StopWatchdog();

        Plugin.Log.LogInfo("[RankedQueue] Back in the main menu - starting the ranked match.");

        _returnToTraining = true;
        _returnToTrainingMatch = foundMatch;
        _autoRequeue = true;

        try
        {
            var startup = MatchManager.Get.CreateOnlinePvPMatch(foundMatch);
            new PvPGamePlay(startup).Start();
            MatchManager.ShowLoadingScreen(foundMatch);

            Plugin.Log.LogInfo(
                "[RankedQueue] Ranked match started - its post-match screen's Find Match will return to training.");
        }
        catch (Exception e)
        {
            Plugin.Log.LogError("[RankedQueue] Failed to start the found match: " + e);
            _returnToTraining = false;
            _returnToTrainingMatch = null;
            _trainingP1 = null;
            _trainingP2 = null;
            _trainingArena = null;
        }

        _searchActive = false;
        Spinner.Clear();
        Plugin.Log.LogInfo("[RankedQueue] Selector reset to idle.");
    }

    private static void StopWatchdog()
    {
        if (_watchdog != null)
        {
            Object.Destroy(_watchdog);
            _watchdog = null;
        }
    }
    
    private static TeamHeroSelection CopyTeam(TeamHeroSelection source)
    {
        if (source == null)
        {
            return null;
        }

        var copy = new TeamHeroSelection();
        int captured = 0;
        for (int s = 0; s < 16; ++s)
        {
            var hero = source.GetHero(s);
            if (hero == null)
            {
                continue;
            }

            if (hero.skin != null)
            {
                copy.SetIndexSkinAndColor(s, hero.index, hero.skin, hero.color);
            }
            else
            {
                copy.SetIndex(s, hero.index);
            }

            captured++;
        }

        return captured > 0 ? copy : null;
    }

    internal static bool TryConsumeReturnToTraining(out TeamHeroSelection p1, out TeamHeroSelection p2, out string arena)
    {
        bool wasSet = _returnToTraining;
        p1 = _trainingP1;
        p2 = _trainingP2;
        arena = _trainingArena;

        _returnToTraining = false;
        _returnToTrainingMatch = null;
        _trainingP1 = null;
        _trainingP2 = null;
        _trainingArena = null;

        return wasSet;
    }
    internal static bool IsHeadlessWindow(UISearchingOpponent candidate)
        => _listenerWindow != null && candidate.Pointer == _listenerWindow.Pointer;

    [HarmonyPatch(typeof(UISpectateOptions), nameof(UISpectateOptions.OnHide))]
    class  UISpectateOptionsOnInitializeComponents
    {
        static void Postfix()
        {
            if (_enabled && !_searchActive)
            {
                StartQueue();
            }
            else if (!_enabled)
            {
                CancelSearch();
            }
            ShowSpinner(_enabled);
        }
    }
}

public class RankedQueueSpinnerBehaviour : MonoBehaviour
{
    private GameObject _spinner;
    private bool _missingIndicatorLogged;
    private static bool _typeRegistered;

    private static void RegisterClass()
    {
        if (!_typeRegistered)
        {
            ClassInjector.RegisterTypeInIl2Cpp<RankedQueueSpinnerBehaviour>();
            _typeRegistered = true;
        }
    }
    
    public static RankedQueueSpinnerBehaviour Init()
    {
        RegisterClass();
        var go = new GameObject("RankedQueueSpinnerBehaviour");
        DontDestroyOnLoad(go);
        var rqsb = go.AddComponent<RankedQueueSpinnerBehaviour>();
        rqsb.enabled = false;
        return rqsb;
    }
    
    public void SetEnabled(bool value)
    {
        enabled = value;
        if (_spinner == null)
        {
            CreateSpinner();
        }

        if (_spinner != null)
        {
            _spinner.SetActive(value);
        }
    }
    
    public void Clear()
    {
        if (_spinner != null)
        {
            Destroy(_spinner);
        }

        _spinner = null;
        enabled = false;
    }
    
    private void CreateSpinner()
    {
        Plugin.Log.LogInfo("[RankedQueueSpinnerBehaviour] Creating Spinner");
        if (_spinner != null)
        {
            Plugin.Log.LogInfo("[RankedQueueSpinnerBehaviour] spinner not null");
            return;
        }

        var siObject = UIManager.Get.savingIndicator?.transform;
        if (siObject == null)
        {
            if (!_missingIndicatorLogged)
            {
                Plugin.Log.LogWarning("[RankedQueue] No UI_SavingIndicator available yet - will keep retrying until the UI is ready.");
                _missingIndicatorLogged = true;
            }
            return;
        }

        _missingIndicatorLogged = false;
        _spinner = Instantiate(siObject.gameObject);
        var canvas = _spinner.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var root = _spinner.transform.Find("root");
        if (root != null)
        {
            List<string> children = ["saving", "saved"];
            foreach (var child in children)
            {
                var result = root.Find(child);
                if (result != null)
                {
                    Destroy(result.gameObject);
                }
            }

            var red = root.Find("circle")?.Find("red");
            if (red != null)
            {
                Destroy(red.gameObject);
            }

            root.Find("image")?.gameObject?.SetActive(true);
            root.localPosition = new Vector3(50, _spinner.transform.localPosition.y * -1, 0);

        }

        _spinner.active = false;
    }
}

[HarmonyPatch(typeof(UIHeroSelect), nameof(UIHeroSelect.OnShow))]
internal class UIHeroSelectOnShow
{
    static void Postfix()
    {
        TrainingModeQueue.CancelSearch();
        TrainingModeQueue.Reset();
    }
}