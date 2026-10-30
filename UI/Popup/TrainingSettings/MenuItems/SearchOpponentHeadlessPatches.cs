using HarmonyLib;
using nway.gameplay.match;
using nway.gameplay.online;
using nway.gameplay.ui;
using nway.network;

namespace GrimbaHack.UI.Popup.TrainingSettings.MenuItems;

/// <summary>
/// The game's OnlineMatchManager hands matchmaking results to the two listener objects it was
/// given. The interop can only WRAP native objects (a managed class cannot implement the
/// interop's IFindMatchListener/IMatchConnectListener base classes - the Il2CppObjectBase
/// constructor requires a real native pointer), so the headless ranked queue passes the game's
/// own UISearchingOpponent window as those listeners - created but NEVER shown (its UI
/// components are uninitialised and its own code would NRE on them).
///
/// This prefix intercepts the window's find-result listener method:
///   * for the game's real windows (the normal ranked button) it returns true - the original
///     body runs exactly as it normally would;
///   * for the headless window (RankedQueueSelector._listenerWindow) it runs the mod's state
///     machine instead and returns false, so none of the window's UI code ever executes.
///
/// Registered by the assembly-wide Harmony PatchAll in Plugin.Load, like the rest of the mod's
/// patch classes.
/// </summary>
[HarmonyPatch(typeof(UISearchingOpponent), "nway_gameplay_online_IFindMatchListener_OnFindMatchResult")]
internal static class UISearchingOpponentFindResultPatch
{
    private static bool Prefix(UISearchingOpponent __instance, MatchMakingResult result, HTTPResultCode resultCode)
    {
        if (!RankedQueueSelector.IsHeadlessWindow(__instance))
        {
            return true;
        }

        RankedQueueSelector.HandleFindResult(result, resultCode);
        return false;
    }
}

/// <summary>
/// Same as <see cref="UISearchingOpponentFindResultPatch"/> but for the match-connect listener
/// method: on a real window the original body runs; on the headless window the found match is
/// started with the game's own PvP transition instead.
/// </summary>
[HarmonyPatch(typeof(UISearchingOpponent), "nway_gameplay_online_IMatchConnectListener_OnMatchConnectResult")]
internal static class UISearchingOpponentConnectResultPatch
{
    private static bool Prefix(UISearchingOpponent __instance, MatchConnectingResult result, PvPMatch match)
    {
        if (!RankedQueueSelector.IsHeadlessWindow(__instance))
        {
            return true;
        }

        RankedQueueSelector.HandleConnectResult(result, match);
        return false;
    }
}
