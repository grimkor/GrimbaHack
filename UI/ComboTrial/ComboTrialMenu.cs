using HarmonyLib;
using nway.ui;

namespace GrimbaHack.UI.ComboTrial;

[HarmonyPatch(typeof(UITutorialSelect), nameof(UITutorialSelect.OnInitializeComponents))]
public class ComboTrialMenu
{
    static void Postfix()
    {
        Plugin.Log.LogInfo("UITutorialSeelct.OnInit");
    }
}