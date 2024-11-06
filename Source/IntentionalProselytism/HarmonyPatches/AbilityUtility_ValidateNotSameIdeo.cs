using HarmonyLib;
using RimWorld;
using Verse;

namespace IntentionalProselytism.HarmonyPatches;

[HarmonyPatch(typeof(AbilityUtility), nameof(AbilityUtility.ValidateNotSameIdeo))]
internal static class AbilityUtility_ValidateNotSameIdeo
{
    internal static void Postfix(ref bool __result, Pawn casterPawn, Pawn targetPawn)
    {
        if (!__result && !targetPawn.IsFreeNonSlaveColonist &&
            casterPawn.Ideo != IntentionalProselytismMod._datastorage.GetIdeo(targetPawn))
        {
            __result = true;
        }
    }
}