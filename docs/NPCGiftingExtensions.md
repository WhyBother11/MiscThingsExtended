## NPC Gifting Extensions
For NPC gifting related extensions, you should target `Mods/WhyBother11.MTE/NPCGiftingExtensionData`, which is a dictionary
where key is NPC internal name, as defined in `Data/Characters`, and the value is model described below. This dictionary
is prepopulated with every currently loaded NPC, so to edit it properly with Content Patcher you have to use `Fields` or `Entries`
with `TargetFields` set.

### NPCGiftingExtensionData

Any present fields are not required.

| Field        | Type                   | Description                                               |
|--------------|------------------------|-----------------------------------------------------------|
| `Extensions` | `List<NPCGiftingData>` | Contains data for applying extensions for any gifted item |

### NPCGiftingData

Any present fields are not required. Actions will be run for every entry with satisfied `Condition`, while override fields
are selected based on `Precedence`.

| Field                      | Type           | Description                                                                                                                                                                                |
|----------------------------|----------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `Condition`                | `string`       | Game state query to check if any following actions and overrides should apply. Uses received by NPC gift as input item, so to check it use `ITEM` related queries. Defaults to `TRUE`.     |
| `Actions`                  | `List<string>` | Actions that should run if `Condition` is satisfied. Those actions are run after friendship points were added.                                                                             |
| `OverrideDialogueLine`     | `string`       | Dialogue key, as defined in `Characters/Dialogue/<GiftedNPC>`, that should be used when NPC receives a gift and `Condition` is satisfied.                                                  |
| `OverrideTaste`            | `string`       | Overrides NPC gifting taste for given gift if `Condition` is satisfied. Should be one of `Hated`, `Disliked`, `Neutral`, `Liked`, `Loved`.                                                 |
| `OverrideFriendshipPoints` | `int`          | Overrides received friendship points for given gift if `Condition` is satified.                                                                                                            |
| `Precedence`               | `int`          | In case `Condition` of multiple `NPCGiftingData` entries are satisfied, entry with highest value is selected for any override field (i.e field starting with `Override`). Defaults to `0`. |

