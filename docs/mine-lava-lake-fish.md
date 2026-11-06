This mod allows to add custom fish to floor 100 with Content Patcher, so it supports all its features. To do so, you should
target `Mods/WhyBother11.MTE/MineLavaLakeFish` and load vanilla [fish spawn data](https://stardewvalleywiki.com/Modding:Location_data#Contents), 
as you would otherwise for any location. Here's an example:

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