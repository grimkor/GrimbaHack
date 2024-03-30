using System.Collections.Generic;

namespace GrimbaHack.Modules.Combo;

public class ComboQuickConverter
{
    public static string ConvertGia(string attackName)
    {
        var input = "";
        var cleanName = attackName.Split("_combat_")[1].Replace("_otg", "");
        if (!cleanName.EndsWith("0")) return "";

        if (cleanName.Contains("ex_"))
        {
            input += "L+S";
        }
        else if (cleanName.StartsWith("cn_"))
        {
            if (cleanName.Contains("cn_1"))
            {
                input += "4M";
            }
            else if (cleanName.Contains("cn_2"))
            {
                input += "6H";
            }
            else if (cleanName.Contains("cn_3"))
            {
                input += "6H";
            }
        }
        else if (cleanName.Contains("sp_"))
        {
            if (cleanName.Contains("sp_1"))
            {
                input += "5S";
            }
            else if (cleanName.Contains("sp_2"))
            {
                input += "4S";
            }
            else if (cleanName.Contains("sp_3"))
            {
                input += "6S";
            }
        }

        else if (cleanName.Contains("sup_0"))
        {
            if (cleanName.Contains("high_medium_1_sup_0"))
            {
                input += "5M";
            }
            else if (cleanName.Contains("high_heavy_1_sup_0"))
            {
                input += "5H";
            }
            else if (cleanName.Contains("sp_3"))
            {
                input += "6S";
            }
            else if (cleanName.Contains("high_light_1_sup_0"))
            {
                input += "5L";
            }
        }
        else
        {
            if (cleanName.Contains("high"))
            {
                input += "5";
            }
            else if (cleanName.Contains("low"))
            {
                input += "2";
            }
            else if (cleanName.Contains("jump"))
            {
                input += "9";
            }

            if (cleanName.Contains("light"))
            {
                input += "L";
            }
            else if (cleanName.Contains("medium"))
            {
                input += "M";
            }
            else if (cleanName.Contains("heavy"))
            {
                input += "H";
            }
        }


        return input;
    }

    public static string ConvertEric(string attackName)
    {
        var input = "";
        var cleanName = attackName.Split("_combat_")[1].Replace("_otg", "");
        if (!cleanName.EndsWith("0") && !cleanName.Contains("sp_1") && !cleanName.Contains("sup")) return "";
        if (cleanName.Contains("sp_1_0_hit")) return "";

        if (cleanName.Contains("ex_"))
        {
            input += "L+S";
        }
        else if (cleanName.StartsWith("cn_"))
        {
            if (cleanName.Contains("cn_1"))
            {
                input += "4H";
            }
            else if (cleanName.Contains("cn_2"))
            {
                input += "6H";
            }
            else if (cleanName.Contains("cn_3"))
            {
                input += "6H";
            }
        }
        else if (cleanName.Contains("sp_"))
        {
            if (cleanName.Contains("sp_3_h"))
            {
                input += "H";
            }
            else if (cleanName.Contains("sp_1"))
            {
                input += "5S";
            }
            else if (cleanName.Contains("sp_2"))
            {
                input += "4S";
            }
            else if (cleanName.Contains("sp_3"))
            {
                input += "6S";
            }
        }
        else
        {
            if (cleanName.Contains("high"))
            {
                input += "5";
            }
            else if (cleanName.Contains("low"))
            {
                input += "2";
            }
            else if (cleanName.Contains("jump"))
            {
                input += "9";
            }

            if (cleanName.Contains("light"))
            {
                input += "L";
            }
            else if (cleanName.Contains("medium"))
            {
                input += "M";
            }
            else if (cleanName.Contains("heavy"))
            {
                input += "H";
            }
        }

        return input;
    }
}
