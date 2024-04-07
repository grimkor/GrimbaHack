using System.Linq;
using GrimbaHack.Utility;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GrimbaHack.Modules;

public class MemoryLeakFix
{
    public bool Enabled { get; set; }

    private MemoryLeakFix()
    {
    }

    public static MemoryLeakFix Instance { get; private set; }

    static MemoryLeakFix()
    {
        Instance = new MemoryLeakFix();
        Instance.Enabled = Plugin.EXPERIMENTAL_MemoryFix.Value;
        OnEnterMatchActionHandler.Instance.AddPrefixCallback((AppState state) =>
        {
            if (!Instance.Enabled)
            {
                return;
            }

            var go = Object.FindObjectsOfType<Material>();
            foreach (var material in go)
            {
                if (!material.name.Contains("vfx_m"))
                {
                    continue;
                }

                Object.Destroy(material);
            }

            // This has the issue that it removes the EX flash, turns out that might also be a leak
            // or it tidies up properly between loads and needs a rule to skip to not remove it wrongly
            // go
            //     .GroupBy(x => x.name)
            //     .Where(grouping => grouping.Count() > 20)
            //     .ToList()
            //     .ForEach(duplicate =>
            //     {
            //         foreach (var material in duplicate)
            //         {
            //             Object.Destroy(material);
            //         }
            //     });
        });
    }
}
