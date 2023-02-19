using System.Collections.Generic;
using System.Linq;
using epoch.db;
using GrimbaHack.Data;
using GrimbaHack.Utility;
using HarmonyLib;
using nway.gameplay.match;
using nway.gameplay.online;
using nway.gameplay.ui;

namespace GrimbaHack.Modules;

public class CheatPrevention
{
    public static bool Enabled { get; set; } = false;

    // Cancel ranked/casual matchmaking
    private class PatchFindMatch
    {
        private PatchFindMatch()
        {
            OnOnlineMatchManagerFindMatchActionHandler.Instance.AddCallback((
                TeamHeroSelection team,
                epoch.db.MatchType matchType,
                bool rematch,
                string selectedStage,
                string joinCode,
                MatchRule matchRule,
                IFindMatchListener finshMatchListener,
                IMatchConnectListener matchConnectListener,
                IPremadeMatchMakingListener codeListener
            ) =>
            {
                if (Enabled && Global.IsBannedGameMode(matchType))
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
            });
        }
    }
}