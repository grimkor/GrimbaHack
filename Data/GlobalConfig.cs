using System.Collections.Generic;

namespace GrimbaHack.Data;

public class Stage
{
    public string Value;
    public string Label;
}

public class BGMTrack
{
    public string Value;
    public string Label;
}

public enum PanelTypes
{
    Global,
    BGMPlayer,
    TrainingMode
}

public static class Global
{
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
}