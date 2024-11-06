using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace IntentionalProselytism.HarmonyPatches;

[HarmonyPatch(typeof(IdeoUIUtility), "DrawIdeoRow")]
internal static class IdeoUIUtility_DrawIdeoRow
{
    internal static bool canDelete;

    internal static void Postfix(Ideo ideo, float curY, Rect fillRect, List<Pawn> pawns)
    {
        if (ideo != IdeoUIUtility.selected || pawns?.Count > 0 || !ideo.CanDelete())
        {
            return;
        }

        DoDeleteIcon(new Rect(0, curY - 46, fillRect.width, 46f), ideo);
    }

    internal static bool CanDelete(this Ideo ideo)
    {
        canDelete = Find.FactionManager.AllFactionsInViewOrder.All(x => !(x.ideos?.Has(ideo) ?? false)) &&
                    PawnsFinder.AllMapsAndWorld_Alive.All(x => x.Ideo != ideo);
        return canDelete;
    }

    internal static void DoDeleteIcon(Rect viewRect, Ideo ideo)
    {
        var size = 24f;
        var rect = new Rect(viewRect.x + viewRect.width - size, viewRect.y + ((viewRect.height - size) / 2), size,
            size);
        if (Widgets.ButtonImage(rect, TexButton.Delete, Color.white, GenUI.SubtleMouseoverColor))
        {
            Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("ConfirmDelete".Translate(ideo.name),
                () => { Find.IdeoManager.Remove(ideo); }, true));
        }
    }
}