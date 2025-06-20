using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Mlie;
using RimWorld;
using UnityEngine;
using Verse;

namespace IntentionalProselytism;

public class IntentionalProselytismMod : Mod
{
    internal const string ModnameVfem = "Vanilla Factions Expanded - Mechanoids";

    internal static DataStorage DataStorage;

    private static readonly Type typeBuildingIndoctrinationPod =
        AccessTools.TypeByName("VFEMech.Building_IndoctrinationPod");

    private static readonly FieldInfo fieldIdeoConversionTarget =
        AccessTools.Field(typeBuildingIndoctrinationPod, "ideoConversionTarget");

    private static string currentVersion;

    public IntentionalProselytismMod(ModContentPack content) : base(content)
    {
        GetSettings<IntentionalProselytismSettings>();
        LongEventHandler.ExecuteWhenFinished(() => { });
        currentVersion = VersionFromManifest.GetVersionFromModMetaData(content.ModMetaData);
    }

    public override void DoSettingsWindowContents(Rect canvas)
    {
        var label = TranslationKeys.CertaintyReduceFactor.Translate();
        var labelSize = Text.CalcSize(label);
        var rect = new Rect(canvas.x, canvas.y, labelSize.x + 10f, Text.LineHeight);
        IntentionalProselytismSettings.CertaintyReduceFactor = Widgets.HorizontalSlider(rect,
            IntentionalProselytismSettings.CertaintyReduceFactor, 0.05f, 0.5f, true, label);
        TooltipHandler.TipRegion(rect, TranslationKeys.CertaintyReduceFactorDesc.Translate());
        rect = new Rect(canvas.x + 220, canvas.y, 50, Text.LineHeight);
        Widgets.Label(rect, IntentionalProselytismSettings.CertaintyReduceFactor.ToString());
        rect = new Rect(canvas.x, canvas.y + Text.LineHeight, 100, Text.LineHeight);
        if (Widgets.ButtonText(rect, TranslationKeys.ResetToDefault.Translate()))
        {
            IntentionalProselytismSettings.CertaintyReduceFactor = 0.2f;
        }

        label = TranslationKeys.DisableInterColonistProselytizing.Translate();
        rect = new Rect(canvas.x, rect.y + Text.LineHeight, Text.CalcSize(label).x + Text.LineHeight + 10f,
            Text.LineHeight);
        Widgets.CheckboxLabeled(rect, label, ref IntentionalProselytismSettings.DisableInterColonistProselytizing);
        TooltipHandler.TipRegion(rect, TranslationKeys.DisableInterColonistProselytizingDesc.Translate());

        if (currentVersion != null)
        {
            rect = new Rect(canvas.x, rect.y + Text.LineHeight, Text.CalcSize(label).x + Text.LineHeight + 10f,
                Text.LineHeight);
            GUI.contentColor = Color.gray;
            Widgets.Label(rect, TranslationKeys.ModVersion.Translate(currentVersion));
            GUI.contentColor = Color.white;
        }

        if (!ModLister.HasActiveModWithName(ModnameVfem))
        {
            return;
        }

        label = TranslationKeys.UnlockVfemIndoctrinationPod.Translate();
        rect = new Rect(canvas.x, rect.y + Text.LineHeight, Text.CalcSize(label).x + Text.LineHeight + 10f,
            Text.LineHeight);
        Widgets.CheckboxLabeled(rect, label, ref IntentionalProselytismSettings.UnlockVfemIndoctrinationPod);
        TooltipHandler.TipRegion(rect, TranslationKeys.UnlockVfemIndoctrinationPodDesc.Translate());
        if (!IntentionalProselytismSettings.UnlockVfemIndoctrinationPod &&
            Current.ProgramState == ProgramState.Playing)
        {
            Find.Maps.ForEach(x =>
                x.spawnedThings
                    .Where(thing => typeBuildingIndoctrinationPod.IsAssignableFrom(thing.def.thingClass)).ToList()
                    .ForEach(y => fieldIdeoConversionTarget.SetValue(y, Faction.OfPlayer.ideos.PrimaryIdeo)));
        }
    }

    public override string SettingsCategory()
    {
        return TranslationKeys.Setting.Translate();
    }
}