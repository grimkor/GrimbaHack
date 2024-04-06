using Il2CppInterop.Runtime.Injection;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GrimbaHack.UI;

public class UIBehaviour : MonoBehaviour
{

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
