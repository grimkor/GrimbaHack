using System;
using epoch.db;
using HarmonyLib;
using nway.gameplay;
using nway.gameplay.match;
using nway.gameplay.ui;
using nway.ui;

namespace GrimbaHack.UI.ComboTrial;

// [HarmonyPatch(typeof(UIMainMenu), nameof(UIMainMenu.OnInitializeComponents))]
// public class Test
// {
//     static void Postfix(UIMainMenu __instance)
//     {
//         __instance.AddButtonCallback(MenuButton.XboxLB, (Action<ILayeredEventData>)(_ =>
//         {
//             var match = new TrainingMatch();
//             match.arenaId = "Arena_Training_Pit";
//             var team = new TeamHeroSelection();
//             var team2 = new TeamHeroSelection();
//             for (int i = 0; i < 3; i++)
//             {
//                 team.SetHero(i, new TeamHeroSelection.Hero(i + 1));
//                 team2.SetHero(i, new TeamHeroSelection.Hero(i + 1));
//             }
//
//             match.Initialize(team, team2, PlayerControllerMapping.CreateForSinglePlayer());
//             MatchManager.ShowLoadingScreen(match);
//             var startup = MatchManager.Get.SetupGamePlay(match, MatchManager.Get.GetPID(),
//                 PlayerControllerMapping.CreateForTwoPlayerTraining());
//             var gameplay = new PvPGamePlay(startup);
//
//             gameplay.Start();
//         }));
//     }
// }

[HarmonyPatch(typeof(UIHeroSelect), nameof(UIHeroSelect.OnConfirmSkinSelect))]
public class SelectHero
{
    public static TeamHeroSelection.Hero selectedHero;

    static void Postfix(UIHeroSelect __instance, UIHeroSelect.Team team, bool isSkinUnlocked)
    {
        selectedHero = new TeamHeroSelection.Hero(__instance.leftPlayer.teamSelection.selections[0].data.heroIndex);
        selectedHero.color = __instance.leftPlayer.teamSelection.selections[0].skin.colorID;
        selectedHero.skin = __instance.leftPlayer.teamSelection.selections[0].skin.skinID;
        __instance.CloseWindow(true);
        ShowTutorialSelection();
    }

    static void ShowTutorialSelection()
    {
        var screen = new ScreenTutorial();
        screen.tutorial = TableStory.instance.tutorial;
        screen.tutorial.chapters[0].battles.Clear();
        for (int n = 1; n < 11; n++)
        {
            var b = new StoryBattle();
            var db = new DB_StoryBattle();
            db.displayName = $"Combo Trial {n}";
            db.enemyLeader = 1;
            db.enemyAssist1 = 2;
            db.enemyAssist2 = 3;
            db.playerLeader = 1;
            db.playerAssist1 = 2;
            db.playerAssist2 = 3;
            db.infiniteTime = true;
            db.arenaID = "Arena_Training_Pit";
            for (int i = 0; i < 3; i++)
            {
                db.playerHealthStack.Add(1);
                db.enemyHealthStack.Add(1);
                db.enemyStartingHealthPercent.Add(100);
                db.playerStartingHealthPercent.Add(100);
            }

            db.enemyReceiveDamageFromSkillTags.Add(SkillTag.Normal);
            db.enemyReceiveDamageFromSkillTags.Add(SkillTag.Projectile);
            db.enemyReceiveDamageFromSkillTags.Add(SkillTag.Super);
            db.enemyReceiveDamageFromSkillTags.Add(SkillTag.Throw);
            db.winConditionType = CombatConditionType.None;

            b.Initialize(db);
            screen.tutorial.chapters[0].AddStoryBattle(b);
        }

        SceneManager.EnterScreen(screen);
    }

    public static void StartGame()
    {
        var match = new TrainingMatch();
        match.arenaId = "Arena_Training_Pit";
        var team1 = new TeamHeroSelection();
        var team2 = new TeamHeroSelection();
        for (int i = 0; i < 3; i++)
        {
            team1.SetHero(i, new TeamHeroSelection.Hero(i + 1));
            team2.SetHero(i, new TeamHeroSelection.Hero(i + 1));
        }

        team1.SetHero(0, selectedHero);
        match.Initialize(team1, team2, PlayerControllerMapping.CreateForSinglePlayer());
        MatchManager.ShowLoadingScreen(match);
        var startup = MatchManager.Get.SetupGamePlay(match, MatchManager.Get.GetPID(),
            PlayerControllerMapping.CreateForTwoPlayerTraining());
        var gameplay = new PvPGamePlay(startup);

        gameplay.Start();
    }
}

[HarmonyPatch(typeof(UITutorialSelect.BattleBlade), nameof(UITutorialSelect.BattleBlade.OnSubmit))]
public class OverrideSelection
{
    static bool Prefix()
    {
        SelectHero.StartGame();
        return false;
    }
}