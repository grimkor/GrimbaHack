using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace GrimbaHack;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    private static ManualLogSource _log;

    public override void Load()
    {
        _log = Log;
        Log.LogInfo("Plugin Loaded");
        var harmony = new Harmony("Base.Grimbakor.Mod");
        harmony.PatchAll(Assembly.GetExecutingAssembly());
        AddComponent<CameraControl>();
        var exploration = AddComponent<FrameDataModal>();
        var dummy = AddComponent<TrainingModeDummy>();
        // exploration.Setup(Log);
        dummy.Setup(Log);
        // AddComponent<Online>();
    }
}
