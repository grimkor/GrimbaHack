using System;
using GrimbaHack.Utility;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;
using Object = UnityEngine.Object;

namespace GrimbaHack.Modules;

public class MemoryLeakFix
{
    public bool Enabled { get; set; }
    private MemoryLeakFix()
    {
    }
    
    public static MemoryLeakFix Instance { get; private set; }

    static MemoryLeakFix()
    {
        Instance = new MemoryLeakFix();
        Instance.Enabled = Plugin.EXPERIMENTAL_MemoryFix.Value;
        OnEnterTrainingMatchActionHandler.Instance.AddPrefixCallback(() =>
        {
            if (!Instance.Enabled)
            {
                return;
            }
            var go = Object.FindObjectsOfType<Material>();
            foreach (var material in go)
            {
                if (!material.name.Contains("vfx_m"))
                {
                    continue;
                }
                Object.Destroy(material);
            }
        });
    }

    public static void CreateUIControls(GameObject contentRoot)
    {
        var textureLoaderGroup = UIFactory.CreateUIObject("MemoryLeakFixGroup", contentRoot);
        UIFactory.SetLayoutElement(textureLoaderGroup);
        UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(textureLoaderGroup, false, false, true, true, padLeft: 25,
            spacing: 10, childAlignment: TextAnchor.MiddleLeft);
        UIFactory.CreateToggle(textureLoaderGroup, "MemoryLeakFixToggle", out var toggle,
            out var label);
        toggle.isOn = Instance.Enabled;
        label.text = "Enable memory leak fix";
        toggle.onValueChanged.AddListener(new Action<bool>((value) =>
        {
            Instance.Enabled = value;
            Plugin.EXPERIMENTAL_MemoryFix.Value = value;
        }));

        UIFactory.SetLayoutElement(toggle.gameObject, minHeight: 25, minWidth: 50);
    }
}