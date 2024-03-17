using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using GrimbaHack.Data;
using GrimbaHack.Modules;
using GrimbaHack.Modules.PlayerInput;
using GrimbaHack.UI;
using GrimbaHack.UI.Elements;
using GrimbaHack.Utility;
using GrimbaHack.UI.Managers;
using GrimbaHack.UI.MenuItems;
using GrimbaHack.Utility;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.Injection;
using nway.ui;
using UnityEngine;
using UnityEngine.Events;


namespace GrimbaHack;

public class SomeWindow : UIWindow
{
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

        GrimUITrainingModeController.Instance.Init();
        GrimUIMainSettingsController.Instance.Init();
        CommandHistoryFix.Init();
        ClassInjector.RegisterTypeInIl2Cpp<PlayerInputBehaviour>();
        ClassInjector.RegisterTypeInIl2Cpp<UnityMainThreadDispatcher>();
        AddComponent<UnityMainThreadDispatcher>();
        EnumInjector.RegisterEnumInIl2Cpp<DefaultMenuOptions>();
        EnumInjector.RegisterEnumInIl2Cpp<StageSelectOverrideMenuOptions>();
    }
}
//
// [HarmonyPatch(typeof(UIPopupManager), nameof(UIPopupManager.ShowWindow))]
// public class Test
// {
//     private static void TestMethod()
//     {
//         Plugin.Log.LogInfo("HAHA");
//     }
//
//     public static void Postfix(UIPopupManager __instance, UIWindow uiWindow)
//     {
//         Plugin.Log.LogInfo("### UIPopupManager.ShowWindow() ###");
//         Plugin.Log.LogInfo($"uIWindow.GetUIPath() {uiWindow.GetUIPath()}");
//         // if (uiWindow.GetUIPath() != "UI_Settings")
//         // {
//         //     __instance.AddModalUIWindow(uiWindow);
//         // }
//     }
// }

// Nothing so far
// [HarmonyPatch(typeof(UIPopupManager), nameof(UIPopupManager.AddModalUIWindow))]
// class AddUIWindow
// {
//     public static void Postfix(UIWindow uiWindow)
//     {
//         Plugin.Log.LogInfo("### UIPopupManager.AddModalUIWindow() ###");
//         Plugin.Log.LogInfo($"GetUIPath() {uiWindow.GetUIPath()}");
//     }
// }

// [HarmonyPatch(typeof(UIWindow), nameof(UIWindow.InitializeComponents))]
// class InitializeComponents
// {
//     public static void Postfix(UIPopup uiPopup,
//         LayeredEventSystem eventSystem,
//         ConsoleInputModule inputModule)
//     {
//         Plugin.Log.LogInfo($"### UIWindow.InitializeComponents ###");
//         Plugin.Log.LogInfo(uiPopup.name);
//         Plugin.Log.LogInfo(eventSystem.name);
//         Plugin.Log.LogInfo(inputModule.name);
//         Plugin.Log.LogInfo(inputModule.input.name);
//     }
// }

[HarmonyPatch(typeof(UIWindow), nameof(UIWindow.Show))]
class UIWindowShow

{
    private static bool pressed;

