using HarmonyLib;
using RimWorld;
using System;
using System.Reflection;
using Verse;
using Verse.AI;

namespace PerspectiveShiftExpanded
{
    public class JobStateSnapshot
    {
        public JobDef def;
        public bool playerForced;
        public LocalTargetInfo targetA;
    }
    [HotSwappable]

    [HarmonyPatch(typeof(Pawn_JobTracker), nameof(Pawn_JobTracker.EndCurrentJob))]
    public static class Pawn_JobTracker_EndCurrentJob_Patch
    {


        public static void Prefix(Pawn_JobTracker __instance, out JobStateSnapshot __state)
        {
            __state = new JobStateSnapshot();
            if (__instance.curJob == null) { return; }

            __state.def = __instance.curJob.def;
            __state.playerForced = __instance.curJob.playerForced;
            __state.targetA = __instance.curJob.targetA;

            if (ModCompatibility.PSE_PS_State_IsAvatar(__instance.pawn))
            {
                JobDefsNeedTimeSpeedUp.ResetTimeSpeed(__state.def);
            }
        }


    }
}