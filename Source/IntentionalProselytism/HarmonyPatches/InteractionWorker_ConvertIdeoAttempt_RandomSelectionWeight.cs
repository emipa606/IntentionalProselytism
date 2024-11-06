using HarmonyLib;
using RimWorld;
using Verse;

namespace IntentionalProselytism.HarmonyPatches;

[HarmonyPatch(typeof(InteractionWorker_ConvertIdeoAttempt),
    nameof(InteractionWorker_ConvertIdeoAttempt.RandomSelectionWeight))]
internal static class InteractionWorker_ConvertIdeoAttempt_RandomSelectionWeight
{
    internal static void Postfix(ref float __result, Pawn initiator, Pawn recipient)
    {
        if (IntentionalProselytismMod._datastorage.GetDisabled(recipient))
        {
            __result = 0;
            return;
        }

        if (IntentionalProselytismSettings.disableInterColonistProselytizing && initiator.IsFreeNonSlaveColonist &&
            recipient.IsFreeNonSlaveColonist)
        {
            __result = 0;
        }
    }
}