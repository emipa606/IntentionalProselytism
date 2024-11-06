using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace IntentionalProselytism.HarmonyPatches;

[HarmonyPatch(typeof(CompAbilityEffect_Convert), nameof(CompAbilityEffect_Convert.Apply))]
internal static class CompAbilityEffect_Convert_Apply
{
    internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var info1 = AccessTools.DeclaredField(typeof(Pawn), nameof(Pawn.ideo));
        var info2 = AccessTools.DeclaredField(typeof(AbilityComp), nameof(AbilityComp.parent));
        var info3 = AccessTools.DeclaredField(typeof(Ability), nameof(Ability.pawn));
        var info4 = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.Ideo));
        var info5 = AccessTools.DeclaredMethod(typeof(Pawn_IdeoTracker), nameof(Pawn_IdeoTracker.SetIdeo));
        var info6 = AccessTools.DeclaredField(typeof(Ideo), nameof(Ideo.name));

        var list = instructions.ToList();
        for (var i = 0; i < list.Count; i++)
        {
            if (i + 6 < list.Count &&
                list[i].opcode == OpCodes.Ldloc_1 &&
                list[i + 1].LoadsField(info1) &&
                list[i + 2].opcode == OpCodes.Ldarg_0 &&
                list[i + 3].LoadsField(info2) &&
                list[i + 4].LoadsField(info3) &&
                list[i + 5].Calls(info4) &&
                list[i + 6].Calls(info5)
               )
            {
                i += 6;
            }
            else if (
                i + 3 < list.Count &&
                list[i].opcode == OpCodes.Ldloc_0 &&
                list[i + 1].Calls(info4) &&
                list[i + 2].LoadsField(info6) &&
                list[i + 3].opcode == OpCodes.Ldstr && list[i + 3].OperandIs("IDEO")
            )
            {
                yield return new CodeInstruction(OpCodes.Ldloc_1);
            }
            else
            {
                yield return list[i];
            }
        }
    }
}