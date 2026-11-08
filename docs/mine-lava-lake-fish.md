This mod allows to add custom fish to floor 100 (untested, but should also work for floors 80-120 if there happens to be a fishable water) with Content Patcher, so it supports all its features. To do so, you should
target `Mods/WhyBother11.MTE/MineLavaLakeFish` and load vanilla [fish spawn data](https://stardewvalleywiki.com/Modding:Location_data#Contents), 
as you would otherwise for any location.

Keep in mind that there are some minor limitations for mod compatibility caused by floor 100 fish logic being hardcoded:

- [Lookup Anything](https://www.nexusmods.com/stardewvalley/mods/541), [Fishing Log](https://www.nexusmods.com/stardewvalley/mods/50188) and [Fish Helper UI](https://www.nexusmods.com/stardewvalley/mods/33167) will not recognize added fish as the floor 100 fish. They may display fish location as `WhyBother11.MTE_MineLavaLake` as it is a fake location used to delegate fishing logic.
- [Fishing Info Overlays](https://www.nexusmods.com/stardewvalley/mods/8970) will show the fish only if its config value `BarExtraCheckFrequency` is > 0, as in this case it simulates fishing, not just takes location data which is not applicable to floor 100.

However, [Visible Fish](https://www.nexusmods.com/stardewvalley/mods/8897) will show fish, as it simulates fishing.

Here's an example of adding fish to floor 100:

```json
{
    "Action": "EditData",
    "Target": "Mods/WhyBother11.MTE/MineLavaLakeFish",
    "Entries": {
        "{{ModID}}_Example": {
            "Id": "{{ModID}}_Example",
            "ItemId": "(O){{ModID}}_Example"
        }
    }
}
```
