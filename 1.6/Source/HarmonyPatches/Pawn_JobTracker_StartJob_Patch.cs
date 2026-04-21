using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

// 前置补丁:当化身(Avatar)执行[自动躲避\自动卧倒]时, 跳过执行
// 后置补丁:当化身(Avatar)开始新工作时,根据工作定义设置时间加速状态
// Prefix_ModCompatibility: CE && PS
// Postfix_ModCompatibility: PS 

namespace PerspectiveShiftExpanded
{
    [HarmonyPatch(typeof(Pawn_JobTracker), nameof(Pawn_JobTracker.StartJob))]
    public static class Pawn_JobTracker_StartJob_Patch
    {
        public static bool Prefix(Pawn_JobTracker __instance, Job newJob)
        {
            if (!ModCompatibility.CombatExpanded) { return true; }
            if (!ModCompatibility.PerspectiveShift) { return true; }

            Pawn pawn = __instance.pawn;
            if (pawn == null || !ModCompatibility.PSE_PS_State_IsAvatar(pawn)) { return true; }
            if (newJob == null || newJob.def == null) { return true; }

            JobDef RunForCover = ModCompatibility.PSE_CE_GET_CE_JobDefOf_RunForCover();
            JobDef HunkerDown = ModCompatibility.PSE_CE_GET_CE_JobDefOf_HunkerDown();
            if (RunForCover == null || HunkerDown == null) { return true; }
            if (newJob.def == RunForCover || newJob.def == HunkerDown)
            {
                return false;
            }
            return true;
        }

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