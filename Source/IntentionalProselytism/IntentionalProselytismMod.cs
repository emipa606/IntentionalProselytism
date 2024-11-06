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
    internal const string Modname_VFEM = "Vanilla Factions Expanded - Mechanoids";

    internal static DataStorage _datastorage;

    internal static readonly Type type_Building_IndoctrinationPod =
        AccessTools.TypeByName("VFEMech.Building_IndoctrinationPod");

    internal static readonly FieldInfo field_ideoConversionTarget =
        AccessTools.Field(type_Building_IndoctrinationPod, "ideoConversionTarget");

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
        IntentionalProselytismSettings.certaintyReduceFactor = Widgets.HorizontalSlider(rect,
            IntentionalProselytismSettings.certaintyReduceFactor, 0.05f, 0.5f, true, label);
        TooltipHandler.TipRegion(rect, TranslationKeys.CertaintyReduceFactor__Desc.Translate());
        rect = new Rect(canvas.x + 220, canvas.y, 50, Text.LineHeight);
        Widgets.Label(rect, IntentionalProselytismSettings.certaintyReduceFactor.ToString());
        rect = new Rect(canvas.x, canvas.y + Text.LineHeight, 100, Text.LineHeight);
        if (Widgets.ButtonText(rect, TranslationKeys.ResetToDefault.Translate()))
        {
            IntentionalProselytismSettings.certaintyReduceFactor = 0.2f;
        }

        label = TranslationKeys.DisableInterColonistProselytizing.Translate();
        rect = new Rect(canvas.x, rect.y + Text.LineHeight, Text.CalcSize(label).x + Text.LineHeight + 10f,
            Text.LineHeight);
        Widgets.CheckboxLabeled(rect, label, ref IntentionalProselytismSettings.disableInterColonistProselytizing);
        TooltipHandler.TipRegion(rect, TranslationKeys.DisableInterColonistProselytizing__Desc.Translate());

        if (currentVersion != null)
        {
            rect = new Rect(canvas.x, rect.y + Text.LineHeight, Text.CalcSize(label).x + Text.LineHeight + 10f,
                Text.LineHeight);
            GUI.contentColor = Color.gray;
            Widgets.Label(rect, TranslationKeys.ModVersion.Translate(currentVersion));
            GUI.contentColor = Color.white;
        }

        if (!ModLister.HasActiveModWithName(Modname_VFEM))
        {
            return;
        }

        label = TranslationKeys.UnlockVFEMIndoctrinationPod.Translate();
        rect = new Rect(canvas.x, rect.y + Text.LineHeight, Text.CalcSize(label).x + Text.LineHeight + 10f,
            Text.LineHeight);
        Widgets.CheckboxLabeled(rect, label, ref IntentionalProselytismSettings.unlockVFEMIndoctrinationPod);
        TooltipHandler.TipRegion(rect, TranslationKeys.UnlockVFEMIndoctrinationPod__Desc.Translate());
        if (!IntentionalProselytismSettings.unlockVFEMIndoctrinationPod &&
            Current.ProgramState == ProgramState.Playing)
        {
            Find.Maps.ForEach(x =>
                x.spawnedThings
                    .Where(thing => type_Building_IndoctrinationPod.IsAssignableFrom(thing.def.thingClass)).ToList()
                    .ForEach(y => field_ideoConversionTarget.SetValue(y, Faction.OfPlayer.ideos.PrimaryIdeo)));
        }
    }

    public override string SettingsCategory()
    {
        return TranslationKeys.Setting.Translate();
    }
}