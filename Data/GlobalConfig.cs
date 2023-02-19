using System.Collections.Generic;
using epoch.db;
using nway.gameplay.match;

namespace GrimbaHack.Data;

public class Stage
{
    public string Value;
    public string Label;
}

public class BGMTrack
{
    public string Label;
    public string Value;
}

public class LabelValue
{
    public string Label;
    public int Value;
}

public enum PanelTypes
{
    Global,
    BGMPlayer,
    TrainingMode,
    RecordingDummy
}

public enum DUMMY_INPUTS
{
    EMPTY = 0,
    RECORD = 4194304,
    EX = 5243152,
    _5S = 1048832,
    _5L = 16,
}

public static class Global
{
    public static readonly string Version = "v0.4.0";

    public static readonly List<Stage> Stages = new()
    {
        new Stage() { Label = "Training", Value = "Arena_MET" },
        new Stage() { Label = "Harwood County", Value = "Arena_WAR" },
        new Stage() { Label = "Mystic Forest", Value = "Arena_FOR" },
        new Stage() { Label = "Throne", Value = "Arena_DRK" },
        new Stage() { Label = "Throne Story", Value = "Arena_DRC" },
        new Stage() { Label = "Earth Ceno Era", Value = "Arena_PRE" },
        new Stage() { Label = "Command Center", Value = "Arena_CMD" },
        new Stage() { Label = "Corinth", Value = "Arena_COR" },
        new Stage() { Label = "Delta Base", Value = "Arena_SPD" },
        new Stage() { Label = "Command Center Story", Value = "Arena_CCC" },
        new Stage() { Label = "Earth Tower", Value = "Arena_TWR" },
        new Stage() { Label = "Random", Value = "RANDOM" },
    };

    public static readonly List<BGMTrack> BGMTracks = new()
    {
        new BGMTrack() { Value = "mus_menu", Label = " Menu music" },
        new BGMTrack() { Value = "mus_menu_lobby", Label = " Lobby Music" },
        new BGMTrack() { Value = "mus_arena_trn", Label = " Training mode" },
        new BGMTrack() { Value = "mus_arena_war_2", Label = " Harwood County" },
        new BGMTrack() { Value = "mus_arena_for", Label = " Mystic Forest" },
        new BGMTrack() { Value = "mus_arena_drk", Label = " Lord Drakkon's Throne" },
        new BGMTrack() { Value = "mus_arena_drk_story", Label = " Lord Drakkon's Throne Story" },
        new BGMTrack() { Value = "mus_arena_pre", Label = " Planet Earth" },
        new BGMTrack() { Value = "mus_arena_cmd", Label = " Command Center" },
        new BGMTrack() { Value = "mus_arena_cor", Label = " Corinth" },
        new BGMTrack() { Value = "mus_arena_spd", Label = " Delta Base" },
        new BGMTrack() { Value = "mus_arena_cmd_fire", Label = " Command Center Story" },
        new BGMTrack() { Value = "mus_arena_twr", Label = " Tower" },
    };

    public static readonly List<LabelValue> PercentOptions = new()
    {
        new LabelValue() { Label = "Random", Value = -1 },
        new LabelValue() { Label = "0%", Value = 0 },
        new LabelValue() { Label = "5%", Value = 5 },
        new LabelValue() { Label = "10%", Value = 10 },
        new LabelValue() { Label = "15%", Value = 15 },
        new LabelValue() { Label = "20%", Value = 20 },
        new LabelValue() { Label = "25%", Value = 25 },
        new LabelValue() { Label = "30%", Value = 30 },
        new LabelValue() { Label = "35%", Value = 35 },
        new LabelValue() { Label = "40%", Value = 40 },
        new LabelValue() { Label = "45%", Value = 45 },
        new LabelValue() { Label = "50%", Value = 50 },
        new LabelValue() { Label = "55%", Value = 55 },
        new LabelValue() { Label = "60%", Value = 60 },
        new LabelValue() { Label = "65%", Value = 65 },
        new LabelValue() { Label = "70%", Value = 70 },
        new LabelValue() { Label = "75%", Value = 75 },
        new LabelValue() { Label = "80%", Value = 80 },
        new LabelValue() { Label = "85%", Value = 85 },
        new LabelValue() { Label = "90%", Value = 90 },
        new LabelValue() { Label = "95%", Value = 95 },
        new LabelValue() { Label = "100%", Value = 100 },
    };

    static public bool isTrainingMatch()
    {
        return GameManager.instance.appStateManager.state == AppState.Combat &&
               MatchManager.instance.matchType == MatchType.Training;
    }
}