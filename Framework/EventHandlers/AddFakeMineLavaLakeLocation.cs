using MiscThingsExtended.Framework.Patches;
using MiscThingsExtended.Framework.Util;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Extensions;

namespace MiscThingsExtended.Framework.EventHandlers;

[Registrable]
public static class AddFakeMineLavaLakeLocation
{
    public static void Register(IModHelper helper)
    {
        helper.Events.GameLoop.DayStarted += OnDayStarted_RegisterFakeMineLavaPoolLocation;
        helper.Events.GameLoop.DayEnding += OnDayEnding_RemoveFakeLocation;
    }

    /// <summary>
    /// Register fake location for proper fishing handling.
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event args, see <see cref="SaveLoadedEventArgs"/></param>
    private static void OnDayStarted_RegisterFakeMineLavaPoolLocation(object? sender, DayStartedEventArgs e)
    {
        // just some small placeholder map
        var mapPath = Path.Combine("Maps", "LeoTreeHouse");
        var location = new GameLocation(mapPath, MineShaftLavaLakePatch.FakeMineLavaLakeLocation);
        Game1.locations.Add(location);
        Log.Trace($"Loaded {MineShaftLavaLakePatch.FakeMineLavaLakeLocation} to store Mines floor 100 fish data");
    }

    /// <summary>
    /// Remove fake location to stop game from loading fake location from save and displaying warning about unknown location
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event args, see <see cref="DayEndingEventArgs"/></param>
    private static void OnDayEnding_RemoveFakeLocation(object? sender, DayEndingEventArgs e)
    {
        Game1.locations.RemoveWhere(loc => loc.Name == MineShaftLavaLakePatch.FakeMineLavaLakeLocation);
    }
}
