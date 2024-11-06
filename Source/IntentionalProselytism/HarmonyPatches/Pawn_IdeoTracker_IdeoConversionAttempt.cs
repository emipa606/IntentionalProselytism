using HarmonyLib;
using RimWorld;
using Verse;

namespace IntentionalProselytism.HarmonyPatches;

[HarmonyPatch(typeof(Pawn_IdeoTracker), nameof(Pawn_IdeoTracker.IdeoConversionAttempt))]
internal static class Pawn_IdeoTracker_IdeoConversionAttempt
{
    internal static void Prefix(Pawn ___pawn, ref Ideo initiatorIdeo)
    {
        if (___pawn.IsFreeNonSlaveColonist)
        {
            return;
        }

        var res = IntentionalProselytismMod._datastorage.GetIdeo(___pawn);
        if (res == null)
        {
            return;
        }

        initiatorIdeo = res;
    }
}