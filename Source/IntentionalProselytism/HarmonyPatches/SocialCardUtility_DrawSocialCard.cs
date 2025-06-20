using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace IntentionalProselytism.HarmonyPatches;

[HarmonyPatch(typeof(SocialCardUtility), nameof(SocialCardUtility.DrawSocialCard))]
internal static class SocialCardUtility_DrawSocialCard
{
    private static readonly FieldInfo socialCardUtilityRoleChangeButtonSize =
        AccessTools.DeclaredField(typeof(SocialCardUtility), "RoleChangeButtonSize");

    internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var info1 = AccessTools.DeclaredMethod(typeof(SocialCardUtility),
            nameof(SocialCardUtility.DrawPawnRoleSelection));
        var info2 = AccessTools.DeclaredMethod(typeof(SocialCardUtility_DrawSocialCard),
            nameof(DrawPawnIdeoSelection));
        var info3 = AccessTools.DeclaredMethod(typeof(SocialCardUtility), nameof(SocialCardUtility.DrawPawnRole));
        var info4 = AccessTools.DeclaredMethod(typeof(SocialCardUtility_DrawSocialCard), nameof(DrawPawnIdeo));

        foreach (var code in instructions)
        {
            yield return code;
            if (code.Calls(info1))
            {
                yield return new CodeInstruction(OpCodes.Ldarg_1);
                yield return new CodeInstruction(OpCodes.Ldloc_3);
                yield return Call(info2);
            }
            else if (code.Calls(info3))
            {
                yield return new CodeInstruction(OpCodes.Ldarg_1);
                yield return new CodeInstruction(OpCodes.Ldloc_3);
                yield return Call(info4);
            }
        }

        yield break;

        CodeInstruction Call(MethodInfo info)
        {
            return new CodeInstruction(OpCodes.Call, info);
        }
    }

    internal static void DrawPawnIdeoSelection(Pawn pawn, Rect rect)
    {
        if (pawn.IsFreeNonSlaveColonist)
        {
            return;
        }

        var RoleChangeButtonSize = (Vector2)socialCardUtilityRoleChangeButtonSize.GetValue(null);
        var y = rect.y + (rect.height / 2f) - 14f;
        var rect2 = new Rect(rect.width - 150f, y, RoleChangeButtonSize.x, RoleChangeButtonSize.y)
        {
            xMax = rect.width - 26f - 4f
        };
        var hasPawns = true;
        var rituals = Find.IdeoManager.GetActiveRituals(pawn.Map);
        foreach (var r in rituals)
        {
            if (!r.PawnsToCountTowardsPresence.Contains(pawn))
            {
                continue;
            }

            hasPawns = false;
            break;
        }

        if (!Widgets.ButtonText(rect2, TranslationKeys.ChooseIdeo.Translate() + "...") || !hasPawns)
        {
            return;
        }

        var options = new List<FloatMenuOption>();
        var ideo = IntentionalProselytismMod.DataStorage.GetIdeo(pawn);
        if (ideo != null)
        {
            options.Add(new FloatMenuOption(TranslationKeys.RemoveCurrentIdeo.Translate(),
                () => IntentionalProselytismMod.DataStorage.RemoveIdeo(pawn), Widgets.PlaceholderIconTex,
                Color.white));
        }

        foreach (var i in Find.IdeoManager.IdeosListForReading)
        {
            if (i != pawn.Ideo)
            {
                options.Add(new FloatMenuOption(i.name,
                    () => IntentionalProselytismMod.DataStorage.SetIdeo(pawn, i), i.Icon, i.Color));
            }
        }

        options.Add(new FloatMenuOption(TranslationKeys.DisableProselyting.Translate(),
            () => IntentionalProselytismMod.DataStorage.SetDiabled(pawn), HarmonyBase.Icon_DisabledProselyting,
            Color.white));
        Find.WindowStack.Add(new FloatMenu(options));
    }

    internal static void DrawPawnIdeo(Pawn pawn, Rect rect)
    {
        if (pawn.IsFreeNonSlaveColonist)
        {
            return;
        }

        var ideo = IntentionalProselytismMod.DataStorage.GetIdeo(pawn);
        var disabled = IntentionalProselytismMod.DataStorage.GetDisabled(pawn);
        var num = rect.x + 17f;
        if (disabled || ideo != null)
        {
            var y = rect.y + (rect.height / 2f) - 16f;
            var outerRect = rect;
            outerRect.x = num;
            outerRect.y = y;
            outerRect.width = 32f;
            outerRect.height = 32f;
            GUI.color = ideo?.Color ?? Color.white;
            Widgets.DrawTextureFitted(outerRect, disabled ? HarmonyBase.Icon_DisabledProselyting : ideo.Icon, 1f);
            GUI.color = Color.white;
            num += 42f;
        }
        else
        {
            GUI.color = Color.grey;
        }

        var roleChangeButtonSize = (Vector2)socialCardUtilityRoleChangeButtonSize.GetValue(null);
        var rect2 = new Rect(rect.x + 17f, rect.y + (rect.height / 2f) - 16f, rect.width - num - roleChangeButtonSize.x,
            32f);
        var rect3 = rect;
        rect3.xMin = num;
        Text.Anchor = TextAnchor.MiddleLeft;
        var label = disabled ? TranslationKeys.DisableProselyting.Translate().ToString() :
            ideo == null ? TranslationKeys.NoIdeoAssigned.Translate().ToString() : ideo.name;
        Widgets.Label(rect3, label);
        Text.Anchor = TextAnchor.UpperLeft;
        GUI.color = Color.white;
        if (Mouse.IsOver(rect2))
        {
            string roleDesc = null;
            if (disabled)
            {
                roleDesc = TranslationKeys.IdeoDesc.Translate() + "\n\n" +
                           TranslationKeys.DisableProselytingDesc.Translate();
            }
            else if (ideo != null)
            {
                roleDesc = TranslationKeys.IdeoDesc.Translate() + "\n\n" + ideo.name + "\n\n" + ideo.description +
                           "\n\n" + "Memes".Translate() + "\n" + ideo.memes.Join(x => x.LabelCap, "\n");
            }

            Widgets.DrawHighlight(rect2);
            if (!roleDesc.NullOrEmpty())
            {
                TooltipHandler.TipRegion(rect2, roleDesc);
            }
        }

        GUI.color = new Color(1f, 1f, 1f, 0.5f);
        Widgets.DrawLineHorizontal(0f, rect.yMax, rect.width);
        GUI.color = Color.white;
    }
}