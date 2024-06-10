using System;
using System.Collections.Generic;
using HarmonyLib;
using nway.ui;

namespace GrimbaHack.Utility;

[HarmonyPatch(typeof(UIStackedMenu), nameof(UIStackedMenu.PushPage))]
public class OnUIStackedMenuPushPage
{
    public static OnUIStackedMenuPushPage Instance = new();

    private OnUIStackedMenuPushPage()
    {
    }

    private List<Action<UIPage, EventSystemLayer, LayeredSelectable>> _prefixCallbacks = new();
    private List<Action<UIPage, EventSystemLayer, LayeredSelectable>> _postfixCallbacks = new();

    public void AddPrefix(Action<UIPage, EventSystemLayer, LayeredSelectable> callback) =>
        _prefixCallbacks.Add(callback);

    public void AddPostfix(Action<UIPage, EventSystemLayer, LayeredSelectable> callback) =>
        _postfixCallbacks.Add(callback);

    static void Postfix(
        UIPage page,
        EventSystemLayer lastLayer,
        LayeredSelectable lastSelection
    )
    {
        foreach (var callback in Instance._postfixCallbacks)
        {
            callback(page, lastLayer, lastSelection);
        }
    }

    static void Prefix(
        UIPage page,
        EventSystemLayer lastLayer,
        LayeredSelectable lastSelection
    )
    {
        foreach (var callback in Instance._prefixCallbacks)
        {
            callback(page, lastLayer, lastSelection);
        }
    }
}