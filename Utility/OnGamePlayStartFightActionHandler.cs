using System.Collections.Generic;
using HarmonyLib;
using nway.gameplay;
using Action = System.Action;

namespace GrimbaHack.Utility;

[HarmonyPatch(typeof(GamePlay), nameof(GamePlay.StartFight))]
public class OnGamePlayStartFightActionHandler
{
    private OnGamePlayStartFightActionHandler()
    {
    }

    static OnGamePlayStartFightActionHandler()
    {
        Instance = new OnGamePlayStartFightActionHandler();
    }

    public static OnGamePlayStartFightActionHandler Instance { get; set; }
    private List<Action> callbacks = new();

    public void AddCallback(Action callback)
    {
        Instance.callbacks.Add(callback);
    }

    public static void Postfix()
    {
        foreach (var callback in Instance.callbacks)
        {
            callback();
        }
    }
}