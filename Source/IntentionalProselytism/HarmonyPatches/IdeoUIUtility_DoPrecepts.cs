using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace IntentionalProselytism.HarmonyPatches;

[HarmonyPatch(typeof(IdeoUIUtility), nameof(IdeoUIUtility.DoPrecepts))]
internal static class IdeoUIUtility_DoPrecepts
{
    internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        string ldstr = null;
        var first = true;
        foreach (var code in instructions)
        {
            yield return code;
            if (code.opcode == OpCodes.Ldstr)
            {
                ldstr = (string)code.operand;
            }
            else if (first && code.opcode == OpCodes.Ldarg_3 && ldstr == "IdeoRelic")
            {
                yield return new CodeInstruction(OpCodes.Call,
                    AccessTools.Method(typeof(IdeoUIUtility_DoPrecepts), nameof(ChangeMode)));
                first = false;
            }
        }
    }

    internal static IdeoEditMode ChangeMode(IdeoEditMode mode)
    {
        return mode == IdeoEditMode.Dev || mode == IdeoEditMode.Reform || Current.ProgramState != ProgramState.Playing
            ? mode
            : IdeoEditMode.None;
    }
}