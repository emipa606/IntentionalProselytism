﻿using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace IntentionalProselytism;

public class DataStorage(World world) : WorldComponent(world)
{
    private static int version;
    private HashSet<int> disableProselyting = [];

    private Dictionary<int, Ideo> pawnIdeoData = new();
    private List<int> pawnIdeoDataKey;
    private List<Ideo> pawnIdeoDataValue;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Collections.Look(ref pawnIdeoData, "pawnIdeoData", LookMode.Value, LookMode.Reference,
            ref pawnIdeoDataKey, ref pawnIdeoDataValue);
        Scribe_Collections.Look(ref disableProselyting, "disableProselyting", LookMode.Value);
        disableProselyting ??= [];

        Scribe_Values.Look(ref version, "version");
    }

    public Ideo GetIdeo(Pawn pawn)
    {
        return pawnIdeoData.TryGetValue(pawn.thingIDNumber);
    }

    public void SetIdeo(Pawn pawn, Ideo ideo)
    {
        removeDisabled(pawn);
        pawnIdeoData[pawn.thingIDNumber] = ideo;
    }

    public void RemoveIdeo(Pawn pawn)
    {
        pawnIdeoData.Remove(pawn.thingIDNumber);
    }

    public void SetDiabled(Pawn pawn)
    {
        RemoveIdeo(pawn);
        disableProselyting.Add(pawn.thingIDNumber);
    }

    private void removeDisabled(Pawn pawn)
    {
        disableProselyting.Remove(pawn.thingIDNumber);
    }

    public bool GetDisabled(Pawn pawn)
    {
        return disableProselyting.Contains(pawn.thingIDNumber);
    }

    public override void FinalizeInit(bool fromLoad)
    {
        IntentionalProselytismMod.DataStorage = this;
    }

    public static Ideo GetIdeoStatic(Pawn pawn, Pawn pawn2 = null)
    {
        var targ = IntentionalProselytismMod.DataStorage.GetIdeo(pawn);
        if (targ is not null)
        {
            return targ;
        }

        targ = pawn2 is null ? Faction.OfPlayer.ideos.PrimaryIdeo : pawn2.Ideo;

        return targ;
    }
}