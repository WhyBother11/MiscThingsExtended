## Map properties

Any map property here should be prepended with `WhyBother11.MTE_`. To not clutter documentation, it is omitted here.

### `Clocks`

Format: `Clocks [<TileX> <TileY> <DeltaX> <DeltaY> <HourHandTexture> <HourHandX> <HourHandY> <HourHandWidth> <HourHandWidthHeight> <MinuteHandTexture> <MinuteHandX> <MinuteHandY> <MinuteHandWidth> <MinuteHandHeight> <Scale>]+`.
Every argument is `int` type, except for `Scale` which has `float` type.

`TileX` and `TileY` represent tile coordinates relative to map corner (i.e. absolute tile position). `DeltaX` and `DeltaY` are used for pixel delta for more precise clock placement. `HourHandTexture`
is a texture path for hour hand, while `HourHandX`, `HourHandY` are used to determine sprite position in the texture. `TextureWidth` and `TextureHeight` are used to specify sprite size.
The same goes for `MinuteHand`, which is used for drawing minute hand. The textures for clock hands are assumed to be pointed up. `Scale` is used to make sprite bigger or smaller.
