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
            Plugin.Log.LogInfo("Running Memory Leak Fix");
            var go = Object.FindObjectsOfType<Material>();
            go
                .GroupBy(x => x.name)
                .Where(grouping => grouping.Count() > 20)
                .ToList()
                .ForEach(duplicate =>
                {
                    foreach (var material in duplicate)
                    {
                        Object.Destroy(material);
                    }
                });
        });
    }
}
