using System;
using System.Collections.Generic;
using GrimbaHack.UI.Pages;
using HarmonyLib;
using nway.gameplay.ui;

namespace GrimbaHack.Utility;

[HarmonyPatch(typeof(UITrainingOptions), nameof(UITrainingOptions.OnHide))]
public class OnUITrainingOptionsOnHideActionHandler
{
    private List<Action<UITrainingOptions>> _callbacks = new();

    public static OnUITrainingOptionsOnHideActionHandler Instance = new();

    private OnUITrainingOptionsOnHideActionHandler()
    {
    }

    public void AddPostfix(Action<UITrainingOptions> callback) => _callbacks.Add(callback);

    static void Postfix(UITrainingOptions __instance)
    {
        Plugin.Log.LogInfo("Running OnHide Postfix handler");
        foreach (var callback in Instance._callbacks)
        {
            callback(__instance);
        }
    }
}