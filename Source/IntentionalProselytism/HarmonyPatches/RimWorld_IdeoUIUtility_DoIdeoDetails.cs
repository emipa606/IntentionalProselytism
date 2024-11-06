using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace IntentionalProselytism.HarmonyPatches;

[HarmonyPatch(typeof(IdeoUIUtility), nameof(IdeoUIUtility.DoIdeoDetails))]
internal static class RimWorld_IdeoUIUtility_DoIdeoDetails
{
    internal static string text;
    internal static float textWidth;

    static RimWorld_IdeoUIUtility_DoIdeoDetails()
    {
        LongEventHandler.ExecuteWhenFinished(() =>
        {
            text = "IntentionalProselytism.MakeFluid".Translate();
            textWidth = Text.CalcSize(text).x + 15f;
        });
    }

    internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var hasFound = false;
        var foundLabel = false;
        var label = default(Label);
        var list = instructions.ToList();
        for (var i = 0; i < list.Count; ++i)
        {
            var code = list[i];
            if (!foundLabel && code.opcode == OpCodes.Brfalse_S &&
                list[i - 1].Calls(AccessTools.PropertyGetter(typeof(Ideo), nameof(Ideo.Fluid))))
            {
                label = (Label)code.operand;
                foundLabel = true;
            }
            else if (!hasFound && foundLabel && code.labels.Contains(label))
            {
                code.labels.Remove(label);
                yield return new CodeInstruction(OpCodes.Ldloc_3).WithLabels(label);
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Ldarg_1);
                yield return new CodeInstruction(OpCodes.Call,
                    AccessTools.Method(typeof(RimWorld_IdeoUIUtility_DoIdeoDetails), nameof(MakeFluid)));
                hasFound = true;
            }

            yield return code;
        }
    }

    internal static void MakeFluid(float curY, Rect inRect, Ideo ideo)
    {
        if (ideo.Fluid || !CanFluid(ideo))
        {
            return;
        }

        var width = inRect.width - 16f;
        var x2 = (width - (IdeoUIUtility.PreceptBoxSize.x * 3f) - 16f) / 2f;
        if (!Widgets.ButtonText(new Rect(x2, curY, textWidth, Text.LineHeight + 5f), text))
        {
            return;
        }

        ideo.Fluid = true;
        ideo.development.reformCount = ideo.memes.Count - 2;
    }

    internal static bool CanFluid(Ideo ideo)
    {
        return !Find.FactionManager.AllFactionsInViewOrder.Any(x =>
                   x != Faction.OfPlayer && (x.ideos?.Has(ideo) ?? false)) &&
               !PawnsFinder.AllMapsAndWorld_Alive.Any(x => x.Faction != Faction.OfPlayer && x.Ideo == ideo);
    }
}