using System;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GrimbaHack.UI.Popup.TrainingSettings.MenuItems;

/// <summary>
/// A temporary DontDestroyOnLoad MonoBehaviour that keeps the ranked-match transition moving.
/// The transition (training match scene -> main-menu scene, then into the new match) rides on
/// the game's own Unity scene-loaded events (ScreenManager.OnSceneWasLoaded). If those events
/// fail to fire, the game would sit on an empty hub scene forever. After a few seconds the
/// watchdog re-delivers the same call a scene-loaded event would have made
/// (SceneManager.OnSceneWasLoaded) - each branch of the game's screen state machine guards
/// itself on its current state, so re-entering one is either a no-op or completes the stuck
/// transition. If two nudges do not help, it gives up and says so in the log.
/// </summary>
internal sealed class RankedQueueTransitionWatchdog : MonoBehaviour
{
    private static bool _typeRegistered;

    private int _ticks;
    private int _nudges;

    public static GameObject Start()
    {
        if (!_typeRegistered)
        {
            ClassInjector.RegisterTypeInIl2Cpp<RankedQueueTransitionWatchdog>();
            _typeRegistered = true;
        }

        var go = new GameObject("RankedQueueTransitionWatchdog");
        DontDestroyOnLoad(go);
        go.AddComponent<RankedQueueTransitionWatchdog>();
        return go;
    }

    private void Update()
    {
        ++_ticks;

        // First check five seconds in, then every ten; the normal transition finishes well
        // before that (the game's own match-end takes a couple of seconds).
        if (_ticks < 300 || _ticks % 600 != 0)
        {
            return;
        }

        if (_nudges >= 2)
        {
            Plugin.Log.LogError(
                "[RankedQueue] The match transition never completed (active scene: " +
                SceneManager.GetActiveScene().name +
                "). The game may need a restart to get back into a normal state.");
            Destroy(gameObject);
            return;
        }

        ++_nudges;
        var scene = SceneManager.GetActiveScene();
        Plugin.Log.LogWarning(
            "[RankedQueue] The scene transition is not completing on its own (active scene: " +
            scene.name + ") - nudging the game's scene state machine.");

        try
        {
            nway.gameplay.SceneManager.OnSceneWasLoaded(scene, LoadSceneMode.Single);
        }
        catch (Exception e)
        {
            Plugin.Log.LogWarning("[RankedQueue] The scene nudge failed: " + e);
        }
    }
}
