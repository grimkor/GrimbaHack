using System;
using System.Collections.Generic;
using HarmonyLib;
using nway.gameplay.ui;

namespace GrimbaHack.Utility;

[HarmonyPatch(typeof(UITrainingOptions), nameof(UITrainingOptions.OnShow))]
public class OnUITrainingOptionsOnShowActionHandler
{
    public static readonly OnUITrainingOptionsOnShowActionHandler Instance = new();
    private readonly List<Action<UITrainingOptions>> _callbacks = new();

    private OnUITrainingOptionsOnShowActionHandler()
    {
    }


    public void AddPostfix(Action<UITrainingOptions> callback) => _callbacks.Add(callback);

    static void Postfix(UITrainingOptions __instance)
    {
        foreach (var callback in Instance._callbacks)
        {
            callback(__instance);
        }
    }
}
