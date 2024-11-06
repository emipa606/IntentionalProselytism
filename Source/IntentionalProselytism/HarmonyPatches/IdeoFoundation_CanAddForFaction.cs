using HarmonyLib;
using RimWorld;
using Verse;

namespace IntentionalProselytism.HarmonyPatches;

[HarmonyPatch(typeof(IdeoFoundation), nameof(IdeoFoundation.CanAddForFaction))]
internal static class IdeoFoundation_CanAddForFaction
{
    internal static bool Prefix(PreceptDef precept)
    {
        return precept != PreceptDefOf.IdeoRelic || Current.ProgramState != ProgramState.Playing;
    }
}