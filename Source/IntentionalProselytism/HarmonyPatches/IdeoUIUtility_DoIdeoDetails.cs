using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;

namespace IntentionalProselytism.HarmonyPatches;

[HarmonyPatch(typeof(IdeoUIUtility), nameof(IdeoUIUtility.DoIdeoDetails))]
internal static class IdeoUIUtility_DoIdeoDetails
{
    internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var firstLoc = true;
        foreach (var code in instructions)
        {
            yield return code;
            if (!firstLoc || code.opcode != OpCodes.Ldloc_1)
            {
                continue;
            }

            yield return new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(IdeoUIUtility_DoIdeoDetails), nameof(ChangeMode)));
            yield return new CodeInstruction(OpCodes.Stloc_1);
            yield return new CodeInstruction(OpCodes.Ldloc_1);
            firstLoc = false;
        }
    }

    internal static IdeoEditMode ChangeMode(IdeoEditMode mode)
    {
        return mode == IdeoEditMode.Dev || mode == IdeoEditMode.Reform ? mode :
            IdeoUIUtility_DrawIdeoRow.canDelete ? IdeoEditMode.GameStart : mode;
    }
}