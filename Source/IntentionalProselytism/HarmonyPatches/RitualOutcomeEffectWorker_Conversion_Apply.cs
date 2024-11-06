using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;

namespace IntentionalProselytism;

[HarmonyPatch(typeof(RitualOutcomeEffectWorker_Conversion), nameof(RitualOutcomeEffectWorker_Conversion.Apply))]
internal static class RitualOutcomeEffectWorker_Conversion_Apply
{
    internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var info1 = AccessTools.DeclaredMethod(typeof(Pawn_IdeoTracker), nameof(Pawn_IdeoTracker.SetIdeo));
        foreach (var code in instructions)
        {
            if (code.Calls(info1))
            {
                yield return new CodeInstruction(OpCodes.Pop);
                yield return new CodeInstruction(OpCodes.Ldloc_S, 5);
                yield return new CodeInstruction(OpCodes.Ldloc_S, 4);
                yield return new CodeInstruction(OpCodes.Call,
                    AccessTools.DeclaredMethod(typeof(DataStorage), nameof(DataStorage.GetIdeoStatic)));
            }

            yield return code;
        }
    }
}