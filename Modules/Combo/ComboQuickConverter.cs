namespace GrimbaHack.Modules.Combo;

public static class ComboQuickConverter
{
    public static string BaseConverter(string attackName)
    {
        if (attackName.Contains("_ex_0"))
        {
            return "L+S";
        }

        if (attackName.Contains("_sup_start"))
        {
            return "H+S";
        }

        if (attackName.Contains("_sp_airspec"))
        {
            return "9S";
        }

        if (attackName.Contains("_sp_1"))
        {
            return "5S";
        }

        if (attackName.Contains("_sp_2"))
        {
            return "4S";
        }

        if (attackName.Contains("_sp_3"))
        {
            return "6S";
        }

        var input = "";
        if (attackName.Contains("_high_"))
        {
            input += "5";
        }
        else if (attackName.Contains("_low_"))
        {
            input += "2";
        }
        else if (attackName.Contains("_jump_"))
        {
            input += "9";
        }

        if (attackName.Contains("_light_"))
        {
            input += "L";
        }
        else if (attackName.Contains("_medium_"))
        {
            input += "M";
        }
        else if (attackName.Contains("_heavy_"))
        {
            input += "H";
        }

        return input;
    }

    public static string ConvertGia(string attackName)
    {
        var input = "";

        // command normals
        switch (attackName)
        {
            case "S21_YellowRanger_combat_cn_1_0":
                return "4M";
            case "S21_YellowRanger_combat_cn_2_0":
                return "4H";
            case "S21_YellowRanger_combat_cn_3_0":
            case "S21_YellowRanger_combat_cn_3_0_otg":
                return "6H";
            case "S21_YellowRanger_combat_cn_1_1":
                return "M";
            case "S21_YellowRanger_combat_cn_1_2":
                return "H";
        }

        if (!attackName.EndsWith("0") && !attackName.EndsWith("otg")) return "";
        input += BaseConverter(attackName);


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
