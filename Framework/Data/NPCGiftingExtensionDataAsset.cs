using Microsoft.Xna.Framework.Content;
using MiscThingsExtended.Framework.Util;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace MiscThingsExtended.Framework.Data;

public class NPCGiftingExtensionData
{
    [ContentSerializer(Optional = true)]
    public List<NPCGiftingData> Extensions = new ();
        
    public class NPCGiftingData
    {
        public string Condition = "TRUE";

        [ContentSerializer(Optional = true)]
        public List<string> Actions = new ();

        [ContentSerializer(Optional = true)]
        public string? OverrideDialogueLine;

        [ContentSerializer(Optional = true)]
        public string? OverrideTaste;

        [ContentSerializer(Optional = true)] 
        public int? OverrideFriendshipPoints;

        [ContentSerializer(Optional = true)] 
        public bool BypassGiftLimit = false;
        
        [ContentSerializer(Optional = true)] 
        public int Precedence = 0;
    }
}

[DataAsset("NPCGiftingExtensionData")]
public class NPCGiftingExtensionDataAsset : DataAsset<NPCGiftingExtensionData>
{
    
    private NPCGiftingExtensionDataAsset(string path) : base(path) { }

    public new static void RegisterAsset(IModHelper helper, string path)
    {
        _instance = new NPCGiftingExtensionDataAsset(path);
        DataAsset<NPCGiftingExtensionData>.RegisterAsset(helper, path);
    }
    
    protected override void Load(AssetRequestedEventArgs e)
    {
        var data = Game1.characterData.ToDictionary(kvp => kvp.Key, _ => new NPCGiftingExtensionData());
        Log.Log($"Loaded {data.Keys.Count} characters: {string.Join(", ", data.Keys)}");
        e.LoadFrom(() => data, AssetLoadPriority.Exclusive);
    }

    protected override void OnReady()
    {
        foreach (var (_, extensionData) in Data)
        {
            extensionData.Extensions.Sort((f, s) =>
            {
                if (f.Precedence < s.Precedence) return 1;
                if (f.Precedence > s.Precedence) return -1;
                return 0;
            });
        }
    }
}
