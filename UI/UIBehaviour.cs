using GrimbaHack._bank;
using GrimbaHack.Utility;
using Il2CppInterop.Runtime.Injection;
using Il2CppSystem;
using nway.ui;
using UnityEngine;
using UnityEngine.InputSystem;
using UniverseLib.Runtime.Il2Cpp;
using IntPtr = System.IntPtr;
using Object = System.Object;

namespace GrimbaHack.UI;

public class UIBehaviour : MonoBehaviour
{
    private UIExtendedWindow test;

    internal static void Setup()
    {
        ClassInjector.RegisterTypeInIl2Cpp<UIBehaviour>();
        GameObject gameObject = new("UIBehaviour");
        DontDestroyOnLoad(gameObject);
        gameObject.hideFlags = HideFlags.HideAndDontSave;
        gameObject.AddComponent<UIBehaviour>();
    }

    internal void Update()
    {
        UIManager.Update();
        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            UIManager.ShowUI = !UIManager.ShowUI;
        }
    }
}