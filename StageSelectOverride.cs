using HarmonyLib;
using nway.gameplay.level;
using nway.gameplay.ui;

namespace GrimbaHack;

public class StageSelectOverride
{
    [HarmonyPatch(typeof(SceneLoader), nameof(SceneLoader.RequestSimpleScene))]  
    private class Patch3  
    {  
        static bool Prefix(ILoadingContext loadingContext,  
            ref string sceneName,  
            Il2CppSystem.Action<bool> callback)  
        {        // ARENA_MET = training mode  
            sceneName = "Arena_MET";  
            return true;  
        }}

}