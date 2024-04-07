using GrimbaHack.Utility;

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
        OnEnterTrainingMatchActionHandler.Instance.AddPostfix(() => Instance.SetUnlimitedInstall(Instance._enabled));
        OnEnterMainMenuActionHandler.Instance.AddCallback(() => Instance.SetUnlimitedInstall(false));
    }

    public void SetEnabled(bool enable)
    {
        SetUnlimitedInstall(enable);
        Enabled = enable;
        _enabled = enable;
    }

    public bool GetEnabled()
    {
        return Instance._enabled;
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
}
