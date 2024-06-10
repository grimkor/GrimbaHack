using GrimbaHack.Utility;
using HarmonyLib;
using nway.gameplay.ui;

namespace GrimbaHack.Modules;

public sealed class PickSameCharacter : CheatPrevention
{
    private PickSameCharacter()
    {
    }

    public static readonly PickSameCharacter Instance = new();
    public new bool Enabled;

    static PickSameCharacter()
    {
        OnUIHeroSelectSelectNextActionHandler.Instance.AddCallback(
            (heroCard, team) =>
            {
                if (Instance.Enabled)
                {
                    heroCard.EnableFor(team, true);
                }
            });
    }
}
