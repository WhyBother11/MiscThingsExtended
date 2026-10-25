global using static MiscThingsExtended.ModBindings;
global using SUtility = StardewValley.Utility;
global using SObject = StardewValley.Object;
using StardewModdingAPI;

namespace MiscThingsExtended;

public static class ModBindings
{
    public static IModHelper Helper => ModEntry.Instance.Helper;
    
    public static IManifest Manifest => ModEntry.Instance.ModManifest;
    
    public static IMonitor Log => ModEntry.Instance.Monitor;
    
    public static string Id => ModEntry.Instance.ModManifest.UniqueID;
}