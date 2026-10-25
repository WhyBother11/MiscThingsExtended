using System.Reflection.Emit;
using HarmonyLib;

namespace MiscThingsExtended.Framework.Util;

public static class Utility
{
    /// <summary>
    /// Prepend mod id to given <paramref name="str"/>.
    /// </summary>
    /// <param name="str">String to prepend id to.</param>
    /// <returns><paramref name="str"/> with prepended mod id.</returns>
    public static string PrependId(this string str) => $"{Id}_{str}";
    
    public static bool MatchByIndex(this CodeInstruction codeInstruction, int index)
    {
        if (codeInstruction.operand is int idx) return idx == index;
        if (codeInstruction.operand is byte bIdx) return bIdx == (byte) index;
        if (codeInstruction.operand is LocalBuilder lb) return lb.LocalIndex == index;
        throw new ArgumentException($"Operand {codeInstruction.operand.GetType()} is not supported");
    }
}
