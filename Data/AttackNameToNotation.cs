using System.Collections.Generic;

namespace GrimbaHack.Data;

public static class AttackNameToNotation
{
    public static Dictionary<int, Dictionary<string, string>> Data = new()
    {
        // S01_GreenRanger
        {
            1, new()
            {
                { "S01_GreenRanger_combat_cn_1_0", "6M" },
                { "S01_GreenRanger_combat_cn_2_0", "4M" },
                { "S01_GreenRanger_combat_cn_2_1_otg", "4M" }
            }
        },
        // S21_YellowRanger
        {
            2, new()
            {
                { "S21_YellowRanger_combat_cn_1_0", "4M" },
                { "S21_YellowRanger_combat_cn_1_1", "M" },
                { "S21_YellowRanger_combat_cn_1_2", "H" },
                { "S21_YellowRanger_combat_cn_2_0", "4H" },
                { "S21_YellowRanger_combat_cn_3_0", "6H" },
                { "S21_YellowRanger_combat_cn_3_0_otg", "6H" }
            }
        },
        // S01_RedRanger
        {
            3, new()
            {
                { "S01_RedRanger_combat_sp_1_1_counterattack", "5S" },
                { "S01_RedRanger_combat_sp_3_1", "S" },
                { "S01_RedRanger_combat_sp_3_2", "S" },
            }
        },
        // S14_WhiteRanger
        {
            4, new()
            {
                { "S01_GreenRanger_combat_cn_1_0", "6M" },
                { "S01_GreenRanger_combat_cn_2_0", "4M" },
                { "S01_GreenRanger_combat_cn_2_1_otg", "4M" },
            }
        },
        // COM_RangerSlayer
        {
            5, new()
            {
                { "COM_RangerSlayer_combat_cn_1_0", "4M" },
                { "COM_RangerSlayer_combat_cn_2_0", "6M" },
                { "COM_RangerSlayer_combat_sp_3_l_nock_1", "L" },
                { "COM_RangerSlayer_combat_sp_3_m_nock_0", "M" },
                { "COM_RangerSlayer_combat_sp_3_h_kick_nock_1", "H" },
                { "COM_RangerSlayer_combat_sp_airspec_nock_1", "M" },
                { "COM_RangerSlayer_combat_high_medium_1_nock_0", "M" },
                { "COM_RangerSlayer_combat_cn_1_nock_0", "4M" },
                { "COM_RangerSlayer_combat_cn_2_nock_0", "6M" },
                { "COM_RangerSlayer_combat_low_medium_0_otg", "2M" }
            }
        },
        // S07_MagnaDefender
        {
            6, new()
            {
                { "S07_MagnaDefender_combat_cn_1_0", "4M" },
                { "S07_MagnaDefender_combat_cn_2_0", "4H" },
                { "S07_MagnaDefender_combat_cn_2_0_otg", "4H" },
                { "S07_MagnaDefender_combat_sp_airspec_3_otg", "9S" }
            }
        },
        // S01_Goldar
        {
            7, new()
            {
                { "S01_Goldar_combat_cn_1_0", "4M" },
                { "S01_Goldar_combat_cn_2_1_otg", "6M" },
                { "S01_Goldar_combat_cn_2_0", "6M" },
                { "S01_Goldar_combat_cn_3_push", "4H" },
                { "S01_Goldar_combat_sp_2_0", "4S" },
                { "S01_Goldar_combat_sp_1_1_proj", "5S" },
                { "S01_Goldar_combat_sp_1_0", "5S" },
                { "S01_Goldar_combat_sp_3_0", "6S" }
            }
        },
        // COM_Drakkon
        {
            8, new()
            {
                { "COM_Drakkon_combat_sp_3_m_0", "6S~M" },
                { "COM_Drakkon_combat_sp_3_h_0", "6S~H" },
                { "COM_Drakkon_combat_sp_3_m_0_otg", "6S~M" },
                { "COM_Drakkon_combat_sp_airspec_0_otg", "9S" }
            }
        },
        // S13_KatRanger
        {
            9, new()
            {
                { "S13_KatRanger_combat_cn_1_0", "4M" },
                { "S13_KatRanger_combat_cn_2_0", "6H" },
                { "S13_KatRanger_combat_sp_3_1_otg", "6S" },
                { "S13_KatRanger_combat_sp_airspec_0_otg", "9S" }
            }
        },
        // COM_BlackSentry
        {
            10, new()
            {
                { "COM_BlackSentry_combat_cn_1_0", "4M" },
                { "COM_BlackSentry_combat_high_heavy_1_0_otg", "5H" },
                { "COM_BlackSentry_combat_sp_1_ex_1", "L+S" }
            }
        },
        // MOV_BlueRanger
        {
            11, new()
            {
                { "MOV_Blue_combat_sp_1_b", "5S" },
                { "MOV_Blue_combat_sp_2_pre_proj_1_1", "4S" },
                { "MOV_Blue_combat_sp_1_0", "4S" },
                { "MOV_Blue_combat_sp_3_b_0", "6S" },
                { "MOV_Blue_combat_sp_3_1_0", "6S" },
                { "MOV_Blue_combat_sp_3_2_0", "6S" },
                { "MOV_Blue_combat_sp_3_3_0", "6S" }
            }
        },
        // COM_TriniBlackDragon
        {
            12, new()
            {
            }
        },
        // S03_GoldRanger
        {
            13, new()
            {
                { "S03_GoldRanger_combat_cn_1_0", "6M" },
                { "S03_GoldRanger_combat_sp_1_0_hit", "5S" },
                { "S03_GoldRanger_combat_low_medium_0_otg", "2M" }
            }
        },
        // S09_PinkRanger
        {
            14, new()
            {
                { "S09_PinkRanger_combat_cn_1_0", "6H" },
                { "S09_PinkRanger_combat_sp_3_0", "6SS" }
            }
        },
        // S02_LordZedd
        {
            15, new()
            {
                { "S02_LordZedd_combat_cn_1_0", "6H" },
                { "S02_LordZedd_combat_cn_1_0_otg", "6H" },
                { "S02_LordZedd_combat_sp_3_1", "6S" }
            }
        },
        // S13_ShadowRanger
        {
            16, new()
            {
                { "S13_ShadowRanger_combat_cn_1_0", "4H" },
                { "S13_ShadowRanger_combat_sp_1_l_0", "5S~L" },
                { "S13_ShadowRanger_combat_sp_1_m_0", "5S~M" },
                { "S13_ShadowRanger_combat_sp_1_h_1", "5S~H" },
                { "S13_ShadowRanger_combat_sup_start_0", "H+S" },
                { "S13_ShadowRanger_combat_sp_2_0_0_otg", "4S" }
            }
        },
        // S09_QuantumRanger
        {
            17, new()
            {
                { "S09_QuantumRanger_combat_cn_1_0", "4H" },
                { "S09_QuantumRanger_combat_cn_2_0", "6H" },
                { "S09_QuantumRanger_combat_cn_2_0_otg", "6H" },
                { "S09_QuantumRanger_combat_high_light_1_sup_0", "5L" },
                { "S09_QuantumRanger_combat_high_medium_1_sup_0", "5M" },
                { "S09_QuantumRanger_combat_high_heavy_1_sup_0", "5H" },
                { "S09_QuantumRanger_combat_low_heavy_sup_0", "2H" },
                { "S09_QuantumRanger_combat_jump_heavy_sup_0", "9H" },
                { "S09_QuantumRanger_combat_sp_2_b_sup_0", "9S" },
                { "S09_QuantumRanger_combat_sp_2_c_sup_0", "S" },
                { "S09_QuantumRanger_combat_sp_1_sup_0", "5S" },
                { "S09_QuantumRanger_combat_sp_3_sup_5", "6S" },
                { "S09_QuantumRanger_combat_sp_ex_sup_0", "L+S" }
            }
        },
        // S16_DaiShi
        {
            18, new()
            {
                { "S16_DaiShi_combat_cn_1_0", "4H" },
                { "S16_DaiShi_combat_sp_1_l_0", "5S~L" },
                { "S16_DaiShi_combat_sp_1_m_1", "5S~M" },
                { "S16_DaiShi_combat_sp_1_h_0", "5S~H" },
                { "S16_DaiShi_combat_sp_3_l", "6S~L" },
                { "S16_DaiShi_combat_sp_3_m", "6S~M" },
                { "S16_DaiShi_combat_sp_3_h", "6S~H" },
                { "S16_DaiShi_combat_sp_2_wall_l", "4S~L" },
                { "S16_DaiShi_combat_sp_2_wall_m", "4S~M" },
                { "S16_DaiShi_combat_sp_3_slide", "4S~6S" }
            }
        },
        // S16_PurpleRanger
        {
            19, new()
            {
                { "S16_PurpleRanger_combat_high_light_2_0_otg", "5LL" },
                { "S16_PurpleRanger_combat_low_medium_0_otg", "2M" },
                { "S16_PurpleRanger_combat_sp_ex_1_proj_0", "L+M" },
                { "S16_PurpleRanger_combat_sp_airspec_0_otg", "9S" },
                { "S16_PurpleRanger_combat_sp_1_a_0", "4S" },
                { "S16_PurpleRanger_combat_sp_1_b_0", "6S" },
                { "S16_PurpleRanger_combat_sp_2_b_2", "4S" }
            }
        },
        // S18_Lauren
        {
            20, new()
            {
                { "S18_Lauren_combat_cn_1_0", "64L" },
                { "S18_Lauren_combat_cn_2_0", "46H" },
                { "S18_Lauren_combat_sp_1_a_1", "[5S]~4" },
                { "S18_Lauren_combat_sp_1_b_1", "[5S]~6" },
                { "S18_Lauren_combat_sp_3_a_1", "6S>2H" },
                { "S18_Lauren_combat_sup_start_0", "L+S" }
            }
        },
        // S01_Scorpina
        {
            21, new()
            {
                { "S01_Scorpina_combat_sp_5_0", "L+S" },
                { "S01_Scorpina_combat_sp_ex_1", "L+S" },
                { "S01_Scorpina_combat_sup_start_0_beam_0", "H+S" },
                { "S01_Scorpina_combat_high_heavy_2_0_otg", "H" }
            }
        },
        // SF_RyuRanger
        {
            22, new()
            {
                { "SF_RyuRanger_combat_high_special_0", "5S" },
                { "SF_RyuRanger_combat_sp_1_0", "236H" },
                { "SF_RyuRanger_combat_sp_2_l_0", "623L" },
                { "SF_RyuRanger_combat_sp_2_m_0", "623M" },
                { "SF_RyuRanger_combat_sp_2_h_0", "623H" },
                { "SF_RyuRanger_combat_sp_3_l_0", "214L" },
                { "SF_RyuRanger_combat_sp_3_m_0", "214M" },
                { "SF_RyuRanger_combat_sp_3_h_0", "214H" },
                { "SF_RyuRanger_combat_sp_5_0", "~214H" },
                { "SF_RyuRanger_combat_cn_1_0", "6M" },
                { "SF_RyuRanger_combat_cn_1_1_otg", "6M" },
                { "SF_RyuRanger_combat_sp_1_ex_0", "236L+M" },
                { "SF_RyuRanger_combat_sp_2_ex_0", "623L+M" },
                { "SF_RyuRanger_combat_sp_3_ex_0", "214L+M" },
                { "SF_RyuRanger_combat_sup_start_0_beam_0", "L+S" }
            }
        },
        // SF_ChunLiRanger
        {
            23, new()
            {
                { "SF_ChunLiRanger_combat_sp_1_0", "[4]~6H" },
                { "SF_ChunLiRanger_combat_sp_1_alt", "[4]~6HH" },
                { "SF_ChunLiRanger_combat_sp_1_ex_0", "[4]~6L+M" },
                { "SF_ChunLiRanger_combat_sp_2_l_0", "236L" },
                { "SF_ChunLiRanger_combat_sp_2_m_0", "236M" },
                { "SF_ChunLiRanger_combat_sp_2_h_0", "236H" },
                { "SF_ChunLiRanger_combat_sp_2_alt", "H" },
                { "SF_ChunLiRanger_combat_sp_2_air_l_0", "236L" },
                { "SF_ChunLiRanger_combat_sp_2_air_m_0", "236M" },
                { "SF_ChunLiRanger_combat_sp_2_air_h_0", "236H" },
                { "SF_ChunLiRanger_combat_sp_3_l_0", "[2]~8L" },
                { "SF_ChunLiRanger_combat_sp_3_m_0", "[2]~8M" },
                { "SF_ChunLiRanger_combat_sp_3_h_0", "[2]~8H" },
                { "SF_ChunLiRanger_combat_sp_3_alt", "H" },
                { "SF_ChunLiRanger_combat_sp_3_ex_1", "[2]~8L+M" },
                { "SF_ChunLiRanger_combat_jump_cn_1_0_otg", "2M" },
                { "SF_ChunLiRanger_combat_cn_1_0", "6M" }
            }
        },
        {
            // S01_AdamPark
            24, new()
            {
                { "S01_AdamPark_combat_sp_3_l_0", "6S~L" },
                { "S01_AdamPark_combat_sp_3_m_0", "6S~M" },
                { "S01_AdamPark_combat_sp_3_h_0", "6S~H" },
                { "S01_AdamPark_combat_sp_3_h_0_otg", "6S~H" },
                { "S01_AdamPark_combat_sp_1_0", "5S~L" },
                { "S01_AdamPark_combat_sp_2_m_0", "4S~M" },
                { "S01_AdamPark_combat_sp_2_h_0", "4S~H" },
            }
        },
        {
            // S22_Poisandra
            25, new()
            {
                { "S22_Poisandra_combat_cn_1", "6M" },
                { "S22_Poisandra_combat_cn_1_otg", "6M" },
                { "S22_Poisandra_combat_cn_2_1", "6H" },
                { "S22_Poisandra_combat_high_heavy_3_1", "H" },
                { "S22_Poisandra_combat_high_heavy_3_2", "6H" },
                { "S22_Sledge_combat_sp_1_sledge_gunfire_0", "5[S]" },
                { "S22_Sledge_combat_sp_1_sledge_run", "5[S]~6" },
                { "S22_Sledge_combat_sp_1_sledge_fly_end", "5[S]~4" }
            }
        },
        {
            // S01_Rita
            26, new()
            {
                { "S01_Rita_combat_sp_airspec_0_otg", "9S" },
                { "S01_Rita_combat_sp_3_putty_punch", "6S" },
                { "S01_Rita_combat_sp_2_0_b_1", "S" }
            }
        }
    };
}
