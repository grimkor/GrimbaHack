using System;
using System.Collections.Generic;
using HarmonyLib;
using nway.gameplay.ui;

namespace GrimbaHack.Utility;

[HarmonyPatch(typeof(UISettings), nameof(UISettings.OnShow))]
public class OnUISettingsOnShowActionHandler
{
    public static readonly OnUISettingsOnShowActionHandler Instance = new();
    private readonly List<Action<UISettings>> _postfixCalls = new();
    private readonly List<Action<UISettings>> _prefixCalls = new();

    private OnUISettingsOnShowActionHandler()
    {
    }

    public void AddPostfix(Action<UISettings> callback) => _postfixCalls.Add(callback);
    public void AddPrefix(Action<UISettings> callback) => _prefixCalls.Add(callback);

    static void Prefix(UISettings __instance)
    {
        foreach (var callback in Instance._prefixCalls)
        {
            callback(__instance);
        }
    }

    static void Postfix(UISettings __instance)
    {
        foreach (var callback in Instance._postfixCalls)
        {
            callback(__instance);
        }
    }
}