using System;
using System.Collections.Generic;
using HarmonyLib;
using nway.gameplay.match;

namespace GrimbaHack.Utility;

[HarmonyPatch(typeof(Character), nameof(Character.GetOffenseInfo))]
public class OnCharacterGetOffenseInfoActionHandler
{
    private OnCharacterGetOffenseInfoActionHandler()
    {
    }

    static OnCharacterGetOffenseInfoActionHandler()
    {
        Instance = new OnCharacterGetOffenseInfoActionHandler();
    }
    public static OnCharacterGetOffenseInfoActionHandler Instance { get; set; }
    private List<Action<OffenseInfo>> callbacks = new();

    public void AddCallback(Action<OffenseInfo> callback)
    {
        Instance.callbacks.Add(callback);
    }
    
    public static void Postfix(OffenseInfo __result)
    {
        foreach (var callback in Instance.callbacks)
        {
            callback(__result);
        }
    }
}