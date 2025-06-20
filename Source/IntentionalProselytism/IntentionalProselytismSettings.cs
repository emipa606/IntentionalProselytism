using Verse;

namespace IntentionalProselytism;

public class IntentionalProselytismSettings : ModSettings
{
    public static float CertaintyReduceFactor = 0.2f;
    public static bool UnlockVfemIndoctrinationPod;
    public static bool DisableInterColonistProselytizing;

    public override void ExposeData()
    {
        if (CertaintyReduceFactor < 0.05f)
        {
            CertaintyReduceFactor = 0.05f;
        }

        Scribe_Values.Look(ref CertaintyReduceFactor, "certaintyReduceFactor", 0.2f);
        if (CertaintyReduceFactor < 0.05f)
        {
            CertaintyReduceFactor = 0.05f;
        }

        Scribe_Values.Look(ref UnlockVfemIndoctrinationPod, "unlockVFEMIndoctrinationPod");
        Scribe_Values.Look(ref DisableInterColonistProselytizing, "disableInterColonistProselytizing");
    }
}