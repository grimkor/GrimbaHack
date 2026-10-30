using System;
using System.Collections.Generic;
using GrimbaHack.Utility;
using HarmonyLib;
using nway.gameplay;
using nway.gameplay.match;
using nway.gameplay.online;
using nway.gameplay.ui;
using nway.network;
using nway.ui;
using UnityEngine;
using MatchType = epoch.db.MatchType;
using GameSceneManager = nway.gameplay.SceneManager;
using Object = UnityEngine.Object;

namespace GrimbaHack.UI.Popup.TrainingSettings.MenuItems;

/// <summary>
/// Queues the local player for the online Ranked queue straight from the training menu,
/// using the game's native matchmaking (OnlineMatchManager.FindMatch -> server polling ->
/// match connect), but HEADLESS: no UISearchingOpponent modal window, so the training scene
/// stays fully playable while the search runs in the background. The menu item itself shows
/// the search state (Searching... / Opponent found! Connecting...) and flipping it back to
/// Disabled cancels the search (the same CancelFindingMatch path the game's own cancel button
/// uses).
///
/// Starting the found match: the game only ever starts a match from a NON-match scene (main
/// menu, hero select, lobby, post-match screen) - its SceneManager/LevelManager transition
/// machinery is built for that, so the match must NOT be started while the training scene is
/// still the active scene (that is what stalls the loading screen). Instead, once the
/// opponent's server is joined the mod runs the game's own match-end transition (the one the
/// pause menu uses: LevelManager.LeaveZone -> SceneManager.LeaveCombatScene -> HubEmpty scene
/// -> main-menu scene) and, once the main-menu scene is active, starts the found match exactly
/// like the game's own ranked flow: CreateOnlinePvPMatch -> PvPGamePlay.Start() ->
/// ShowLoadingScreen. A small watchdog (RankedQueueTransitionWatchdog) keeps the transition
/// moving if the game's scene events do not fire on their own.
/// </summary>
static class RankedQueueSelector
{
    private const string LabelIdle = "Queue Online Ranked";
    private const string LabelSearching = "Searching for ranked opponent...";
    private const string LabelConnecting = "Opponent found! Connecting...";

    private enum SearchState
    {
        Idle,
        Searching,
        Connecting
    }

    private static MenuListSelector<DefaultMenuOptions> _selector;
    private static SearchState _state = SearchState.Idle;
    private static bool _updating;
    private static Match _queuedFromMatch;
    private static UISearchingOpponent _listenerWindow;
    private static bool _searchActive;
    private static System.Action _closePopup;
    private static bool _matchStarted;
    private static GameObject _watchdog;
    private static GameObject _uiSpinner;

    static RankedQueueSelector()
    {
        // A training match always starts from a clean slate: wipe any leftover
        // search/transition state of a previous cycle on entry.
        OnSceneStartupOnDestroyActionHandler.Instance.AddCallback(Reset);
    }

