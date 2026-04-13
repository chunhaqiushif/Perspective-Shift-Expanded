using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace PerspectiveShiftExpanded
{
    [HarmonyPatch(typeof(Pawn_JobTracker), nameof(Pawn_JobTracker.StartJob))]
    public static class Pawn_JobTracker_StartJob_Patch
    {
        public static void Postfix(Pawn_JobTracker __instance, Job newJob)
        {
            Pawn pawn = __instance.pawn;
            if (pawn == null || !GetFromPerspectiveShift.IsAvatar(pawn)) { return; }
            if (newJob == null || newJob.def == null) { return; }
            JobDefsNeedTimeSpeedUp.SetTimeSpeedByJobDef(newJob.def);
            Log.Message(newJob.def.defName);
        }
    }
}