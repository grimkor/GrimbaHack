using System;
using System.Collections.Generic;
using GrimbaHack.Data;
using HarmonyLib;
using nway.gameplay.level;
using nway.gameplay.ui;
using Random = System.Random;

namespace GrimbaHack.Modules;

public sealed class StageSelectOverride
{
    public static StageSelectOverride Instance { get; private set; }

    static StageSelectOverride()
    {
        Instance = new StageSelectOverride();
    }

    private StageSelectOverride()
    {
    }

    public static Stage Stage = Global.Stages[0];
    public static List<Stage> RandomStages = Global.Stages.FindAll(x => x.Value != "RANDOM");

    public static void SetStage(Stage stage)
    {
        Stage = stage;
    }

    [HarmonyPatch(typeof(SceneLoader), nameof(SceneLoader.RequestSimpleScene))]
    public class PatchRequestSimpleScene
    {
        static bool Prefix(ILoadingContext loadingContext, ref string sceneName, Action<bool> callback)
        {
            if (Stage.Key == StageSelectOverrideOptions.Disabled) return true;
            if (Stage.Key == StageSelectOverrideOptions.Random)
            {
                if (RandomStages.Count > 0)
                {
                    var selection = new Random().Next(0, RandomStages.Count);
                    sceneName = RandomStages[selection].Value;
                }
            }
            else
            {
                sceneName = Stage.Value;
            }

            return true;
        }
    }

    public void UpdateRandomStageSelect(string itemValue, bool value)
    {
        if (value && !RandomStages.Exists(x => x.Value == itemValue))
        {
            var stage = Global.Stages.Find(x => x.Value == itemValue);
            RandomStages.Add(stage);
        }
        else
        {
            RandomStages.Remove(RandomStages.Find(x => x.Value == itemValue));
        }
    }
}