    /// <summary>
    /// Creates the "Queue Online Ranked" item on the training settings page.
    /// </summary>
    /// <param name="menuPage">The settings page the item is added to.</param>
    /// <param name="closePopup">Closes the settings popup once the search is running.</param>
    public static void Generate(UIPage menuPage, System.Action closePopup)
    {
        _closePopup = closePopup;
        var selector = menuPage.AddItem<MenuListSelector<DefaultMenuOptions>>("rankedQueueSelector");
        var items = new Il2CppSystem.Collections.Generic.List<DefaultMenuOptions>();
        items.Add(DefaultMenuOptions.Disabled);
        items.Add(DefaultMenuOptions.Enabled);
        selector.Items =
            items.TryCast<Il2CppSystem.Collections.Generic.IList<DefaultMenuOptions>>();
        // A non-idle state without an active search is a stale leftover of a finished
        // cycle (the match already started, the state was never reset) - heal it so the
        // item is never presented as stuck "Enabled" and can start a new search.
        if (!_searchActive && _state != SearchState.Idle)
        {
            _state = SearchState.Idle;
        }

        selector.LocalizedText = StateLabel();
        selector.CurrentItem = _state == SearchState.Idle ? DefaultMenuOptions.Disabled : DefaultMenuOptions.Enabled;
        selector.OnValueChanged = (Action<DefaultMenuOptions, DefaultMenuOptions>)((newValue, _) =>
        {
            if (_updating)
            {
                return;
            }

            if (_state == SearchState.Idle)
            {
                if (newValue == DefaultMenuOptions.Enabled)
                {
                    StartQueue();
                }
                return;
            }

            // Search in progress: flipping back to Disabled cancels it.
            if (newValue == DefaultMenuOptions.Disabled)
            {
                CancelSearch();
                return;
            }

            // Re-enabling while already searching: keep it as the current state.
            _updating = true;
            selector.CurrentItem = DefaultMenuOptions.Disabled;
            _updating = false;
        });
    }

    /// <summary>
    /// Called when the settings popup is hidden/destroyed: the menu item (and its page) is
    /// gone, so never touch the selector again from a later callback.
    /// </summary>
    internal static void OnWindowClosed()
    {
        _selector = null;
    }

    /// <summary>
    /// Returns the selector to its default state: cancels any search that is still
    /// running (the same cancel path the game's own cancel button uses), stops the
    /// transition watchdog, destroys the spinner clone, and resets every state field, so
    /// the menu item reads Idle/Disabled and can start a fresh search when next shown.
    /// Safe to call at any time; it is run on entry to every training match.
    /// </summary>
    internal static void Reset()
    {
        Plugin.Log.LogInfo("[RankedQueue] Reset");
        // A search that outlived the match it was started in (the player left without
        // cancelling) is still polling on the server side: cancel it the way the game
        // does. Done directly rather than through CancelSearch/UpdateUi so no selector
        // or spinner is touched in the middle of a scene change.
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

        if (_uiSpinner != null)
        {
            Object.Destroy(_uiSpinner);
        }

        _selector = null;
        _state = SearchState.Idle;
        _updating = false;
        _queuedFromMatch = null;
        _listenerWindow = null;
        _searchActive = false;
        _closePopup = null;
        _matchStarted = false;
        _uiSpinner = null;
    }

    /// <summary>
    /// True when the given window instance is the headless listener window created by this
    /// selector. The comparison uses the native pointer, because the interop creates a fresh
    /// managed wrapper for every native-to-managed call.
    /// </summary>
    internal static bool IsHeadlessWindow(UISearchingOpponent candidate)
        => _listenerWindow != null && candidate.Pointer == _listenerWindow.Pointer;

    private static string StateLabel()
        => _state switch
        {
            SearchState.Searching => LabelSearching,
            SearchState.Connecting => LabelConnecting,
            _ => LabelIdle
        };

    private static void UpdateUi(string label, SearchState newState)
    {
        _state = newState;
        ShowSpinner(newState != SearchState.Idle);
        if (_selector == null)
        {
            return;
        }

        _updating = true;
        _selector.LocalizedText = label;
        if (newState == SearchState.Idle && _selector.CurrentItem == DefaultMenuOptions.Enabled)
        {
            _selector.CurrentItem = DefaultMenuOptions.Disabled;
        }

        _updating = false;
    }

