using System;
using System.Collections.Generic;
using HarmonyLib;
using nway.ui;

namespace GrimbaHack.Utility;

[HarmonyPatch(typeof(UIStackedMenu), nameof(UIStackedMenu.PopPage))]
public class OnUIStackedMenuPopPage
{
    public static OnUIStackedMenuPopPage Instance = new();

    private OnUIStackedMenuPopPage()
    {
    }

    private List<Action<UIStackedMenu, LayeredEventSystem>> _prefixCallbacks = new();
    private List<Action<UIStackedMenu, LayeredEventSystem>> _postfixCallbacks = new();

    public void AddPrefix(Action<UIStackedMenu, LayeredEventSystem> callback) => _prefixCallbacks.Add(callback);
    public void AddPostfix(Action<UIStackedMenu, LayeredEventSystem> callback) => _postfixCallbacks.Add(callback);

    static void Postfix(UIStackedMenu __instance, LayeredEventSystem eventSystem)
    {
        foreach (var callback in Instance._postfixCallbacks)
        {
            callback(__instance, eventSystem);
        }
    }

    static void Prefix(UIStackedMenu __instance, LayeredEventSystem eventSystem)
    {
        foreach (var callback in Instance._prefixCallbacks)
        {
            callback(__instance, eventSystem);
        }
    }
}