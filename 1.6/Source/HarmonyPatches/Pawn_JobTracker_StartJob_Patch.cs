using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

// 当化身(Avatar)开始新工作时,根据工作定义设置时间加速状态
// ModCompatibility: PS

namespace PerspectiveShiftExpanded
{
    [HarmonyPatch(typeof(Pawn_JobTracker), nameof(Pawn_JobTracker.StartJob))]
    public static class Pawn_JobTracker_StartJob_Patch
    {
        public static void Postfix(Pawn_JobTracker __instance, Job newJob)
        {
            if (!ModCompatibility.PerspectiveShift) { return; }

            Pawn pawn = __instance.pawn;
            if (pawn == null || !ModCompatibility.PSE_PS_State_IsAvatar(pawn)) { return; }
            if (newJob == null || newJob.def == null) { return; }
            JobDefsNeedTimeSpeedUp.SetTimeSpeedByJobDef(newJob.def);
        }
    }
}