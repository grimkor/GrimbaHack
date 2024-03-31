using System.Collections.Generic;
using System.IO;
using System.Text.Json;
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

    public List<ComboExport> GetCharacterCombos(int heroIndex)
    {
        if (_combos.TryGetValue(TableHero.instance.GetDBHeroData(heroIndex).heroName, out var comboExports))
        {
            return comboExports;
        }

        return new List<ComboExport>();
    }

    public bool CharacterHasCombos(int heroIndex)
    {
        return Instance._combos.ContainsKey(TableHero.instance.GetDBHeroData(heroIndex).heroName);
    }
}
