using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace PerspectiveShiftExpanded
{
    [StaticConstructorOnStartup]
    public static class Startup
    {
        public static readonly Harmony harmony;
        static Startup()
        {
            harmony = new Harmony("Tupler.PerspectiveShiftExpanded");
            harmony.PatchAll();
        }
    }
}
