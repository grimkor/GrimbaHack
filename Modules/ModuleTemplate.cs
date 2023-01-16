namespace GrimbaHack.Modules;

public sealed class ModuleTemplate : ModuleBase
{
    private ModuleTemplate() {}
    
    public static ModuleTemplate Instance { get; private set; }
    
    static ModuleTemplate()
    {
        Instance = new ModuleTemplate();
    }
}