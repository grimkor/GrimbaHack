using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using GrimbaHack.Modules;
using GrimbaHack.UI;
using GrimbaHack.Utility;
using HarmonyLib;
using nway.gameplay;
using nway.gameplay.ui;
using UnityEngine;


namespace GrimbaHack;

public class ButtonSpriteMapping
{
    public Sprite light;
    public Sprite medium;
    public Sprite heavy;
    public Sprite special;
    public Sprite assist1;
    public Sprite assist2;
}

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
    public static ButtonSpriteMapping SpriteMapping = new();

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

        OnEnterTrainingMatchActionHandler.Instance.AddCallback(() =>
        {
            var obj = GameObject.FindObjectsOfType<UnityEngine.Sprite>();

            foreach (var sprite in obj)
            {
                if (sprite.name.Contains("button_pc_light"))
                {
                    SpriteMapping.light = sprite;
                }

                if (sprite.name.Contains("button_pc_medium"))
                {
                    SpriteMapping.medium = sprite;
                }

                if (sprite.name.Contains("button_pc_heavy"))
                {
                    SpriteMapping.heavy = sprite;
                }

                if (sprite.name.Contains("button_pc_special"))
                {
                    SpriteMapping.special = sprite;
                }

                if (sprite.name.Contains("button_pc_special"))
                {
                    SpriteMapping.special = sprite;
                }

                if (sprite.name.Contains("button_pc_assist_1"))
                {
                    SpriteMapping.assist1 = sprite;
                }

                if (sprite.name.Contains("button_pc_assist_2"))
                {
                    SpriteMapping.assist2 = sprite;
                }
            }
        });
    }
}

[HarmonyPatch(typeof(CommandHistoryItem), nameof(CommandHistoryItem.GetButtonImageFromButton))]
public class Test
{
    public static void Prefix(PlayerButton button, RemappedButton.ButtonImage __result)
    {
        Plugin.Log.LogInfo($"{button}, {__result}");
    }

    public static void Postfix(PlayerButton button, RemappedButton.ButtonImage __result)
    {
        Plugin.Log.LogInfo($"{button}, {__result}");
    }
}

[HarmonyPatch(typeof(CommandHistoryItem), nameof(CommandHistoryItem.UpdateActionButton))]
public class CheckButton
{
    public static void Postfix(
        RemappedButton button,
        PlayerButton mask,
        PlayerButton input,
        IController controller,
        ButtonMap buttonMap
    )
    {
        var obj = GameObject.FindObjectsOfType<UnityEngine.Sprite>();
        var sprite = obj.ToList().First(x =>
        {
            if ((mask & PlayerButton.Fire1) != 0)
            {
                button.image.overrideSprite = Plugin.SpriteMapping.light;
                button.image.sprite = Plugin.SpriteMapping.light;
                return x;
            }

            if ((mask & PlayerButton.Fire2) != 0)
            {
                button.image.overrideSprite = Plugin.SpriteMapping.medium;
                button.image.sprite = Plugin.SpriteMapping.medium;
                return x;
            }

            if ((mask & PlayerButton.Fire3) != 0)
            {
                button.image.overrideSprite = Plugin.SpriteMapping.heavy;
                button.image.sprite = Plugin.SpriteMapping.heavy;
                return x;
            }

            if ((mask & PlayerButton.Ability1) != 0)
            {
                button.image.overrideSprite = Plugin.SpriteMapping.special;
                button.image.sprite = Plugin.SpriteMapping.special;
                return x;
            }

            if ((mask & PlayerButton.Assist1) != 0)
            {
                button.image.overrideSprite = Plugin.SpriteMapping.assist1;
                button.image.sprite = Plugin.SpriteMapping.assist1;
                return x;
            }

            if ((mask & PlayerButton.Assist2) != 0)
            {
                button.image.overrideSprite = Plugin.SpriteMapping.assist2;
                button.image.sprite = Plugin.SpriteMapping.assist2;
                return x;
            }

            return false;
        });
    }
}