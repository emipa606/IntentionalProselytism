using HarmonyLib;
using RimWorld;
using Verse;

namespace IntentionalProselytism.HarmonyPatches;

[HarmonyPatch(typeof(SocialCardUtility), nameof(SocialCardUtility.DrawPawnRole))]
internal static class SocialCardUtility_DrawPawnRole
{
    internal static bool Prefix(Pawn pawn)
    {
        return pawn.IsFreeNonSlaveColonist;
    }
}