using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using epoch.db;
using GrimbaHack.Data;
using GrimbaHack.Utility;
using HarmonyLib;
using nway.gameplay;
using nway.gameplay.match;
using nway.gameplay.ui;
using UnityEngine;
using MatchType = epoch.db.MatchType;

namespace GrimbaHack.Modules;

public class TextureLoader : ModuleBase
{
    public bool Enabled
    {
        get => Instance._enabled;
        set
        {
            Instance._enabled = value;
            Plugin.EXPERIMENTAL_TextureLoader.Value = value;
        }
    }

    private bool _enabled;

    private Dictionary<string, byte[]> CharacterFilesForLoading { get; set; } = new();
    private Dictionary<string, byte[]> CoreFilesForLoading { get; set; } = new();
    private readonly Dictionary<string, Dictionary<string, Dictionary<string, byte[]>>> _characterFilesInMemory = new();
    private bool _loadedForScene;
    private bool Preloaded { get; set; }
    public int[] LocalPlayerSkins = new[] { 1, 1, 1 };
    public int[] RemotePlayerSkins = new[] { 1, 1, 1 };

    private TextureLoader()
    {
    }

    public static TextureLoader Instance { get; private set; }

    static TextureLoader()
    {
        Instance = new TextureLoader();
        Instance.Enabled = Plugin.EXPERIMENTAL_TextureLoader.Value;
        OnSimulationInitializeActionHandler.Instance.AddPostfix(() =>
        {
            if (!Instance.Enabled)
            {
                return;
            }

            if (Instance._loadedForScene)
            {
                return;
            }

            var textures = Resources.FindObjectsOfTypeAll<Texture2D>();
            foreach (var tex in textures)
            {
                if (Instance.CoreFilesForLoading.TryGetValue(tex.name, out var newCoreTex))
                {
                    ImageConversion.LoadImage(tex, newCoreTex);
                }

                if (Instance.CharacterFilesForLoading.TryGetValue(tex.name, out var newTex))
                {
                    ImageConversion.LoadImage(tex, newTex);
                }
            }

            Instance._loadedForScene = true;
        });

        OnEnterMainMenuActionHandler.Instance.AddCallback(() =>
        {
            Instance._loadedForScene = false;
            // Instance.Cleanup();
        });

        OnZordSelectedActionHandler.Instance.AddPrefixCallback((__instance, team, _) =>
        {
            var playerOneIsLocal = __instance?.directMatch?.localPlayerSlot != 1;
            if (team == 0)
            {
                MapPlayerTeam(__instance.leftPlayer,
                    playerOneIsLocal
                        ? Instance.LocalPlayerSkins
                        : Instance.RemotePlayerSkins);
            }
            else
            {
                MapPlayerTeam(__instance.rightPlayer,
                    playerOneIsLocal
                        ? Instance.RemotePlayerSkins
                        : Instance.LocalPlayerSkins);
            }
        });

        OnMatchManagerSetupGamePlay.Instance.AddCallback((match, s, arg3) =>
        {
            Instance.CharacterFilesForLoading.Clear();
            if (!Instance.Enabled)
            {
                return;
            }

            if (match.isSpectatorMode)
            {
                return;
            }

            if (match.GetMatchType() == MatchType.Story)
            {
                return;
            }

            LoadTextures(match);
        });

        OnEnterMainMenuActionHandler.Instance.AddCallback(() =>
        {
            if (!Instance.Enabled)
            {
                return;
            }

            Instance.LoadFiles();
        });
    }

    private static void LoadTextures(Match match)
    {
        Plugin.Log.LogInfo("Loading Textures from loader");
        var localPlayerPosition = match.GetMatchUserList()
            .FindIndex(new Func<PlayerInfo, bool>(p =>
                p.pid == GameManager.instance.primaryUser.playerProfile.PlayerID));
        var isPlayerOne = localPlayerPosition != 1;
        var localPlayer = match.GetMatchUserList()[isPlayerOne ? 0 : 1].GetTeamSelection();
        var remotePlayer = match.GetMatchUserList()[isPlayerOne ? 1 : 0].GetTeamSelection();
        PrepareTextures(localPlayer, remotePlayer);
    }

