using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using BepInEx;
using GrimbaHack.Data;
using GrimbaHack.UI.TrainingMode;

namespace GrimbaHack.Modules.ComboTrial;

public class ComboTrialDataManager
{
    private bool _loaded;
    public static ComboTrialDataManager Instance = new();

    private ComboTrialDataManager()
    {
    }

    public void Init()
    {
        Instance.LoadData();
        Instance.CreateFolders();
    }

    private readonly Dictionary<string, List<ComboExport>> _combos = new();

    private void LoadData()
    {
        if (_loaded) return;
        var filepath = Path.Join(BepInEx.Paths.GameRootPath, "output", "combo-trials.json");
        var fileContents = File.ReadAllText(filepath);
        var options = new JsonSerializerOptions { IncludeFields = true };
        var json = JsonSerializer.Deserialize<List<ComboExport>>(fileContents, options);
        for (int i = 1; i < 21; i++)
        {
            var hero = TableHero.instance.GetDBHeroData(i);
            if (hero != null)
            {
                _combos.Add(hero.heroName, json);
            }
        }

        _loaded = true;
    }

    public static List<ComboExport> GetCharacterCombos(int heroIndex)
    {
        var comboExports = new List<ComboExport>();
        if (CharacterHasCombos(heroIndex))
        {
            var options = new JsonSerializerOptions { IncludeFields = true };
            foreach (var file in Directory.GetFiles(Path.Join(FolderPathFromHeroId(heroIndex))))
            {
                var contents = File.ReadAllBytes(file);
                try
                {
                    var combo = JsonSerializer.Deserialize<ComboExport>(contents, options);
                    comboExports.Add(combo);
                }
                catch (Exception e)
                {
                    Plugin.Log.LogError($"Error parsing{file}: {e.Message}");
                }
            }
            return comboExports;
        }
        comboExports.Sort((a,b) => string.Compare(a.Title, b.Title, StringComparison.Ordinal));
        return comboExports;
    }

    public static bool CharacterHasCombos(int heroIndex)
    {
        var folder = Directory.GetFiles(FolderPathFromHeroId(heroIndex)).ToList();
        return folder.ToList().Count > 0;
    }

    private static string GetHeroNameFromId(int heroIndex)
    {
        return TableHero.instance.GetDBHeroData(heroIndex).heroName;
    }

    private static string FolderPathFromHeroId(int heroIndex)
    {
      return Path.Join(Paths.PluginPath, "combos", GetHeroNameFromId(heroIndex));
    }

    private void CreateFolders()
    {
        foreach (var hero in Global.AssetHeroInfoMapper)
        {
            if (!hero.hasCombos) continue;
            if (!Directory.Exists(Path.Join(Paths.PluginPath, "combos")))
            {
                Directory.CreateDirectory(Path.Join(Paths.PluginPath, "combos"));
            }
            if (!Directory.Exists(Path.Join(Paths.PluginPath, "combos", hero.skinOption)))
            {
                Directory.CreateDirectory(Path.Join(Paths.PluginPath, "combos", hero.skinOption));
            }
        }
    }
}
