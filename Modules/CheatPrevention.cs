using System.Collections.Generic;
using System.Linq;
using epoch.db;
using HarmonyLib;
using nway.gameplay.match;
using nway.gameplay.online;
using nway.gameplay.ui;

namespace GrimbaHack.Modules;

public class CheatPrevention
{
    public static bool Enabled { get; set; } = false;

    private static List<MatchType> bannedModes = new List<MatchType>
        { MatchType.RANKED, MatchType.CASUAL, MatchType.LOBBY };

    // Cancel ranked/casual matchmaking
    [HarmonyPatch(typeof(OnlineMatchManager), nameof(OnlineMatchManager.FindMatch))]
    private class PatchFindMatch
    {
        private static bool Prefix(
            TeamHeroSelection team,
            epoch.db.MatchType matchType,
            bool rematch,
            string selectedStage,
            string joinCode,
            MatchRule matchRule,
            IFindMatchListener finshMatchListener,
            IMatchConnectListener matchConnectListener,
            IPremadeMatchMakingListener codeListener
        )
        {
            if (Enabled && bannedModes.Contains(matchType))
            {
                var hasDuplicatesinReferenceArray = team.heroes.GroupBy(x => x).Any(g => g.Count() > 1);
                // If the player has duplicates in their hero selection prevent the match from starting
                if (hasDuplicatesinReferenceArray)
                {
                    // Allow passworded lobbies
                    if (matchType == MatchType.LOBBY &&
                        GameManager.instance.onlineServices.Lobby.CurrentLobby.HasPassword)
                    {
                        return true;
                    }
                    GameManager.instance.onlineServices.OnlineMatch.CancelFindingMatch(true);
                    return false;
                }
            }
            return true;
        }
    }
}