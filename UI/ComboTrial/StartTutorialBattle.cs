using System;
using epoch.db;
using HarmonyLib;
using nway;
using nway.gameplay;
using nway.gameplay.ui;
using nway.ui;
using UnityEngine.Events;

namespace GrimbaHack.UI.ComboTrial;

[HarmonyPatch(typeof(UIMainMenu), nameof(UIMainMenu.InitializeTrainingMenu))]
public class StartTutorialBattle
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
        submit.SetOnSubmit((Action<ILayeredEventData>)((ILayeredEventData eventData) =>
        {
            SelectHero.LoadUIHeroSelect();
        }));
        submit.Enabled = true;

    // __instance.AddButtonCallback(MenuButton.XboxLB, (Action<ILayeredEventData>)(eventData =>
        // {
        //     var screen = new ScreenTutorial();
        //     Plugin.Log.LogInfo("BEFORE GET TUTORIAL");
        //     screen.tutorial = TableStory.instance.tutorial;
        //     Plugin.Log.LogInfo("AFTER GET TUTORIAL");
        //     var b = new StoryBattle();
        //     var db = new DB_StoryBattle();
        //     db.displayName = "COMBO TRIAL 100";
        //     db.enemyLeader = 1;
        //     db.enemyAssist1 = 2;
        //     db.enemyAssist2 = 3;
        //     db.playerLeader = 1;
        //     db.playerAssist1 = 2;
        //     db.playerAssist2 = 3;
        //     db.infiniteTime = true;
        //     db.arenaID = "Arena_Training_Pit";
        //     for (int i = 0; i < 3; i++)
        //     {
        //         db.playerHealthStack.Add(1);
        //         db.enemyHealthStack.Add(1);
        //         db.enemyStartingHealthPercent.Add(100);
        //         db.playerStartingHealthPercent.Add(100);
        //     }
        //
        //     // db.enemyReceiveDamageFromSkillTags.Add(SkillTag.Normal);
        //     // db.enemyReceiveDamageFromSkillTags.Add(SkillTag.Projectile);
        //     // db.enemyReceiveDamageFromSkillTags.Add(SkillTag.Super);
        //     // db.enemyReceiveDamageFromSkillTags.Add(SkillTag.Throw);
        //     // db.winConditionType = CombatConditionType.None;
        //     // // db.playerZord = 1;
        //     // // db.enemyZord = 1;
        //
        //     b.SetAIConfig(0, StoryDifficulty.Easy);
        //
        //     Plugin.Log.LogInfo("INIT");
        //     b.Initialize(db);
        //     Plugin.Log.LogInfo("ADD STORY");
        //     screen.tutorial.chapters[0].AddStoryBattle(b);
        //     Plugin.Log.LogInfo("ENTER SCREEN");
        //     SceneManager.EnterScreen(screen);
        // }));
    }
}