namespace GrimbaHack.Modules.Combo;

public static class ComboQuickConverter
{
    public static string ConvertInput(string attackName, int heroIndex)
    {
        if (Data.AttackNameToNotation.Data[heroIndex].TryGetValue(attackName, out var notation))
        {
            return notation;
        }
        else
            return BaseConverter(attackName);
    }

    private static string BaseConverter(string attackName)
    {
        // Poisandra sucks
        attackName = attackName.Replace("_far", "").Replace("_near", "");
        if (!attackName.EndsWith("0") && !attackName.EndsWith("otg")) return "";
        if (attackName.Contains("_ex_0"))
        {
            return "L+S";
        }

        if (attackName.Contains("_sup_start"))
        {
            return "H+S";
        }

        if (attackName.Contains("_throw_start_0"))
        {
            return "M+H";
        }

        if (attackName.Contains("_throw_back_start_0"))
        {
            return "4M+H";
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
}
