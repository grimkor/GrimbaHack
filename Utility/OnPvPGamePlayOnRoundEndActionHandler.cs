using System;
using System.Collections.Generic;
using ArenaProtocol;
using HarmonyLib;
using nway.gameplay;
using nway.gameplay.match;
using nway.gameplay.online;
using nway.network.connection;

namespace GrimbaHack.Utility;

[HarmonyPatch(typeof(PvPGamePlay), nameof(PvPGamePlay.OnRoundEnd))]
public class OnPvPGamePlayRoundEnd
{
    private OnPvPGamePlayRoundEnd()
    {
    }

    static OnPvPGamePlayRoundEnd()
    {
        Instance = new OnPvPGamePlayRoundEnd();
    }
    public static OnPvPGamePlayRoundEnd Instance { get; set; }
    private readonly List<Action<bool, RoundResult>> _callbacks = new();

    public void AddCallback(Action<bool, RoundResult> callback)
    {
        Instance._callbacks.Add(callback);
    }
    
    public static void Prefix(bool isMatchFinished, RoundResult roundResult)
    {
        foreach (var callback in Instance._callbacks)
        {
            callback(isMatchFinished, roundResult);
        }
    }
}