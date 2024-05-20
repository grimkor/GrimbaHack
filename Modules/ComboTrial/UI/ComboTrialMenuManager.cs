using System.Text.RegularExpressions;
using System.Threading.Tasks;
using epoch.db;
using GrimbaHack.UI.TrainingMode;
using GrimbaHack.Utility;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using nway.gameplay;
using nway.gameplay.match;
using nway.gameplay.ui;
using nway.ui;
using UnityEngine.UI;
using MatchType = epoch.db.MatchType;
using Object = UnityEngine.Object;

namespace GrimbaHack.Modules.ComboTrial.UI;

[HarmonyPatch(typeof(UIHeroSelect), nameof(UIHeroSelect.OnShow))]
public class ComboTrialMenuManager
{
    private static TeamHeroSelection.Hero _selectedHero;
    private static BaseScreen _heroSelection;
    private static TrainingMatch _match;
    public static bool IsTrialCharacterSelect;
    private static UIHeroSelect _uiWindow;

    static ComboTrialMenuManager()
    {
        OnEnterMainMenuActionHandler.Instance.AddCallback(() =>
        {
            var existingUITutorial =
                BaseUIManager.Get.PopupManager.uiPopupStorage.transform.FindChild("UI_TutorialSelection");
            if (existingUITutorial != null)
            {
                Object.Destroy(existingUITutorial);
                BaseUIManager.Get.PopupManager.uiPopupAssetList.Remove("UI_TutorialSelection");
            }

            IsTrialCharacterSelect = false;
        });
    }

    static void Postfix(UIHeroSelect __instance)
    {
        _uiWindow = __instance;
    }

    private static void SelectHero(UIHeroSelect __instance)
    {
        _match = null;
        ComboTrialManager.Instance.SetHero(__instance.leftPlayer.teamSelection.selections[0].data.heroIndex);
        _selectedHero =
            new TeamHeroSelection.Hero(__instance.leftPlayer.teamSelection.selections[0].data.heroIndex);
        _selectedHero.color = __instance.leftPlayer.teamSelection.selections[0].skin.colorID;
        _selectedHero.skin = __instance.leftPlayer.teamSelection.selections[0].skin.skinID;
        __instance.Hide();
        ShowTutorialSelection();
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

        for (var index = 0; index < characterCombos.Count; index++)
        {
            var combo = characterCombos[index];
            var b = new StoryBattle();
            var db = new DB_StoryBattle();
            db.displayName = $"{combo.Title}";
            db.id = index.ToString();
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

    [HarmonyPatch(typeof(UITutorialSelect), nameof(UITutorialSelect.OnInitializeComponents))]
    private class RenameTutorialBanner
    {
        static void Postfix(UITutorialSelect __instance)
        {
            if (ComboTrialManager.Instance.IsComboTrial) return;
            if (SceneManager.screenManager.currentScreen.Pointer == _heroSelection?.Pointer)
            {
                var titleGo = __instance.transform.FindChild("pageTitle");
                if (titleGo != null)
                {
                    var text = titleGo.GetComponentInChildren<Text>();
                    if (text != null)
                    {
                        var heroName = TableUIText.instance.GetUIText(TableHero.instance
                            .GetCharacterSelectEntryByHeroIndex(_selectedHero.index).displayName);
                        text.text = Regex.Split(heroName, " - ")[0];
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(UITutorialSelect.BattleBlade), nameof(UITutorialSelect.BattleBlade.OnSubmit))]
    private class OverrideSelection
    {
        static bool Prefix(UITutorialSelect.BattleBlade __instance)
        {
            if (ComboTrialManager.Instance.IsComboTrial) return false;
            if (SceneManager.screenManager.currentScreen.Pointer == _heroSelection?.Pointer)
            {
                ComboTrialManager.Instance.SetComboId(__instance.index);
                StartGame();
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(UIHeroSelect.UIHeroCard), nameof(UIHeroSelect.UIHeroCard.Confirm))]
    private class OverrideRandom
    {
        static void Postfix()
        {
            if (SceneManager.screenManager.currentScreen.Pointer == _heroSelection?.Pointer)
            {
                SelectHero(_uiWindow);
            }
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

[HarmonyPatch(typeof(UITutorialSelect), nameof(UITutorialSelect.OnInitializeComponents))]
public class ScreenTutEnter
{
    static void Prefix(UITutorialSelect __instance)
    {
        __instance.selectedBattleID = ComboTrialManager.Instance.ComboIndex.ToString();
    }
}

[HarmonyPatch(typeof(UITutorialSelect), nameof(UITutorialSelect.OnShow))]
public class ScreenTutShow
{
    static void Postfix(UITutorialSelect __instance)
    {
        Task.Delay(200).ContinueWith(async (t) =>
        {
            __instance.menuOffset.OnSelect(__instance.startingBlade.root);
        });
    }
}