    private static void ShowSpinner(bool show)
    {
        if (_uiSpinner == null)
        {
            var indicator = UIManager.Get.savingIndicator;
            if (indicator == null || indicator.transform == null)
            {
                Plugin.Log.LogWarning("[RankedQueue] No UI_SavingIndicator available in this scene - spinner will not show.");
                return;
            }

            Transform siObject = indicator.transform;
            Plugin.Log.LogInfo($"isObject: {siObject.gameObject.name}");
            if (siObject != null)
            {
                _uiSpinner = Object.Instantiate(siObject.gameObject);
                Plugin.Log.LogInfo($"uiSpinner: {_uiSpinner.name}");
                var canvas = _uiSpinner.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                var root = _uiSpinner.transform.Find("root");
                if (root != null)
                {
                    List<string> children = ["saving", "saved"];
                    foreach (var child in children)
                    {
                        var result = root.Find(child);
                        if (result != null)
                        {
                            Object.Destroy(result.gameObject);
                            Plugin.Log.LogInfo($"destroyed: {result}");
                        }
                    }
                }
                var red = root.transform.Find("circle")?.Find("red");
                if (red != null)
                {
                    Object.Destroy(red.gameObject);
                }

                root.transform.localPosition = new Vector3(50, _uiSpinner.transform.localPosition.y * -1, 0);
            }
        }
        _uiSpinner.active = show;
    }

