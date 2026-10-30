using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiscThingsExtended.Framework.Util;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Locations;

namespace MiscThingsExtended.Framework.UI;

[Registrable]
public static class Clocks
{

    private static PerScreen<bool> _active = new (() => false);
    
    private static readonly Dictionary<string, Texture2D> TextureCache = new ();
    
    private static PerScreen<List<ParsedClockTileProperty>> _parsedClockTileProperties = new ();

    private static float _hourAngle;
    private static float _minuteAngle;

    public static void Register(IModHelper helper)
    {
        helper.Events.Player.Warped += OnWarped_ParseClockPropertiesAndStartDrawing;
    }

    private static void CalculateAngles()
    {
        var minutes = Game1.timeOfDay - Game1.timeOfDay / 100 * 100 + 
                      (float) Game1.gameTimeInterval / Game1.realMilliSecondsPerGameTenMinutes * 10;
        var hours = (Game1.timeOfDay - Game1.timeOfDay / 1200 * 1200) / 100 + minutes / 60;
        _hourAngle = 2 * MathF.PI * hours / 12;
        _minuteAngle = 2 * MathF.PI * minutes / 60;
    }

    private static void OnWarped_ParseClockPropertiesAndStartDrawing(object? sender, WarpedEventArgs e)
    {
        if (!e.IsLocalPlayer) return;
        if (_active.Value)
        {
            // warped away
            _active.Value = false;
            _parsedClockTileProperties.Value.Clear();
            Helper.Events.Display.RenderedWorld -= OnRenderedWorld_DrawClockHands;
            Helper.Events.GameLoop.UpdateTicked -= OnUpdateTicked_CalculateAngles;
            return;
        }

        if (!e.NewLocation.Map.Properties.TryGetValue("Clocks".PrependId(), out string clockProperties)) 
            return;
        _active.Value = true;
        _parsedClockTileProperties.Value = ParseClockTileProperty(clockProperties);
        CalculateAngles();
        Helper.Events.Display.RenderedWorld += OnRenderedWorld_DrawClockHands;
        Helper.Events.GameLoop.UpdateTicked += OnUpdateTicked_CalculateAngles;
    }

    private static void OnUpdateTicked_CalculateAngles(object? sender, UpdateTickedEventArgs e)
    {
        // looks good enough
        if (!e.IsMultipleOf(2)) return;
        CalculateAngles();
    }

    private static void OnRenderedWorld_DrawClockHands(object? sender, RenderedWorldEventArgs e)
    {
        if (!_parsedClockTileProperties.Value.Any()) return;
        _parsedClockTileProperties.Value.ForEach(clock =>
        {
            e.SpriteBatch.Draw(
                texture: clock.MinuteHandTexture,
                position: new Vector2(clock.X * Game1.tileSize + clock.DeltaX - Game1.viewport.X,
                    clock.Y * Game1.tileSize + clock.DeltaY - Game1.viewport.Y),
                sourceRectangle: new Rectangle(clock.MinuteHandX, clock.MinuteHandY, clock.MinuteHandWidth, clock.MinuteHandHeight),
                color: Color.White,
                rotation: _minuteAngle - MathF.PI,
                effects: SpriteEffects.FlipVertically,
                scale: clock.Scale,
                origin: new Vector2(clock.MinuteHandWidth / 2f, 0),
                layerDepth: 0
            );
            e.SpriteBatch.Draw(
                texture: clock.HourHandTexture,
                position: new Vector2(clock.X * Game1.tileSize + clock.DeltaX - Game1.viewport.X,
                    clock.Y * Game1.tileSize + clock.DeltaY - Game1.viewport.Y),
                sourceRectangle: new Rectangle(clock.HourHandX, clock.HourHandY, clock.HourHandWidth, clock.HourHandHeight),
                color: Color.White,
                rotation: _hourAngle - MathF.PI,
                effects: SpriteEffects.FlipVertically,
                scale: clock.Scale,
                origin: new Vector2(clock.HourHandWidth / 2f, 0),
                layerDepth: 0
            );
        });
    }

    private static List<ParsedClockTileProperty> ParseClockTileProperty(string property)
    {
        var result = new List<ParsedClockTileProperty>();
        var propertyParts = property.Split(' ');
        for (var i = 0; i < propertyParts.Length; i+=15)
        {
            var hourHandTexture = Helper.GameContent.Load<Texture2D>(propertyParts[i + 4]);
            TextureCache[hourHandTexture.Name] = hourHandTexture;
            var minuteHandTexture = Helper.GameContent.Load<Texture2D>(propertyParts[i + 9]);
            TextureCache[minuteHandTexture.Name] = minuteHandTexture;
            result.Add(new ParsedClockTileProperty
            {
                X = int.Parse(propertyParts[i]),
                Y = int.Parse(propertyParts[i + 1]),
                DeltaX = int.Parse(propertyParts[i + 2]),
                DeltaY = int.Parse(propertyParts[i + 3]),
                HourHandTexture = hourHandTexture,
                HourHandX = int.Parse(propertyParts[i + 5]),
                HourHandY = int.Parse(propertyParts[i + 6]),
                HourHandWidth = int.Parse(propertyParts[i + 7]),
                HourHandHeight = int.Parse(propertyParts[i + 8]),
                MinuteHandTexture = minuteHandTexture,
                MinuteHandX = int.Parse(propertyParts[i + 10]),
                MinuteHandY = int.Parse(propertyParts[i + 11]),
                MinuteHandWidth = int.Parse(propertyParts[i + 12]),
                MinuteHandHeight = int.Parse(propertyParts[i + 13]),
                Scale = float.Parse(propertyParts[i + 14]),
            });
        }
        Log.Log($"{propertyParts.Length} clocks", LogLevel.Error);
        return result;
    }

    private class ParsedClockTileProperty
    {

        public int X;
        
        public int Y;
        
        public int DeltaX;
        
        public int DeltaY;

        public Texture2D? HourHandTexture;

        public int HourHandX;

        public int HourHandY;

        public int HourHandWidth;

        public int HourHandHeight;
        
        public Texture2D? MinuteHandTexture;
        
        public int MinuteHandX;

        public int MinuteHandY;

        public int MinuteHandWidth;

        public int MinuteHandHeight;

        public float Scale;
    }
}