    private static void PrepareTextures(TeamHeroSelection localPlayer, TeamHeroSelection remotePlayer)
    {
        Plugin.Log.LogInfo("Preparing textures for loader");
        var p1Heroes = localPlayer.heroes;
        var i = 0;
        foreach (var colorId in Instance.LocalPlayerSkins)
        {
            var hero = p1Heroes[i++];
            if (colorId <= 2) continue;
            var heroData = TableHero.instance.GetDBHeroData(hero.index);
            var skin = TableHeroAssetInfo.instance.GetSkinColorProperty(heroData.heroName, hero.skin, colorId);
            Plugin.Log.LogInfo($"Found {skin.displayName} for the local player");
            var opponentSameHero = remotePlayer.heroes.FirstOrDefault(h => h.index == hero.index, null);
            if (opponentSameHero != null)
            {
                hero.color = opponentSameHero.color == 1 ? 2 : 1;
            }
            else
            {
                hero.color = 1;
            }

            var assetMap = Global.AssetHeroInfoMapper
                .Find(x => x.skinColorPropertyPrefix == $"{heroData.heroName}-{skin.skinID}");
            var folder = assetMap.folder;
            if (Instance._characterFilesInMemory[folder].TryGetValue(skin.displayName, out var textures))
            {
                foreach (var texture in textures)
                {
                    var textureName = texture.Key;
                    var lowerKey = texture.Key.ToLower();
                    if (lowerKey.EndsWith("_alb") || lowerKey.EndsWith("_n") || lowerKey.EndsWith("_mroe"))
                    {
                        textureName = $"{textureName}{(hero.color == 2 ? $"{assetMap.colorSuffix}2" : "")}";
                    }

                    Instance.CharacterFilesForLoading[textureName] = texture.Value;
                }
            }

            hero.skin = skin.skinID;
        }

        var p2Heroes = remotePlayer.heroes;
        i = 0;
        foreach (var colorId in Instance.RemotePlayerSkins)
        {
            var hero = p2Heroes[i++];
            if (colorId <= 2) continue;
            var heroData = TableHero.instance.GetDBHeroData(hero.index);
            var skin = TableHeroAssetInfo.instance.GetSkinColorProperty(heroData.heroName, hero.skin, colorId);
            Plugin.Log.LogInfo($"Found {skin.displayName} for the remote player");
            var opponentSameHero = localPlayer.heroes.FirstOrDefault(h => h.index == hero.index, null);
            if (opponentSameHero != null)
            {
                hero.color = opponentSameHero.color == 1 ? 2 : 1;
            }
            else
            {
                hero.color = 1;
            }

            var assetMap = Global.AssetHeroInfoMapper
                .Find(x => x.skinColorPropertyPrefix == $"{heroData.heroName}-{skin.skinID}");
            var folder = assetMap.folder;
            if (Instance._characterFilesInMemory[folder].TryGetValue(skin.displayName, out var textures))
            {
                foreach (var texture in textures)
                {
                    var textureName = texture.Key;
                    var lowerKey = texture.Key.ToLower();
                    if (lowerKey.EndsWith("_alb") || lowerKey.EndsWith("_n") || lowerKey.EndsWith("_mroe"))
                    {
                        textureName = $"{textureName}{(hero.color == 2 ? $"{assetMap.colorSuffix}2" : "")}";
                    }

                    Plugin.Log.LogInfo($"Loading {textureName} into Loader");
                    Instance.CharacterFilesForLoading[textureName] = texture.Value;
                }
            }

            hero.skin = skin.skinID;
        }
    }

    public void CreateFolders()
    {
        Plugin.Log.LogInfo("Creating folder structure for textures");
        var rootPath = Path.Join(Paths.PluginPath, "textures");
        foreach (var assetHeroNameMap in Global.AssetHeroInfoMapper)
        {
            if (!File.Exists(Path.Join(rootPath, assetHeroNameMap.folder)))
            {
                Directory.CreateDirectory(Path.Join(rootPath, assetHeroNameMap.folder));
            }
        }

        if (!File.Exists(Path.Join(rootPath, "_static")))
        {
            Directory.CreateDirectory(Path.Join(rootPath, "_static"));
        }

        Plugin.Log.LogInfo("Finished creating folder structure for textures");
    }

