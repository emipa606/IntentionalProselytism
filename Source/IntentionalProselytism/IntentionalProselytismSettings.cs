using Verse;

namespace IntentionalProselytism;

public class IntentionalProselytismSettings : ModSettings
{
    public static float certaintyReduceFactor = 0.2f;
    public static bool unlockVFEMIndoctrinationPod;
    public static bool disableInterColonistProselytizing;

    public override void ExposeData()
    {
        if (certaintyReduceFactor < 0.05f)
        {
            certaintyReduceFactor = 0.05f;
        }

        Scribe_Values.Look(ref certaintyReduceFactor, "certaintyReduceFactor", 0.2f);
        if (certaintyReduceFactor < 0.05f)
        {
            certaintyReduceFactor = 0.05f;
        }

        Scribe_Values.Look(ref unlockVFEMIndoctrinationPod, "unlockVFEMIndoctrinationPod");
        Scribe_Values.Look(ref disableInterColonistProselytizing, "disableInterColonistProselytizing");
    }
}