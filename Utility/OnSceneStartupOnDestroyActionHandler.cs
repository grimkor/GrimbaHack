using System;
using System.Collections.Generic;
using HarmonyLib;

namespace GrimbaHack.Utility;

[HarmonyPatch(typeof(SceneStartup), nameof(SceneStartup.OnDestroy))]
public class OnSceneStartupOnDestroyActionHandler
{
    private OnSceneStartupOnDestroyActionHandler()
    {
    }

    static OnSceneStartupOnDestroyActionHandler()
    {
        Instance = new OnSceneStartupOnDestroyActionHandler();
    }
    public static OnSceneStartupOnDestroyActionHandler Instance { get; set; }
    private List<Action> callbacks = new();

    public void AddCallback(Action callback)
    {
        Instance.callbacks.Add(callback);
    }
    
    public static void Prefix()
    {
        foreach (var callback in Instance.callbacks)
        {
            callback();
        }
    }
}