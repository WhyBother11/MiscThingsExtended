using MiscThingsExtended.Framework.Util;
using StardewModdingAPI;
using StardewValley.GameData.Locations;

namespace MiscThingsExtended.Framework.Data;

[DataAsset("MineLavaLakeFish")]
public class MineLavaLakeFishDataAsset : DataAsset<SpawnFishData>
{

    public MineLavaLakeFishDataAsset(string path) : base(path) { }
    
    public new static void RegisterAsset(IModHelper helper, string path)
    {
        _instance = new MineLavaLakeFishDataAsset(path);
        DataAsset<SpawnFishData>.RegisterAsset(helper, path);
    }
}
