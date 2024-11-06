using HarmonyLib;
using RimWorld;
using Verse;

namespace IntentionalProselytism.HarmonyPatches;

[HarmonyPatch(typeof(AbilityUtility), nameof(AbilityUtility.ValidateNotSameIdeo))]
internal static class RimWorld__AbilityUtility_ValidateNotSameIdeo
{
    internal static void Prefix(Pawn targetPawn, ref bool showMessages)
    {
        if (DataStorage.GetIdeoStatic(targetPawn, targetPawn) != targetPawn.Ideo)
        {
            showMessages = false;
        }
    }
}