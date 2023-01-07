using HarmonyLib;
using nway.gameplay.ui;

namespace GrimbaHack;

public class PickSameCharacter
{
    [HarmonyPatch(typeof(UIHeroSelect), nameof(UIHeroSelect.SelectNext))]
    private class PatchSelectNext
    {
        private static void Prefix(ref UIHeroSelect.UIHeroCard heroCard, UIHeroSelect.Team team)
        {
            heroCard.EnableFor(team, true);
        }
    }
}