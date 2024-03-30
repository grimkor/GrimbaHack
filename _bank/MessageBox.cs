using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using nway.gameplay.online;
using nway.gameplay.ui;
using nway.ui;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

namespace GrimbaHack._bank;

public class MessageBoxSample
{
    private static MessageBox mb = new(MessageBoxType.TwoButtons);

    public static void DoTheThing()
    {
        Plugin.Log.LogInfo("Triggered the Button Action Callback");
        mb.message = "Rita is downplayed.";
        mb.button1Text = "Yes";
        mb.button2Text = "Mid at best";
        mb.header = "Pop Quiz";
        mb.button1Callback = (MessageBox.Callback)(() =>
        {
            Plugin.Log.LogInfo("Button1Callback");
            mb.Hide();
        });
        mb.button2Callback = (MessageBox.Callback)(() =>
        {
            Plugin.Log.LogInfo("Button2Callback");
            mb.Hide();
        });
        mb.onShow = (Action)(() => { Plugin.Log.LogInfo("Showing!"); });
        mb.onCancel = (UnityAction<ILayeredEventData>)((ILayeredEventData i) => { Plugin.Log.LogInfo("Closing"); });
        mb.onNavigate = (UnityAction<ILayeredEventData>)((ILayeredEventData i) => { Plugin.Log.LogInfo("Test"); });
        mb.onPause = (UnityAction<ILayeredEventData>)((ILayeredEventData i) => { Plugin.Log.LogInfo("Test"); });
        mb.onSubmit = (UnityAction<ILayeredEventData>)((ILayeredEventData i) => { Plugin.Log.LogInfo("Submitting"); });
        foreach (var buttonEvent in mb.buttonEvents)
        {
            Plugin.Log.LogInfo($"{buttonEvent.key}: {buttonEvent.value}");
        }
        // BaseUIManager.instance.PopupManager.ShowWindow(mb).Show();
    }

    public static bool IsVisible()
    {
        return mb.uiPopup.IsActive();
    }
}

public class UIExtendedWindow : UISettings
{

    public UIExtendedWindow()
    {
        // var go = new GameObject("TEST");
        // Object.DontDestroyOnLoad(go);
        // go.hideFlags = HideFlags.HideAndDontSave;
        // uiPopup = go.AddComponent<UIPopup>();
        // uiPopup.hideFlags = HideFlags.HideAndDontSave;
        // Object.DontDestroyOnLoad(go);
        // transform = go.transform;
        // blockLayer = new EventSystemLayer("TEST");
        // buttonEvents = new List<KeyValuePair<MenuButton, UnityAction<ILayeredEventData>>>();
        // Controllers = new Il2CppReferenceArray<IControllerGroup>([]);
    }

    public static void DoTheThing(ILayeredEventData eventData)
    {
        // var x = ControllerManager.GetController(2);
        // ControllerManager.SwapToPrimary(eventData.Input);
        // // uis.ShowModalWindow();
    }
}