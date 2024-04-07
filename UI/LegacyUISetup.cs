using System.IO;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using UnityEngine;
using UniverseLib;

namespace GrimbaHack.UI;

public static class LegacyUISetup
{
    public static BasePlugin Plugin { get; private set; }

    public static void Init(BasePlugin basePlugin)
    {
        Plugin = basePlugin;
        Universe.Init(1f, DelayInit, LogHandler, new()
        {
            Disable_EventSystem_Override = false,
            Force_Unlock_Mouse = true,
            Unhollowed_Modules_Folder = Path.Combine(Paths.BepInExRootPath, "interop")
        });
        UIBehaviour.Setup();
    }

    static void LogHandler(string message, LogType logType)
    {
        switch (logType)
        {
            case LogType.Log:
                Plugin.Log.LogInfo(message);
                break;
            case LogType.Warning:
                Plugin.Log.LogWarning(message);
                break;
            case LogType.Error:
                Plugin.Log.LogError(message);
                break;
            case LogType.Exception:
                Plugin.Log.LogFatal(message);
                break;
            default:
                Plugin.Log.LogInfo(message);
                break;
        }
    }

    static void DelayInit()
    {
        LegacyUIManager.Init();
    }
}