    public static void ActionFunc(ILayeredEventData eventData)
    {
        // if (UIExtendedWindow.IsVisible())
        // {
        //     return;
        // }
        UnityMainThreadDispatcher.Instance.Enqueue(() =>
        {
            var go = new GameObject("SOME_TEST")
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            GameObject.DontDestroyOnLoad(go);
            var window = new SomeWindow
            {
                transform = go.transform,
                autoInputLayer = true
            };
            var uiWindow = new UIWindow();
            uiWindow.ShowModalWindow();
            Plugin.Log.LogInfo("I DID IT?");
        });

        pressed = true;

        // var i = 0;
        // Plugin.Log.LogInfo($"creating window");
        // var go = new GameObject("SOME_TEST");
        // Plugin.Log.LogInfo($"creating window {i++}");
        // go.hideFlags = HideFlags.HideAndDontSave;
        // Plugin.Log.LogInfo($"creating window {i++}");
        // Object.DontDestroyOnLoad(go);
        // Plugin.Log.LogInfo($"creating window: {i++}");
        // var window = new SomeWindow();
        // Plugin.Log.LogInfo($"creating window {i++}");
        // window.transform = go.transform;
        // Plugin.Log.LogInfo($"creating window {i++}");
        // window.autoInputLayer = true;
        // Plugin.Log.LogInfo($"creating window {i++}");
        // var popup = UIAssetManager.instance.InstantiateUIPopup(go, "SOME_TEST");
        // Plugin.Log.LogInfo($"creating window {i++}");
        // // BaseUIManager.instance.PopupManager.InitLoadedPopup(popup, "SOME_TEST");
        // Plugin.Log.LogInfo($"creating window {i++}");
        // // BaseUIManager.instance.PopupManager.ShowWindow(window).Show();
        //
        // // MessageBoxSample.DoTheThing();
        //
        // Plugin.Log.LogInfo("IM DOING IT");
        // // var ffs = new UIExtendedWindow(MessageBoxType.TwoButtons);
        // // ffs.DoTheThing();
        // // var sure = new UISettings();
        // // sure.ShowModalWindow();
        // var test = new UIExtendedWindow();
        // test.ShowModalWindow();
        // // UIExtendedWindow.DoTheThing(eventData);
        // Plugin.Log.LogInfo("I DID IT?");


        // var x = new UISettings();
        // x.Show();
        // Plugin.Log.LogInfo(eventData.Layer.LayerName);
        // Plugin.Log.LogInfo(eventData.Input.displayName);
        // var go = GameObject.FindObjectsOfType<LayeredSelectable>();
        // var button = go.ToList().Find(x => x.name.ToLower() == "mini");
        // if (button)
        // {
        // eventData.SetSelection(eventData.Layer, button);
        // }
        // var go = new GameObject("ZZZ_NAMED");
        // var x = new UIPage(go.transform);
        // var selectable = go.AddComponent<LayeredSelectable>();
        // Plugin.Log.LogInfo(selectable);
        // selectable.name = "TEST SELECTABLE";
        // Plugin.Log.LogInfo($"{selectable.name}");
        // Plugin.Log.LogInfo($"{selectable.navigation}");
        // Plugin.Log.LogInfo($"{selectable.enabled}");
        // x.selectables.Add(selectable);
    }
// }

    private static bool callbackAdded;

    public static void Postfix(UIWindow __instance, bool skipInitialize)
    {
        // var go = GameObject.FindObjectsOfType<LayeredSelectable>();
        // var mini = go.ToList().Find(x => x.name.ToLower() == "mini");
        // var sound = go.ToList().Find(x => x.name.ToLower() == "soundsettingsbutton");
        // foreach (var selectable in go.ToList())
        // {
            // Plugin.Log.LogInfo(selectable.OnMoveOverrides);
            // Plugin.Log.LogInfo(selectable.name);
            // Plugin.Log.LogInfo(selectable.navigation.selectOnUp.name);
            // Plugin.Log.LogInfo(selectable.navigation.selectOnDown.name);
            // selectable.navigation.selectOnLeft = mini;
            // selectable.navigation.selectOnRight = sound;
            // selectable.OnMoveOverrides.Clear();
            // selectable.navigation.mode = Navigation.Mode.Explicit;
        // }

        // MenuButton.Submit = Selecting options in a pop-up menu
        // if (!callbackAdded)
        // {
            __instance.AddButtonCallback(MenuButton.Left, (UnityAction<ILayeredEventData>)ActionFunc);
            // callbackAdded = true;
        // }
        // __instance.AddButtonCallback(MenuButton.Right, (Action<ILayeredEventData>)(data =>
        // {
        //     EventManager.instance.SendGameEvent(GameEventType.Megazord_Activate_Team0);
        // }));
    }
}

// [HarmonyPatch(typeof(UIPopupManager), nameof(UIPopupManager.CheckActiveModalWindow))]
// class Seomthing
// {
//     public static void Postfix(UISettings __instance, string uiPopupName)
//     {
//         Plugin.Log.LogInfo("### UIPopupManager.CheckActiveModalWindow() ###");
//         Plugin.Log.LogInfo(uiPopupName);
//     }
// }
//
// [HarmonyPatch(typeof(UIWindow), "EnableAutoInputLayer", new[] { typeof(EventSystemLayer), typeof(IControllerGroup[]) })]
// class Hide
// {
//     public static void Prefix(EventSystemLayer specificLayer,
//         params IControllerGroup[] specificControllers)
//     {
//         Plugin.Log.LogInfo("### UIWindow.EnableAuoInputLayer() ###");
//     }
// }
//
// [HarmonyPatch(typeof(UIWindow), "EnableAutoInputLayer", new[] { typeof(EventSystemLayer), typeof(Il2CppReferenceArray<IControllerGroup> ) })]
// class HideTwo
// {
//     public static void Prefix(EventSystemLayer specificLayer,
//         [Optional] Il2CppReferenceArray<IControllerGroup> specificControllers)
//     {
//         Plugin.Log.LogInfo("### UIWindow.EnableAuoInputLayer() ###");
//     }
// }