    public void LoadFiles()
    {
        Plugin.Log.LogInfo("Preloading textures into memory...");
        if (Instance.Preloaded)
        {
            Plugin.Log.LogInfo("Preload already happened, returning with no action.");
            return;
        }

        var filepath = Path.Join(Paths.PluginPath, "textures");

        Plugin.Log.LogInfo("loading textures found in _static...");
        foreach (var file in Directory.GetFiles(Path.Join(filepath, "_static")))
        {
            var filename = Path.GetFileName(file).Split(".")[0];
            Instance.CoreFilesForLoading[filename] = File.ReadAllBytes(Path.Join(file));
        }

        Plugin.Log.LogInfo("Finished loading _static");
        Plugin.Log.LogInfo("loading character textures...");
        var characterFolders = Directory.GetDirectories(filepath);
        foreach (var characterFolder in characterFolders)
        {
            if (Path.GetFileName(characterFolder) == "_static") continue;
            var characterFolderName = Path.GetFileName(characterFolder);
            Instance._characterFilesInMemory[characterFolderName] = new();
            var skinFolders = Directory.GetDirectories(Path.Join(filepath, characterFolderName));
            foreach (var skinFolder in skinFolders)
            {
                var skinFolderName = Path.GetFileName(skinFolder);
                Instance._characterFilesInMemory[characterFolderName][skinFolderName] = new();
                var skinFiles = Directory.GetFiles(Path.Join(filepath, characterFolderName, skinFolderName));
                foreach (var file in skinFiles)
                {
                    var filename = Path.GetFileName(file).Split('.')[0];
                    Instance._characterFilesInMemory[characterFolderName][skinFolderName][filename] =
                        File.ReadAllBytes(Path.Join(file));
                }
            }
        }

        Plugin.Log.LogInfo("finished loading character textures");
        Plugin.Log.LogInfo("Adding additional options to character select screen...");
        foreach (var character in Instance._characterFilesInMemory)
        {
            var mapper = Global.AssetHeroInfoMapper.Find(x => x.folder == character.Key);
            var heroAssetTable = TableHeroAssetInfo.instance;
            var skinOptionInfo = heroAssetTable.skinOptionInfoTable[mapper.skinOption];
            var newSkin = skinOptionInfo.GetSkin(mapper.skinType);
            var templateColor = heroAssetTable.skinColorPropertyTable[$"{mapper.skinColorPropertyPrefix}-1"];
            var i = newSkin.colors.Count + 1;
            foreach (var skinSet in character.Value)
            {
                var colorProperty = new DB_SkinColorProperty(templateColor);
                colorProperty.displayName = skinSet.Key;
                colorProperty.colorID = i;
                newSkin.AddColor(colorProperty);
                heroAssetTable.skinColorPropertyTable.Add($"{mapper.skinColorPropertyPrefix}-{i}", colorProperty);
                i++;
            }
        }

        Plugin.Log.LogInfo("Finished loading additional options to character select screen");
        Plugin.Log.LogInfo("Preloaded textures");
        Instance.Preloaded = true;
    }

    public void Cleanup()
    {
        Plugin.Log.LogInfo("Cleaning up");
        CharacterAssetManager.instance?.UnloadNonShared();
        Instance.LocalPlayerSkins = new[] { 1, 1, 1 };
        Instance.RemotePlayerSkins = new[] { 1, 1, 1 };
    }

    private static void MapPlayerTeam(UIHeroSelect.TeamSelectionController controller, int[] colorArr)
    {
        var selection = controller.teamSelection.selections;
        var noNullHeroes = selection.ToList().TrueForAll(x => x.skin != null);
        if (!noNullHeroes)
        {
            return;
        }

        var i = 0;

        foreach (var hero in selection)
        {
            if (hero.skin == null) continue;
            colorArr[i++] = hero.skin.colorID;
            if (hero.skin.colorID < 3) continue;
            hero.skin = TableHeroAssetInfo.instance.GetSkinColorProperty(hero.skin.heroName, hero.skin.skinID, 1);
        }
    }
}

[HarmonyPatch(typeof(TableHeroAssetInfo), nameof(TableHeroAssetInfo.GetRandomSkinColorProperty))]
public class TestHandler
{
    public static void Postfix(string heroName,
        Func<TableHeroAssetInfo.HeroSkinOptionInfo.Skin, bool> skinUnlockedFilter,
        ref DB_SkinColorProperty __result)
    {
        if (__result.colorID > 2)
        {
            __result.colorID = 1;
        }
    }
}

[HarmonyPatch(typeof(UIHeroSelect), nameof(UIHeroSelect.OnShow))]
public class ResetTextureLoaderOnCharacterSelect
{
    public static void Prefix()
    {
        TextureLoader.Instance.Cleanup();
    }
}
