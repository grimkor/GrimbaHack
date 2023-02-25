using System;
using System.Collections.Generic;
using HarmonyLib;
using nway.gameplay;
using nway.gameplay.match;
using nway.gameplay.online;

namespace GrimbaHack.Utility;

[HarmonyPatch(typeof(OnlineMatchManager), nameof(OnlineMatchManager.FindMatch))]
public class OnOnlineMatchManagerFindMatchActionHandler
{
    private OnOnlineMatchManagerFindMatchActionHandler()
    {
    }

    static OnOnlineMatchManagerFindMatchActionHandler()
    {
        Instance = new OnOnlineMatchManagerFindMatchActionHandler();
    }

    public static OnOnlineMatchManagerFindMatchActionHandler Instance { get; set; }

    private List<Func<
        TeamHeroSelection,
        epoch.db.MatchType,
        bool,
        string,
        string,
        MatchRule,
        IFindMatchListener,
        IMatchConnectListener,
        IPremadeMatchMakingListener,
        bool
    >> callbacks = new();

    public void AddCallback(Func<
        TeamHeroSelection,
        epoch.db.MatchType,
        bool,
        string,
        string,
        MatchRule,
        IFindMatchListener,
        IMatchConnectListener,
        IPremadeMatchMakingListener,
        bool
    > callback)
    {
        Instance.callbacks.Add(callback);
    }

    public static bool Prefix(
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
        var result = true;
        foreach (var callback in Instance.callbacks)
        {
            if (!result) break;
            result = callback(
                team,
                matchType,
                rematch,
                selectedStage,
                joinCode,
                matchRule,
                finshMatchListener,
                matchConnectListener,
                codeListener
            );
        }

        return result;
    }
}