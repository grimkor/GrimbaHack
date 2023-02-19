using System;
using GrimbaHack.Utility;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;

namespace GrimbaHack.Modules;

public class UnlimitedInstall : CheatPrevention
{
    private UnlimitedInstall()
    {
    }

    public static UnlimitedInstall Instance { get; private set; }
    private bool _enabled;
    private int _ericDefaultInstallLength;
    private int _adamDefaultInstallLength;

    static UnlimitedInstall()
    {
        Instance = new UnlimitedInstall();
        Instance._ericDefaultInstallLength = TableStatusEffect.instance.statusEffectMap["quantum_super_install"]
            .staticDurationFrames;
        Instance._adamDefaultInstallLength = TableStatusEffect.instance.statusEffectMap["adam_super_install"]
            .staticDurationFrames;
        OnEnterTrainingMatchActionHandler.Instance.AddCallback(() => Instance.SetUnlimitedInstall(Instance._enabled));
        OnEnterMainMenuActionHandler.Instance.AddCallback(() => Instance.SetUnlimitedInstall(false));
    }

    public bool Enabled
    {
        get => _enabled;
        set
        {
            SetUnlimitedInstall(value);
            _enabled = value;
        }
    }

    private void SetUnlimitedInstall(bool enable)
    {
        if (enable)
        {
            TableStatusEffect.instance.statusEffectMap["quantum_super_install"].staticDurationFrames = -1;
            TableStatusEffect.instance.statusEffectMap["adam_super_install"].staticDurationFrames = -1;
        }
        else
        {
            TableStatusEffect.instance.statusEffectMap["quantum_super_install"].staticDurationFrames =
                _ericDefaultInstallLength;
            TableStatusEffect.instance.statusEffectMap["adam_super_install"].staticDurationFrames =
                _adamDefaultInstallLength;
        }
    }


    public static void CreateUIControls(GameObject contentRoot)
    {
        var unlimitedInstallGroup = UIFactory.CreateUIObject("UnlimitedInstallGroup", contentRoot);
        UIFactory.SetLayoutElement(unlimitedInstallGroup);
        UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(unlimitedInstallGroup, false, false, true, true, padLeft: 25,
            spacing: 10, childAlignment: TextAnchor.MiddleLeft);
        UIFactory.CreateToggle(unlimitedInstallGroup, "UnlimitedInstallToggle", out var unlimitedInstallToggle,
            out var unlimitedInstallToggleLabel);
        unlimitedInstallToggle.isOn = false;
        unlimitedInstallToggleLabel.text = "Unlimited Install Time (Eric/Adam)";
        unlimitedInstallToggle.onValueChanged.AddListener(new Action<bool>((value) => { Instance.Enabled = value; }));

        UIFactory.SetLayoutElement(unlimitedInstallToggle.gameObject, minHeight: 25, minWidth: 50);
    }
}