using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace IntentionalProselytism.HarmonyPatches;

[HarmonyPatch]
internal static class VFEMech_Building_IndoctrinationPod_GetGizmos_d_13_MoveNext
{
    internal static IEnumerable<MethodBase> TargetMethods()
    {
        return new List<MethodBase>
        {
            AccessTools.EnumeratorMoveNext(AccessTools.Method("VFEMech.Building_IndoctrinationPod:GetGizmos"))
        }.Union(AccessTools.TypeByName("VFEMech.Building_IndoctrinationPod").GetMethods(AccessTools.all)
            .Where(x => x.Name.Contains("GetGizmos") || x.Name.Contains("Tick")));
    }

    internal static bool Prepare()
    {
        return ModLister.HasActiveModWithName(IntentionalProselytismMod.ModnameVfem);
    }

    internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (var code in instructions)
        {
            yield return code;
            if (code.Calls(
                    AccessTools.PropertyGetter(typeof(FactionIdeosTracker), nameof(FactionIdeosTracker.AllIdeos))))
            {
                yield return new CodeInstruction(OpCodes.Call,
                    AccessTools.Method(typeof(VFEMech_Building_IndoctrinationPod_GetGizmos_d_13_MoveNext),
                        nameof(GetIdeos)));
            }
        }
    }

    internal static IEnumerable<Ideo> GetIdeos(IEnumerable<Ideo> original)
    {
        return IntentionalProselytismSettings.UnlockVfemIndoctrinationPod
            ? Find.IdeoManager.IdeosListForReading
            : original;
    }
}