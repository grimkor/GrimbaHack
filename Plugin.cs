﻿using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using GrimbaHack.UI;
using HarmonyLib;
using UniverseLib.UI;
using UniverseLib.UI.Panels;

namespace GrimbaHack;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    internal static new ManualLogSource Log;
    public static UIBase UiBase { get; private set; }
    private static PanelBase _panel;

    public override void Load()
    {
        Log = base.Log;
        UISetup.Init(this);
        var harmony = new Harmony("Base.Grimbakor.Mod");
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }
}