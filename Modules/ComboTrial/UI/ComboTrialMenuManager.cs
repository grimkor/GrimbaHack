using epoch.db;
using GrimbaHack.UI.TrainingMode;
using GrimbaHack.Utility;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using nway.gameplay;
using nway.gameplay.match;
using nway.gameplay.ui;
using nway.ui;
using MatchType = epoch.db.MatchType;

namespace GrimbaHack.Modules.ComboTrial.UI;

[HarmonyPatch(typeof(UIHeroSelect), nameof(UIHeroSelect.OnConfirmSkinSelect))]
public class ComboTrialMenuManager
{
    private static TeamHeroSelection.Hero _selectedHero;
    private static BaseScreen _heroSelection;
    private static TrainingMatch _match;
    public static bool IsTrialCharacterSelect;

    static ComboTrialMenuManager()
    {
        OnEnterMainMenuActionHandler.Instance.AddCallback(() => { IsTrialCharacterSelect = false; });
    }

    static void Postfix(UIHeroSelect __instance, UIHeroSelect.Team team, bool isSkinUnlocked)
    {
        if (SceneManager.screenManager.currentScreen.Pointer == _heroSelection?.Pointer)
        {
            _match = null;
            ComboTrialManager.Instance.SetHero(__instance.leftPlayer.teamSelection.selections[0].data.heroIndex);
            _selectedHero = new TeamHeroSelection.Hero(__instance.leftPlayer.teamSelection.selections[0].data.heroIndex);
            _selectedHero.color = __instance.leftPlayer.teamSelection.selections[0].skin.colorID;
            _selectedHero.skin = __instance.leftPlayer.teamSelection.selections[0].skin.skinID;
            __instance.Hide();
            ShowTutorialSelection();
        }
    }

    public static ScreenTutorial CreateTutorialSelection(System.Collections.Generic.List<ComboExport> characterCombos)
    {
        _match = null;
        var screen = new ScreenTutorial();
        screen.tutorial = new Story
        {
            db = new DB_Story
            {
                displayName = "Combo Trial",
                type = StoryType.Tutorial,
                id = "comboTrial",
                displayDescription = "Combo Trial",
                enableZordSelection = false
            },
            chapters = new List<StoryChapter>()
        };
        var chapter = new StoryChapter
        {
            battles = new(),
            db = new DB_StoryChapter
            {
                displayName = "Combo Trial",
                id = "comboTrial",
                actID = "comboTrial",
                storyID = "comboTrial"
            },
        };

        if (characterCombos.Count < 1)
        {
            var b = new StoryBattle();
            var db = new DB_StoryBattle();
            db.displayName = $"None";
            db.enemyLeader = 1;
            db.enemyAssist1 = 2;
            db.enemyAssist2 = 3;
            db.playerLeader = _selectedHero.index;
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
            chapter.AddStoryBattle(b);
        }

        foreach (var combo in characterCombos)
        {
            var b = new StoryBattle();
            var db = new DB_StoryBattle();
            db.displayName = $"{combo.Title}";
            db.enemyLeader = 1;
            db.enemyAssist1 = 2;
            db.enemyAssist2 = 3;
            db.playerLeader = _selectedHero.index;
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
            chapter.AddStoryBattle(b);
        }

        screen.tutorial.chapters.Add(chapter);
        _heroSelection = screen;
        return screen;
    }

    static void ShowTutorialSelection()
    {
        if (ComboTrialManager.Instance.Combos == null || ComboTrialManager.Instance.Combos.Count < 1)
        {
            var messageBox = new UIErrorMessageBox();
            messageBox.header = "Oops!";
            messageBox.message = "No combos found for this character";
            messageBox.ShowModalWindow();
            return;
        }

        SceneManager.EnterScreen(CreateTutorialSelection(ComboTrialManager.Instance.Combos));
    }

    public static void StartGame()
    {
        _match = new TrainingMatch();
        _match.arenaId = "Arena_Training_Pit";
        var team1 = new TeamHeroSelection();
        var team2 = new TeamHeroSelection();
        for (int i = 0; i < 3; i++)
        {
            team1.SetHero(i, new TeamHeroSelection.Hero(i + 1));
            team2.SetHero(i, new TeamHeroSelection.Hero(i + 1));
        }

        team1.SetHero(0, new TeamHeroSelection.Hero(ComboTrialManager.Instance.GetCurrentCombo().CharacterId));
        team2.SetHero(0, new TeamHeroSelection.Hero(ComboTrialManager.Instance.GetCurrentCombo().DummyId));
        _match.Initialize(team1, team2, PlayerControllerMapping.CreateForSinglePlayer());
        MatchManager.ShowLoadingScreen(_match);
        var startup = MatchManager.Get.SetupGamePlay(_match, MatchManager.Get.GetPID(),
            PlayerControllerMapping.CreateForTwoPlayerTraining());
        var gameplay = new PvPGamePlay(startup);

        ComboTrialManager.Instance.SetupComboTrial(ComboTrialManager.Instance.GetCurrentCombo());
        gameplay.Start();
    }

    public static void LoadUIHeroSelect()
    {
        _heroSelection = new ScreenHeroSelection
        {
            hubArgs = new ScreenHeroSelection.HeroSelectionArgs
            {
                matchType = MatchType.Arcade
            }
        };
        SceneManager.EnterScreen(_heroSelection);
    }

    [HarmonyPatch(typeof(UITutorialSelect.BattleBlade), nameof(UITutorialSelect.BattleBlade.OnSubmit))]
    private class OverrideSelection
    {
        static bool Prefix(UITutorialSelect.BattleBlade __instance)
        {
            if (SceneManager.screenManager.currentScreen.Pointer == _heroSelection?.Pointer)
            {
                ComboTrialManager.Instance.SetComboId(__instance.index);
                StartGame();
                return false;
            }

            return true;
        }
    }
}

[HarmonyPatch(typeof(UIHeroSelect), nameof(UIHeroSelect.SetUpCard))]
public class HeroCardEnabling
{
    static void Postfix(UIHeroSelect.UIHeroCard heroCard, DB_CharacterSelectEntry data)
    {
        if (!ComboTrialMenuManager.IsTrialCharacterSelect) return;
        heroCard.ArcadeCompleted = false;
        heroCard.Enabled = heroCard.Enabled && ComboTrialDataManager.CharacterHasCombos(data.heroIndex);
        heroCard.HeroEnabled =
            heroCard.HeroEnabled && ComboTrialDataManager.CharacterHasCombos(data.heroIndex);
    }
}