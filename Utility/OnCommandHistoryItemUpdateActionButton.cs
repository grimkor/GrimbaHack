using System;
using System.Collections.Generic;
using HarmonyLib;
using nway.gameplay;
using nway.gameplay.ui;

namespace GrimbaHack.Utility;

[HarmonyPatch(typeof(CommandHistoryItem), nameof(CommandHistoryItem.UpdateActionButton))]
public class OnCommandHistoryItemUpdateActionButton
{
    public static readonly OnCommandHistoryItemUpdateActionButton Instance = new();

    private readonly List<Action<RemappedButton,
        PlayerButton,
        PlayerButton,
        IController,
        ButtonMap>> _prefixCallbacks = new();

    private readonly List<Action<RemappedButton,
        PlayerButton,
        PlayerButton,
        IController,
        ButtonMap>> _postfixCallbacks = new();

    private OnCommandHistoryItemUpdateActionButton()
    {
    }

    static OnCommandHistoryItemUpdateActionButton()
    {
    }

    public void AddPrefix(Action<RemappedButton,
        PlayerButton,
        PlayerButton,
        IController,
        ButtonMap> callback) => _prefixCallbacks.Add(callback);

    public void AddPostfix(Action<
        RemappedButton,
        PlayerButton,
        PlayerButton,
        IController,
        ButtonMap> callback) => _postfixCallbacks.Add(callback);

    public static void Postfix(
        RemappedButton button,
        PlayerButton mask,
        PlayerButton input,
        IController controller,
        ButtonMap buttonMap)
    {
        foreach (var callback in Instance._postfixCallbacks)
        {
            callback(button, mask, input, controller, buttonMap);
        }
    }
    public static void Prefix(
        RemappedButton button,
        PlayerButton mask,
        PlayerButton input,
        IController controller,
        ButtonMap buttonMap)
    {
        foreach (var callback in Instance._prefixCallbacks)
        {
            callback(button, mask, input, controller, buttonMap);
        }
    }
}