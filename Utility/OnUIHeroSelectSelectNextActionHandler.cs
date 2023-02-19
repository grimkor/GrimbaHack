using System;
using System.Collections.Generic;
using HarmonyLib;
using nway.gameplay.simulation;
using nway.gameplay.ui;

namespace GrimbaHack.Utility;

[HarmonyPatch(typeof(UIHeroSelect), nameof(UIHeroSelect.SelectNext))]
public class OnUIHeroSelectSelectNextActionHandler
{
    private OnUIHeroSelectSelectNextActionHandler()
    {
    }

    static OnUIHeroSelectSelectNextActionHandler()
    {
        Instance = new OnUIHeroSelectSelectNextActionHandler();
    }

    public static OnUIHeroSelectSelectNextActionHandler Instance { get; private set; }
    private List<Action<UIHeroSelect.UIHeroCard, UIHeroSelect.Team>> callbacks = new();

    public void AddCallback(Action<UIHeroSelect.UIHeroCard, UIHeroSelect.Team> callback)
    {
        Instance.callbacks.Add(callback);
    }

    public static void Postfix(ref UIHeroSelect.UIHeroCard heroCard, UIHeroSelect.Team team)
    {
        foreach (var callback in Instance.callbacks)
        {
            callback(heroCard, team);
        }
    }
}