    private static void StartQueue()
    {
        // A new search cycle: clear the one-shot guard of any earlier transition in this
        // session, so a second ranked match (after the first one ended and the user queued
        // again from a new training match) is not blocked.
        _matchStarted = false;

        var match = MatchManager.Get.match;
        if (match == null)
        {
            Plugin.Log.LogWarning("[RankedQueue] No active match found - cannot queue for ranked.");
            return;
        }

        // Same training-mode detection the rest of the mod uses (Data.Global).
        // (Note: an `is TrainingMatch` cast does NOT work here - the interop wrapper for
        // MatchManager.match does not report its native runtime type, so it can't be cast down.)
        if (!Data.Global.IsTrainingMatch())
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

        // During match setup the game already resolved which side of the user list the local
        // player sits on: MatchManager.SetupGamePlay -> GamePlayStartup.UpdateSlots(MatchManager.GetPID()),
        // where GetPID() == UserPersistence.Get.PID (NOT the profile's PlayerID). Ask for that
        // pre-computed slot instead of re-deriving it by string-matching PIDs ourselves.
        var localSlot = MatchManager.Get.GetLocalPlayerSlot();
        if (localSlot < 0 || localSlot >= userList.Count)
        {
            // Fallback: the same pid comparison UpdateSlots performs.
            localSlot = userList.FindIndex(new Func<PlayerInfo, bool>(p => p.pid == UserPersistence.Get.PID));
        }

        if (localSlot < 0 || localSlot >= userList.Count)
        {
            // Log everything useful so the next run is diagnosable in one shot.
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

        // Same service gate the game's own ranked flow uses (signed-in, no maintenance, etc.).
        // The callback is a STANDARD System.Action here (plain C# lambda conversion); the
        // interop Il2CppSystem.Action<bool> parameter is reached through its implicit operator
        // at the call site - that keeps the Roslyn interop-delegate conversion quirks out of it.
        System.Action<bool> serviceGate = (eligible) =>
        {
            if (!eligible)
            {
                Plugin.Log.LogWarning("[RankedQueue] Online services check failed - not queueing.");
                return;
            }

            // Ranked is locked out during live episodes - the game shows a box and bails
            if (GameManager.Get.onlineServices.Episode.ShouldDisableRankedMode(Timer.Server.GetCurrentTime()))
            {
                Plugin.Log.LogWarning("[RankedQueue] Ranked mode is currently disabled (episode) - not queueing.");
                return;
            }

            try
            {
                // The manager requires two listener objects (IFindMatchListener and
                // IMatchConnectListener) and a managed class cannot implement those interop
                // types, so the listeners are the game's own UISearchingOpponent window,
                // created exactly the way the game's ranked button creates it. It is NEVER
                // shown: its two listener methods are intercepted by
                // SearchOpponentHeadlessPatches, which skip the window's UI code (an
                // uninitialised window would NRE in it) and drive the state machine / match
                // transition instead.
                var win = new UISearchingOpponent(UISearchingOpponent.Mode.MainMenu, MatchRule.CreateDefault());
                win.matchType = MatchType.Ranked;
                win.p1 = localPlayer;
                win.rematch = false;

                _listenerWindow = win;
                _queuedFromMatch = match;
                _searchActive = true;
                UpdateUi(LabelSearching, SearchState.Searching);

                // The game's native matchmaking: the manager does the polling and the server
                // connect itself; the results arrive at the (headless) window above.
                //
                // The interop's type graph has no managed subtype of the listener "interfaces"
                // (they are plain Il2CppObjectBase classes with an IntPtr wrapper ctor), so the
                // window can't be passed to FindMatch directly: wrap the same native object in
                // the listener wrapper types instead (the interop's own way of viewing a native
                // object through one of its interfaces - native dispatch reaches the window's
                // vtable either way).
                var findListener = new IFindMatchListener(win.Pointer);
                var connectListener = new IMatchConnectListener(win.Pointer);
                GameManager.Get.onlineServices.OnlineMatch.FindMatch(
                   localPlayer, MatchType.Ranked, false, null, null, MatchRule.CreateDefault(),
                   findListener, connectListener, null);

                // Only close the settings popup now that the search is actually running:
                // the user is back in the live training scene and can keep playing.
                _closePopup();

                // The settings popup (and with it this menu item) is closed by now, so the
                // console is the only place the search state is visible: say so explicitly.
                Plugin.Log.LogInfo(
                    "[RankedQueue] Searching for a ranked opponent - you can keep playing in the training match.");
            }
            catch (Exception e)
            {
                Plugin.Log.LogError("[RankedQueue] Failed to start the ranked queue: " + e);
                _searchActive = false;
                _queuedFromMatch = null;
                UpdateUi(LabelIdle, SearchState.Idle);
            }
        };

        GameManager.Get.CheckOnlineServiceAndProceed(true, serviceGate);
    }

    private static void CancelSearch()
    {
        if (!_searchActive)
        {
            return;
        }

        Plugin.Log.LogInfo("[RankedQueue] Cancelling the ranked search.");
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

        UpdateUi(LabelIdle, SearchState.Idle);
    }

    // ------------------------------------------------------------------
    // The game's callbacks, driven from the headless listener window
    // (wired via SearchOpponentHeadlessPatches - see IsHeadlessWindow).
    // ------------------------------------------------------------------

    private static bool StaleQueue()
    {
        if (_queuedFromMatch == null)
        {
            return true;
        }

        // Interop wrappers are fresh managed objects per property access and do NOT override
        // Equals, so identity must be compared through the native pointer (the same convention
        // this mod uses elsewhere, e.g. TrainingSettingsPopup's Pointer comparison).
        var current = MatchManager.Get.match;
        if (current == null || _queuedFromMatch.Pointer != current.Pointer)
        {
            // The player left the match we queued from - the search result is moot.
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
        UpdateUi(LabelIdle, SearchState.Idle);
    }

    /// <summary>
    /// A search result arrived (from the polling of the matchmaking server).
    /// </summary>
    internal static void HandleFindResult(MatchMakingResult result, HTTPResultCode code)
    {
        if (!_searchActive)
        {
            // Late callback after a cancel/finish - the original body must not run either
            // (the window has no UI components), so swallow it quietly.
            return;
        }

        if (StaleQueue())
        {
            return;
        }

        if (result == MatchMakingResult.FoundMatch)
        {
            // The manager now joins the match server itself and reports the outcome through
            // the connect listener (HandleConnectResult).
            Plugin.Log.LogInfo("[RankedQueue] Opponent found - connecting to the match server...");
            UpdateUi(LabelConnecting, SearchState.Connecting);
            return;
        }

        // TimedOut / Cancelled / NetworkError / InvalidJoinCode / Maintenance - no match to connect to.
        Plugin.Log.LogWarning($"[RankedQueue] Search ended without a match ({result}, {code}).");
        EndSearch();
        StartQueue();
    }

    /// <summary>
    /// The server join/connect finished (after an opponent was found).
    /// </summary>
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
            // The search is finished from here on: late callbacks on the listener window must
            // not trigger anything else.
            _searchActive = false;
            _listenerWindow = null;
            _queuedFromMatch = null;
            UpdateUi(LabelConnecting, SearchState.Connecting);

            // The game only starts a match from a non-match scene, so first run the game's
            // own match-end transition out of the training match; the match itself starts in
            // OnMainMenuLoaded, once the main-menu scene is active (see StartMatchTransition).
            Plugin.Log.LogInfo("[RankedQueue] Connected - ending the training match to start the ranked match...");
            StartMatchTransition(match);
            return;
        }

        // Failed to connect/join, or a late cancel after a match had been found.
        Plugin.Log.LogWarning($"[RankedQueue] Did not join the found match ({result}).");
        EndSearch();
    }

