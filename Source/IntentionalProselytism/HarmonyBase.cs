using HarmonyLib;
using UnityEngine;
using Verse;

namespace IntentionalProselytism;

[StaticConstructorOnStartup]
internal static class HarmonyBase
{
    internal static readonly Harmony instance;

    public static readonly Texture2D
        Icon_DisabledProselyting = ContentFinder<Texture2D>.Get("UI/DisabledProselyting");

    static HarmonyBase()
    {
        instance = new Harmony("ordpus.IntentionalProselytism");
        instance.PatchAll();
    }
}