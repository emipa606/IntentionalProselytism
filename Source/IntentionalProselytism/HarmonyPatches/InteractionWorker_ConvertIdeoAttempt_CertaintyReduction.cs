using HarmonyLib;
using RimWorld;
using Verse;

namespace IntentionalProselytism.HarmonyPatches;

[HarmonyPatch(typeof(InteractionWorker_ConvertIdeoAttempt),
    nameof(InteractionWorker_ConvertIdeoAttempt.CertaintyReduction))]
internal static class InteractionWorker_ConvertIdeoAttempt_CertaintyReduction
{
    internal static void Postfix(ref float __result, Pawn initiator, Pawn recipient)
    {
        var ideo = IntentionalProselytismMod.DataStorage.GetIdeo(recipient);
        if (ideo != null && ideo != initiator.Ideo)
        {
            __result *= IntentionalProselytismSettings.CertaintyReduceFactor;
        }
    }
}