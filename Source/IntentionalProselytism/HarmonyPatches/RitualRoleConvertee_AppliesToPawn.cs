using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace IntentionalProselytism.HarmonyPatches;

[HarmonyPatch(typeof(RitualRoleConvertee), nameof(RitualRoleConvertee.AppliesToPawn))]
internal static class RitualRoleConvertee_AppliesToPawn
{
    internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var info1 = AccessTools.PropertyGetter(typeof(RitualRoleAssignments), nameof(RitualRoleAssignments.Ritual));
        var info2 = AccessTools.DeclaredField(typeof(Precept), nameof(Precept.ideo));
        var info3 = AccessTools.PropertyGetter(typeof(Pawn), nameof(Ideo));
        var info4 = AccessTools.DeclaredMethod(typeof(RitualRoleConvertee_AppliesToPawn), nameof(SameIdeo));

        var list = instructions.ToList();
        for (var i = 0; i < list.Count; i++)
        {
            yield return list[i];
            if (i + 9 >= list.Count ||
                list[i].opcode != OpCodes.Ldarg_1 ||
                !list[i + 1].Calls(info3) ||
                !list[i + 2].IsLdarg(4) ||
                list[i + 3].opcode != OpCodes.Brtrue_S ||
                list[i + 4].opcode != OpCodes.Ldnull ||
                list[i + 5].opcode != OpCodes.Br_S ||
                !list[i + 6].IsLdarg(4) ||
                !list[i + 7].Calls(info1) ||
                !list[i + 8].LoadsField(info2) ||
                list[i + 9].opcode != OpCodes.Bne_Un_S)
            {
                continue;
            }

            list[i + 9].opcode = OpCodes.Brfalse_S;
            yield return new CodeInstruction(OpCodes.Ldarg, 4);
            yield return new CodeInstruction(OpCodes.Call, info4);
            yield return list[i + 9];
            i += 9;
        }
    }

    internal static bool SameIdeo(Pawn p, RitualRoleAssignments assignment)
    {
        var ideo = IntentionalProselytismMod.DataStorage.GetIdeo(p) ?? p.Ideo;
        return ideo == assignment?.Ritual.ideo;
    }
}