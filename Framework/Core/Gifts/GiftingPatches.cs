using System.Reflection.Emit;
using HarmonyLib;
using MiscThingsExtended.Framework.Data;
using MiscThingsExtended.Framework.Util;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Triggers;

namespace MiscThingsExtended.Framework.Core.Gifts;

[HarmonyPatch]
public static class GiftingPatches
{

    [HarmonyPatch(typeof(NPC), nameof(NPC.getGiftTasteForThisItem))]
    [HarmonyPrefix]
    private static bool NPC_getGiftTasteForThisItem_Postfix(NPC __instance, Item item, ref int __result)
    {
        if (!NPCGiftingExtensionDataAsset.Data.TryGetValue(__instance.Name, out var data)) return true;
        foreach (var npcGiftingData in data.Extensions)
        {
            if (npcGiftingData.OverrideTaste is not null && 
                GameStateQuery.CheckConditions(npcGiftingData.Condition, __instance.currentLocation, Game1.player, null, item))
            {
                __result = npcGiftingData.OverrideTaste switch
                {
                    "Hated" => NPC.gift_taste_hate,
                    "Disliked" => NPC.gift_taste_dislike,
                    "Neutral" => NPC.gift_taste_neutral,
                    "Liked" => NPC.gift_taste_like,
                    "Loved" => NPC.gift_taste_love,
                    _ => __result
                };
                return false;
            }
        }

        return true;
    }

    [HarmonyPatch(typeof(NPC), nameof(NPC.GetGiftReaction))]
    [HarmonyPrefix]
    private static bool NPC_GetGiftReaction_Prefix(NPC __instance, SObject gift, ref Dialogue __result)
    {
        if (!NPCGiftingExtensionDataAsset.Data.TryGetValue(__instance.Name, out var data)) return true;
        foreach (var npcGiftingData in data.Extensions)
        {
            if (npcGiftingData.OverrideDialogueLine is not null &&
                GameStateQuery.CheckConditions(npcGiftingData.Condition, __instance.currentLocation, Game1.player, null, gift))
            {
                var dialogue = __instance.TryGetDialogue(npcGiftingData.OverrideDialogueLine);
                if (dialogue is not null) 
                    __result = dialogue;
                else Log.Log($"Unknown dialogue key {npcGiftingData.OverrideDialogueLine}");
                return false;
            }
        }
        return true;
    }
    
    
    [HarmonyPatch(typeof(NPC), nameof(NPC.receiveGift))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> NPC_receiveGift_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        try
        {
            var matcher = new CodeMatcher(instructions);
            matcher.MatchStartForward(
                new CodeMatch(OpCodes.Ldloc_1),
                new CodeMatch(OpCodes.Switch)
            ).ThrowIfNotMatch("Cannot find instructions for switching farmer's giftTasteForThisItem.");
            var matched = true;
            while (true)
            {
                try
                {
                    matcher.MatchStartForward(
                        new CodeMatch(OpCodes.Ldarg_0),
                        new CodeMatch(OpCodes.Callvirt,
                            AccessTools.Method(typeof(Farmer), nameof(Farmer.changeFriendship)))
                    ).ThrowIfNotMatch("No match");
                    matcher.InsertAndAdvance(
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldarg_1),
                        new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(GiftingPatches), nameof(OverrideFriendshipPointsIfNecessary)))
                    );
                    matcher.Advance(2);
                    matched = true;
                }
                catch (Exception _)
                {
                    matched = false;
                }
                if (!matched) break;
            }

            matcher.Start();
            // match if (showResponse)
            matcher.MatchStartForward(
                new CodeMatch(instr => instr.opcode == OpCodes.Ldarg_S && instr.MatchByIndex(5)),
                new CodeMatch(OpCodes.Brfalse_S)
            ).ThrowIfNotMatch("Cannot find instructions for checking if response should be shown.");
            matcher.MatchStartForward(new CodeMatch(OpCodes.Ret));
            matcher.Insert(
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(GiftingPatches), nameof(OnGiftReceivedCheckActions)))
            );
            return matcher.Instructions();
        } catch(Exception e)
        {
            return instructions;
        }
    }

    private static void OnGiftReceivedCheckActions(NPC npc, SObject obj)
    {
        if (!NPCGiftingExtensionDataAsset.Data.TryGetValue(npc.Name, out var data)) return;
        foreach (var npcGiftingData in data.Extensions)
        {
            if (GameStateQuery.CheckConditions(npcGiftingData.Condition, npc.currentLocation, Game1.player, null, obj))
            {
                npcGiftingData.Actions.ForEach(action =>
                {
                    if (!TriggerActionManager.TryRunAction(action, out var error, out var exception))
                        Log.Log($"Failed to run action {action}: {error}. Exception:\n{exception}", LogLevel.Error);
                });
            }
        }
    }

    private static int OverrideFriendshipPointsIfNecessary(int amount, NPC npc, SObject obj)
    {
        if (!NPCGiftingExtensionDataAsset.Data.TryGetValue(npc.Name, out var data)) return amount;
        foreach (var npcGiftingData in data.Extensions)
        {
            if (npcGiftingData.OverrideFriendshipPoints is not null &&
                GameStateQuery.CheckConditions(npcGiftingData.Condition, npc.currentLocation, Game1.player, null, obj))
            {
                return npcGiftingData.OverrideFriendshipPoints.Value;
            }
        }

        return amount;
    }
}