using System;
using System.Collections.Generic;
using HarmonyLib;

namespace GrimbaHack.Utility;

[HarmonyPatch(typeof(Character), nameof(Character.ApplyBlockAndHitStun))]
public class OnApplyBlockAndHitStunActionHandler
{
    private OnApplyBlockAndHitStunActionHandler()
    {
    }

    static OnApplyBlockAndHitStunActionHandler()
    {
        Instance = new OnApplyBlockAndHitStunActionHandler();
    }

    public static OnApplyBlockAndHitStunActionHandler Instance { get; private set; }
    private List<Action<bool, int>> callbacks = new();

    public void AddCallback(Action<bool, int> callback)
    {
        Instance.callbacks.Add(callback);
    }

    public static void Prefix(bool isHitStun, int originalFrames)
    {
        foreach (var callback in Instance.callbacks)
        {
            callback(isHitStun, originalFrames);
        }
    }
}