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

public class AssetHeroNameMap
{
    public string folder;
    public string skinOption;
    public string skinColorPropertyPrefix;
    public string colorSuffix;
    public string skinType;
}
public enum PanelTypes
{
    Global,
    BGMPlayer,
    TrainingMode,
    RecordingDummy,
    OnlineTrainingMode,
    Twitch,
    Experimental
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
    public static readonly string Version = "v1.3.0";

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
    
    public static readonly List<AssetHeroNameMap> AssetHeroInfoMapper = new()
    {
        new AssetHeroNameMap() {folder =  "s01_green", skinColorPropertyPrefix = "S01_GreenRanger-Default", colorSuffix = "_color", skinType = "Default", skinOption =  "S01_GreenRanger"  },
        new AssetHeroNameMap() {folder =  "s01_greenv2", skinColorPropertyPrefix = "S01_GreenRanger-GreenV2", colorSuffix = "_color", skinType = "GreenV2", skinOption =  "S01_GreenRanger"  },
        new AssetHeroNameMap() {folder =  "s21_yellow", skinColorPropertyPrefix = "S21_YellowRanger-Default", colorSuffix = "_color", skinType = "Default", skinOption =  "S21_YellowRanger"  },
        new AssetHeroNameMap() {folder =  "s01_red", skinColorPropertyPrefix = "S01_RedRanger-Default", colorSuffix = "_color", skinType = "Default", skinOption =  "S01_RedRanger"  },
        new AssetHeroNameMap() {folder =  "s14_white", skinColorPropertyPrefix = "S14_WhiteRanger-Default", colorSuffix = "_color", skinType = "Default", skinOption =  "S14_WhiteRanger"  },
        new AssetHeroNameMap() {folder =  "s01_redshield", skinColorPropertyPrefix = "S01_RedRanger-RedShield", colorSuffix = "_color", skinType = "RedShield", skinOption =  "S01_RedRanger"  },
        new AssetHeroNameMap() {folder =  "com_rangerslayer", skinColorPropertyPrefix = "COM_RangerSlayer-Default", colorSuffix = "_color", skinType = "Default", skinOption =  "COM_RangerSlayer"  },
        new AssetHeroNameMap() {folder =  "s01_pink", skinColorPropertyPrefix = "COM_RangerSlayer-MMPR", colorSuffix = "_color", skinType = "MMPR", skinOption =  "COM_RangerSlayer"  },
        new AssetHeroNameMap() {folder =  "s07_magnadefender", skinColorPropertyPrefix = "S07_MagnaDefender-Default", colorSuffix = "_color", skinType = "Default", skinOption =  "S07_MagnaDefender"  },
        new AssetHeroNameMap() {folder =  "s01_goldar", skinColorPropertyPrefix = "S01_Goldar-Default", colorSuffix = "_color", skinType = "Default", skinOption =  "S01_Goldar"  },
        new AssetHeroNameMap() {folder =  "com_drakkon", skinColorPropertyPrefix = "COM_Drakkon-Default", colorSuffix = "_color", skinType = "Default", skinOption =  "COM_Drakkon"  },
        new AssetHeroNameMap() {folder =  "com_drakkonV2", skinColorPropertyPrefix = "COM_Drakkon-DrakkonV2", colorSuffix = "_color", skinType = "DrakkonV2", skinOption =  "COM_Drakkon"  },
        new AssetHeroNameMap() {folder =  "s13_kat", skinColorPropertyPrefix = "S13_KatRanger-Default", colorSuffix = "_color", skinType = "Default", skinOption =  "S13_KatRanger"  },
        new AssetHeroNameMap() {folder =  "com_blacksentry", skinColorPropertyPrefix = "COM_BlackSentry-Default", colorSuffix = "_color", skinType = "Default", skinOption =  "COM_BlackSentry"  },
        new AssetHeroNameMap() {folder =  "mov_blue", skinColorPropertyPrefix = "MOV_BlueRanger-Default", colorSuffix = "_color", skinType = "Default", skinOption =  "MOV_BlueRanger"  },
        new AssetHeroNameMap() {folder =  "com_triniblackdragon", skinColorPropertyPrefix = "COM_TriniBlackDragon-Default", colorSuffix = "_color", skinType = "Default", skinOption =  "COM_TriniBlackDragon"  },
        new AssetHeroNameMap() {folder =  "s03_gold", skinColorPropertyPrefix = "S03_GoldRanger-Default", colorSuffix = "_color", skinType = "Default", skinOption =  "S03_GoldRanger"  },
        new AssetHeroNameMap() {folder =  "s01_white", skinColorPropertyPrefix = "S14_WhiteRanger-Default", colorSuffix = "_color", skinType = "Default", skinOption =  "S14_WhiteRanger"  },
        new AssetHeroNameMap() {folder =  "s09_pink", skinColorPropertyPrefix = "S09_PinkRanger-Default", colorSuffix = "_color", skinType = "Default", skinOption =  "S09_PinkRanger"  },
        new AssetHeroNameMap() {folder =  "s02_lordzedd", skinColorPropertyPrefix = "S02_LordZedd-Default", colorSuffix = "_color", skinType = "Default", skinOption =  "S02_LordZedd"  },
        new AssetHeroNameMap() {folder =  "s13_shadow", skinColorPropertyPrefix = "S13_ShadowRanger-Default", colorSuffix = "_color", skinType = "Default", skinOption =  "S13_ShadowRanger"  },
        new AssetHeroNameMap() {folder =  "s13_doggie", skinColorPropertyPrefix = "S13_ShadowRanger-Doggie", colorSuffix = "_C", skinType = "Doggie", skinOption =  "S13_ShadowRanger"  },
        new AssetHeroNameMap() {folder =  "s09_quantum", skinColorPropertyPrefix = "S09_QuantumRanger-Default", colorSuffix = "_color", skinType = "Default", skinOption =  "S09_QuantumRanger"  },
        new AssetHeroNameMap() {folder =  "s16_daishi", skinColorPropertyPrefix = "S16_DaiShi-Default", colorSuffix = "_color", skinType = "Default", skinOption =  "S16_DaiShi"  },
        new AssetHeroNameMap() {folder =  "s16_daishi_phantom", skinColorPropertyPrefix = "S16_DaiShi-Phantom", colorSuffix = "_color", skinType = "Phantom", skinOption =  "S16_DaiShi"  },
        new AssetHeroNameMap() {folder =  "s16_purple", skinColorPropertyPrefix = "S16_PurpleRanger-Default", colorSuffix = "_color", skinType = "Default", skinOption =  "S16_PurpleRanger"  },
        new AssetHeroNameMap() {folder =  "s18_lauren", skinColorPropertyPrefix = "S18_Lauren-Default", colorSuffix = "_color", skinType = "Default", skinOption =  "S18_Lauren"  },
        new AssetHeroNameMap() {folder =  "s01_scorpina", skinColorPropertyPrefix = "S01_Scorpina-Default", colorSuffix = "_color", skinType = "Default", skinOption =  "S01_Scorpina"  },
        new AssetHeroNameMap() {folder =  "sf_ryu", skinColorPropertyPrefix = "SF_RyuRanger-Default", colorSuffix = "_C", skinType = "Default", skinOption =  "SF_RyuRanger"  },
        new AssetHeroNameMap() {folder =  "sf_ryu90s", skinColorPropertyPrefix = "SF_RyuRanger-Ryu90s", colorSuffix = "_C", skinType = "Ryu90s", skinOption =  "SF_RyuRanger"  },
        new AssetHeroNameMap() {folder =  "ryu_H_less", skinColorPropertyPrefix = "SF_RyuRanger-Ryu_Hless", colorSuffix = "_C", skinType = "Ryu_Hless", skinOption =  "SF_RyuRanger"  },
        new AssetHeroNameMap() {folder =  "sf_chunli", skinColorPropertyPrefix = "SF_ChunLiRanger-Default", colorSuffix = "_C", skinType = "Default", skinOption =  "SF_ChunLiRanger"  },
        new AssetHeroNameMap() {folder =  "sf_chunli90s", skinColorPropertyPrefix = "SF_ChunLiRanger-Chunli90s", colorSuffix = "_C", skinType = "Chunli90s", skinOption =  "SF_ChunLiRanger"  },
        new AssetHeroNameMap() {folder =  "NB", skinColorPropertyPrefix = "S01_AdamPark-Default", colorSuffix = "_C", skinType = "Default", skinOption =  "S01_AdamPark"  },
        new AssetHeroNameMap() {folder =  "poisandra", skinColorPropertyPrefix = "S22_Poisandra-Default", colorSuffix = "_C", skinType = "Default", skinOption =  "S22_Poisandra"  },
        new AssetHeroNameMap() {folder =  "s01_rita", skinColorPropertyPrefix = "S01_Rita-Default", colorSuffix = "_color", skinType = "Default", skinOption =  "S01_Rita"  },
        new AssetHeroNameMap() {folder =  "s22_sledge", skinColorPropertyPrefix = "S22_Sledge-Default", colorSuffix = "", skinType = "Default", skinOption =  "S22_Sledge"  },
    };

    public static List<MatchType> BannedGameModes = new()
        { MatchType.RANKED, MatchType.CASUAL, MatchType.LOBBY };

    static public bool IsTrainingMatch()
    {
        return GameManager.instance.appStateManager.state == AppState.Combat &&
               MatchManager.instance.matchType == MatchType.Training;
    }
    static public bool IsPremadeMatch()
    {
        return GameManager.instance.appStateManager.state == AppState.Combat &&
               MatchManager.instance.matchType == MatchType.PREMADE;
    }

    static public bool IsBannedGameMode(MatchType matchType)
    {
        return BannedGameModes.Contains(matchType);
    }
}