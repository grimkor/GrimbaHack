using System;
using epoch.db;
using HarmonyLib;
using nway.gameplay.ui;
using nway.ui;

namespace GrimbaHack.Modules.ComboTrial.UI;

[HarmonyPatch(typeof(UIMainMenu), nameof(UIMainMenu.InitializeTrainingMenu))]
public class CreateMainMenuComboTrialOption
{
    static void Postfix(UIMainMenu __instance)
    {
        var submit = __instance.trainingPage.Page.AddItem<UIMainMenu.MainMenuSubmitMini>("comboTrials");
        submit.SetOnSubmit((Action<ILayeredEventData>)(eventData =>
        {
            ControllerManager.SwapToPrimary(eventData.Input);
            __instance.EnterTeamSelect(MatchType.Training);
        }));
        submit.LocalizedText = "Combo Trials";
        __instance.trainingPage.Page.CreateChain(true, true, __instance.SubMenuLayer);
        submit.ShowBannerUnlocked(true);
        submit.Selectable.interactable = true;
        submit.SetOnSubmit((Action<ILayeredEventData>)((_) =>
        {
            ComboTrialMenuManager.IsTrialCharacterSelect = true;
            ComboTrialMenuManager.LoadUIHeroSelect();
        }));
        submit.Enabled = true;
    }
}
