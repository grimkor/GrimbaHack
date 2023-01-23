using System;
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

    static UnlimitedInstall()
    {
        Instance = new UnlimitedInstall();
    }

    private void setEnabled(bool value)
    {
        Enabled = value;
        if (value)
        {
            if (_ericDefaultInstallLength == 0)
                _ericDefaultInstallLength = TableStatusEffect.instance.statusEffectMap["quantum_super_install"]
                    .staticDurationFrames;
            if (_adamDefaultInstallLength == 0)
                _adamDefaultInstallLength = TableStatusEffect.instance.statusEffectMap["adam_super_install"]
                    .staticDurationFrames;
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

    private int _ericDefaultInstallLength = 0;
    private int _adamDefaultInstallLength = 0;

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
        unlimitedInstallToggle.onValueChanged.AddListener(new Action<bool>((value) => { Instance.setEnabled(value); }));

        UIFactory.SetLayoutElement(unlimitedInstallToggle.gameObject, minHeight: 25, minWidth: 50);
    }
}