using System;
using GrimbaHack.Modules.TrainingModeQueue;
using HarmonyLib;
using nway.gameplay;
using nway.gameplay.match;
using nway.gameplay.ui;
using MatchType = epoch.db.MatchType;

namespace GrimbaHack.UI.Popup.TrainingSettings.MenuItems;

[HarmonyPatch(typeof(OnlinePvPMatchEndControl), nameof(OnlinePvPMatchEndControl.OnSelectSearchNewMatch))]
internal static class FindMatchReturnToTrainingPatch
{
    private static bool Prefix(OnlinePvPMatchEndControl __instance)
    {
        if (__instance == null)
        {
            return true;
        }

        if (!TrainingModeQueue.TryConsumeReturnToTraining(out TeamHeroSelection p1, out TeamHeroSelection p2, out string arena))
        {
            return true;
        }

        Plugin.Log.LogInfo("[RankedQueue] Find Match pressed on the queue-launched ranked match - returning to training instead.");

        TrainingModeQueue.Reset();

        var menu = __instance.menu;
        if (menu == null)
        {
            Plugin.Log.LogWarning("[RankedQueue] Post-match menu unavailable - falling back to the original Find Match.");
            return true;
        }

        if (p1 != null && p2 != null)
        {
            try
            {
                var match = new TrainingMatch();
                match.arenaId = string.IsNullOrEmpty(arena) ? "Arena_Training_Pit" : arena;
                match.Initialize(p1, p2, PlayerControllerMapping.CreateForSinglePlayer());
                var loadingScreen = MatchManager.ShowLoadingScreen(match);
                var startup = MatchManager.Get.SetupGamePlay(match, MatchManager.Get.GetPID(),
                    PlayerControllerMapping.CreateForTwoPlayerTraining());

                Plugin.Log.LogInfo("[RankedQueue] Starting the training match directly with the captured teams...");
                menu.Leave(new ScreenPlayOnlineMatch(startup),
                    loadingScreen != null ? new ILoadingContext(loadingScreen.Pointer) : null);
                Plugin.Log.LogInfo("[RankedQueue] Returned to a training match with the same teams and arena.");
                return false;
            }
            catch (Exception e)
            {
                Plugin.Log.LogError("[RankedQueue] Failed to start the training match directly - falling back to the training hero selection: " + e);
            }
        }
        else
        {
            Plugin.Log.LogWarning("[RankedQueue] No usable captured teams (p1/p2 missing) - using the training hero selection instead.");
        }

        try
        {
            var heroSelection = new ScreenHeroSelection
            {
                hubArgs = new ScreenHeroSelection.HeroSelectionArgs
                {
                    matchType = MatchType.Training,
                    rematch = false
                }
            };
            menu.Leave(heroSelection, null);
            Plugin.Log.LogInfo("[RankedQueue] Returned to the training hero selection screen.");
        }
        catch (Exception e)
        {
            Plugin.Log.LogError("[RankedQueue] Failed to return to training in any way: " + e);
        }

        return false;
    }
}
