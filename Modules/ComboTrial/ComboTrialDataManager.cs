using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using BepInEx;
using GrimbaHack.Data;
using GrimbaHack.Modules.ComboTrial.UI;
using GrimbaHack.UI.TrainingMode;

namespace GrimbaHack.Modules.ComboTrial;

public class ComboTrialDataManager
{
    public static ComboTrialDataManager Instance = new();

    private ComboTrialDataManager()
    {
    }

    public void Init()
    {
        Instance.CreateFolders();
        Instance.Convert();
    }

    private void Convert()
    {
        foreach (var assetHeroNameMap in Global.AssetHeroInfoMapper.Where(character => character.hasCombos).ToList())
        {
            var options = new JsonSerializerOptions
                { IncludeFields = true, AllowTrailingCommas = true, WriteIndented = true };
            foreach (var file in Directory.GetFiles(Path.Join(Paths.PluginPath, "combos", assetHeroNameMap.skinOption)))
            {
                try
                {
                    var contents = File.ReadAllText(file, Encoding.UTF8);
                    if (!contents.Contains("Ids")) continue;
                    var combo = JsonSerializer.Deserialize<ComboExportOld>(contents, options);
                    var newCombo = new ComboExport
                    {
                        Title = combo.Title,
                        Character = combo.Character,
                        Dummy = combo.Dummy,
                        SuperMeter = combo.SuperMeter,
                        MzMeter = combo.MzMeter,
                        CharacterId = combo.CharacterId,
                        DummyId = combo.DummyId,
                        PlayerPosition = combo.PlayerPosition,
                        DummyPosition = combo.DummyPosition,
                        Inputs = combo.Inputs,
                    };
                    foreach (var oldRow in combo.Combo)
                    {
                        var newRow = new List<ComboItem>();
                        foreach (var item in oldRow)
                        {
                            var newItem = new ComboItem();
                            for (int i = 0; i < item.Ids.Count; i++)
                            {
                                newItem.Items.Add(new() { item.Ids[i], item.Notation[i] });
                            }

                            newRow.Add(newItem);
                        }

                        newCombo.Combo.Add(newRow);
                    }

                    var serialised = JsonSerializer.Serialize(newCombo, options);
                    File.WriteAllText(file, serialised);
                }
                catch (Exception e)
                {
                    Plugin.Log.LogError($"Error parsing{file}: {e.Message}");
                }
            }
        }
    }


    public static List<ComboExport> GetCharacterCombos(int heroIndex)
    {
        var comboExports = new List<ComboExport>();
        if (CharacterHasCombos(heroIndex))
        {
            var options = new JsonSerializerOptions { IncludeFields = true, AllowTrailingCommas = true };
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

        if (!Directory.Exists(Path.Join(Paths.PluginPath, "combos", "__EXPORT")))
        {
            Directory.CreateDirectory(Path.Join(Paths.PluginPath, "combos", "__EXPORT"));
        }
    }
}
