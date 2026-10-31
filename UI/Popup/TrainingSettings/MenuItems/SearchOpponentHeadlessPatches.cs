using GrimbaHack.Modules.TrainingModeQueue;
using HarmonyLib;
using nway.gameplay.match;
using nway.gameplay.online;
using nway.gameplay.ui;
using nway.network;

namespace GrimbaHack.UI.Popup.TrainingSettings.MenuItems;

[HarmonyPatch(typeof(UISearchingOpponent), "nway_gameplay_online_IFindMatchListener_OnFindMatchResult")]
internal static class UISearchingOpponentFindResultPatch
{
    private static bool Prefix(UISearchingOpponent __instance, MatchMakingResult result, HTTPResultCode resultCode)
    {
        if (!TrainingModeQueue.IsHeadlessWindow(__instance))
        {
            return true;
        }

        TrainingModeQueue.HandleFindResult(result, resultCode);
        return false;
    }
}

[HarmonyPatch(typeof(UISearchingOpponent), "nway_gameplay_online_IMatchConnectListener_OnMatchConnectResult")]
internal static class UISearchingOpponentConnectResultPatch
{
    private static bool Prefix(UISearchingOpponent __instance, MatchConnectingResult result, PvPMatch match)
    {
        if (!TrainingModeQueue.IsHeadlessWindow(__instance))
        {
            return true;
        }

        TrainingModeQueue.HandleConnectResult(result, match);
        return false;
    }
}
