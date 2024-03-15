using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using GrimbaHack.Data;
using GrimbaHack.Modules;
using GrimbaHack.UI;
using GrimbaHack.UI.Elements;
using GrimbaHack.Utility;
using GrimbaHack.UI.Managers;
using GrimbaHack.UI.MenuItems;
using GrimbaHack.UI.Pages;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;

namespace GrimbaHack;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    internal new static ManualLogSource Log;

    internal static ConfigEntry<string> TwitchClientID;
    internal static ConfigEntry<string> TwitchAccessToken;
    internal static ConfigEntry<string> TwitchUsername;
    internal static ConfigEntry<string> TwitchBroadcasterID;
    internal static ConfigEntry<string> TwitchPredictionTitle;
    internal static ConfigEntry<string> TwitchPredictionMessage;
    internal static ConfigEntry<bool> TwitchPredictionTournamentMode;
    internal static ConfigEntry<int> TwitchPredictionWinsRequired;
    internal static ConfigEntry<bool> EXPERIMENTAL_MemoryFix;
    internal static ConfigEntry<bool> EXPERIMENTAL_TextureLoader;

    public override void Load()
    {
        TwitchClientID = Config.Bind("Twitch", "ClientId", "", "Client ID for Twitch API");
        TwitchAccessToken = Config.Bind("Twitch", "AccessToken", "", "Access Token for Twitch");
        TwitchUsername = Config.Bind("Twitch", "TwitchUsername", "", "Twitch username");
        TwitchBroadcasterID = Config.Bind("Twitch", "TwitchBroadcasterID", "", "Broadcaster ID for twitch channel");
        TwitchPredictionTitle = Config.Bind("Twitch", "TwitchPredictionTitle", "Who wins?", "Title for the Prediction");
        TwitchPredictionMessage = Config.Bind("Twitch", "TwitchPredictionMessage", "{P1} vs {P2}, who wins? Bet now!",
            "Message the chat bot will say when a prediction starts, empty message will not send a message");
        TwitchPredictionTournamentMode = Config.Bind("Twitch", "TwitchPredictionTournamentMode", false,
            "Tournament mode changes to a FT format expecting 1 round matches instead of multi-round matches.");
        TwitchPredictionWinsRequired = Config.Bind("Twitch", "TwitchPredictionWinsRequired", 3,
            "Wins required when in tournament mode");
        EXPERIMENTAL_MemoryFix = Config.Bind("EXPERIMENTAL", "EXPERIMENTAL_MemoryFix", false,
            "Enable experimental memory leak fix");
        EXPERIMENTAL_TextureLoader = Config.Bind("EXPERIMENTAL", "EXPERIMENTAL_TextureLoader", false,
            "Enable experimental texture loader");

        Log = base.Log;
        UISetup.Init(this);
        var harmony = new Harmony("Base.Grimbakor.Mod");
        harmony.PatchAll(Assembly.GetExecutingAssembly());
        TextureLoader.Instance.CreateFolders();
        EnumInjector.RegisterEnumInIl2Cpp<DefaultMenuOptions>();
        EnumInjector.RegisterEnumInIl2Cpp<TrainingModeSelectables>();
        EnumInjector.RegisterEnumInIl2Cpp<StageSelectOverrideOptions>();
        ClassInjector.RegisterTypeInIl2Cpp<BetterRangeSelector>();
        ClassInjector.RegisterTypeInIl2Cpp<GrimUIPage>();
        ClassInjector.RegisterTypeInIl2Cpp<GrimUITrainingModeSettings>();
        ClassInjector.RegisterTypeInIl2Cpp<GrimUIMainSettings>();
        ClassInjector.RegisterTypeInIl2Cpp<RandomStageSelectPage>();

        GrimUITrainingModeController.Instance.Init();
        GrimUIMainSettingsController.Instance.Init();
        CommandHistoryFix.Init();
    }
}
