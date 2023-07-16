using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using GrimbaHack.UI;
using HarmonyLib;

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
    public override void Load()
    {
        TwitchClientID = Config.Bind("Twitch", "ClientId", "", "Client ID for Twitch API");
        TwitchAccessToken = Config.Bind("Twitch", "AccessToken", "", "Access Token for Twitch");
        TwitchUsername = Config.Bind("Twitch", "TwitchUsername", "", "Twitch username");
        TwitchBroadcasterID = Config.Bind("Twitch", "TwitchBroadcasterID", "", "Broadcaster ID for twitch channel");
        TwitchPredictionTitle = Config.Bind("Twitch", "TwitchPredictionTitle", "Who wins?", "Title for the Prediction");
        TwitchPredictionMessage = Config.Bind("Twitch", "TwitchPredictionMessage", "{P1} vs {P2}, who wins? Bet now!", "Message the chat bot will say when a prediction starts, empty message will not send a message");
        
        Log = base.Log;
        UISetup.Init(this);
        var harmony = new Harmony("Base.Grimbakor.Mod");
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }
}