    // ------------------------------------------------------------------
    // Starting the found match - in two steps, mirroring the game's own
    // ranked flow: it always leaves whatever scene it is in first (the
    // match-end transition), lands in the main-menu scene, and only
    // then starts the new match from there.
    // ------------------------------------------------------------------

    private static void StartMatchTransition(PvPMatch foundMatch)
    {
        try
        {
            // Defensive: never leave a stale watchdog from a previous cycle running.
            StopWatchdog();

            // The match we queued from is over now (the game's own end-of-match marker;
            // the found-match data lives in foundMatch and is not affected by this).
            MatchManager.Get.CompleteMatch();

            // The same exit-time cleanup LevelManager.LeaveZone performs when a match ends
            // (it is the game's canonical "leave the match scene" routine).
            AudioManager.Get.StopAllSFX();
            LevelManager.Get.UnloadScene();
            UIManager.Get.SetLayerScreenSpaceOverlay(UILayerType.BackgroundOrthographic);
            UserPersistence.Get.TrackedStats.RefreshStatCache();
            UserPersistence.Get.TrackedStats.UpdateUnlockedAchievements();

            // Leave the match scene the way the game does (LeaveCombatScene: exit the old
            // screen, load the HubEmpty scene, then the main-menu scene). The game's
            // LeaveZone passes a NULL callback here because it never needs to know when the
            // menu is up - we pass ours: it fires when the main-menu scene becomes active.
            var menuScreen = new ScreenMainMenu(MainMenuSelection.None);
            System.Action<ILoadingContext> onMenuLoaded = _ => OnMainMenuLoaded(foundMatch);
            GameSceneManager.LeaveCombatScene(menuScreen, onMenuLoaded);

            // The transition rides on the game's own scene-loaded events; keep it moving if
            // they do not fire on their own.
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

        // The game's own PvP transition (UILobby.OnStartMatch) - now from a non-match scene,
        // the exact state the game's ranked button always transitions from.
        try
        {
            var startup = MatchManager.Get.CreateOnlinePvPMatch(foundMatch);
            new PvPGamePlay(startup).Start();
            MatchManager.ShowLoadingScreen(foundMatch);
        }
        catch (Exception e)
        {
            Plugin.Log.LogError("[RankedQueue] Failed to start the found match: " + e);
        }

        // The queue's work for this cycle is done (the match is starting, or the attempt
        // failed): return the selector to idle, or a later training session sees the item
        // stuck on "Enabled" and can never start a new search.
        _searchActive = false;
        _state = SearchState.Idle;
        _uiSpinner = null; // it lived in the training scene, which is unloaded by now
        Plugin.Log.LogInfo("[RankedQueue] Ranked match started - selector reset to idle.");
    }

    private static void StopWatchdog()
    {
        if (_watchdog != null)
        {
            UnityEngine.Object.Destroy(_watchdog);
            _watchdog = null;
        }
    }
}
