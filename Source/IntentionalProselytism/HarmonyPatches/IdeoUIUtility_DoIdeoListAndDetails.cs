using System;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.Sound;

namespace IntentionalProselytism.HarmonyPatches;

[HarmonyPatch(typeof(IdeoUIUtility), nameof(IdeoUIUtility.DoIdeoListAndDetails))]
internal static class IdeoUIUtility_DoIdeoListAndDetails
{
    private static void CreateNewIdeo()
    {
        var ideo = IdeoUtility.MakeEmptyIdeo();
        Find.WindowStack.Add(new Dialog_ChooseMemes(ideo, MemeCategory.Structure, false, () =>
        {
            SoundDefOf.Click.PlayOneShotOnCamera();
            IdeoUIUtility.SetSelected(ideo);
            if (!Find.IdeoManager.IdeosListForReading.Contains(ideo))
            {
                Find.IdeoManager.Add(ideo);
            }
        }));
    }

    internal static void Prefix(ref bool showCreateIdeoButton, ref bool showLoadExistingIdeoBtn,
        ref Action createCustomBtnActOverride)
    {
        showCreateIdeoButton = true;
        showLoadExistingIdeoBtn = true;
        if (Find.WindowStack.WindowOfType<Dialog_ConfigureIdeo>() == null)
        {
            createCustomBtnActOverride = CreateNewIdeo;
        }
    }
}