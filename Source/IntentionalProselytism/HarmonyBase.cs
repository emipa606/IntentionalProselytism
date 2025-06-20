using HarmonyLib;
using UnityEngine;
using Verse;

namespace IntentionalProselytism;

[StaticConstructorOnStartup]
internal static class HarmonyBase
{
    public static readonly Texture2D
        Icon_DisabledProselyting = ContentFinder<Texture2D>.Get("UI/DisabledProselyting");

    static HarmonyBase()
    {
        new Harmony("ordpus.IntentionalProselytism").PatchAll();
    }
}