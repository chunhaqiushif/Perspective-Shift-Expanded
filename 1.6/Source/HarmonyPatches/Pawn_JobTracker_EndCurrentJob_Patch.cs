using HarmonyLib;
using RimWorld;
using System;
using System.Reflection;
using Verse;
using Verse.AI;

// 当化身(Avatar)结束当前工作时,重置时间加速状态
// 当CE换弹结束时, 弹出提示
// ModCompatibility: PS && CE

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
            if (!ModCompatibility.PerspectiveShift) { __state = null; return; }

            __state = new JobStateSnapshot();
            if (__instance.curJob == null) { return; }

            __state.def = __instance.curJob.def;
            __state.playerForced = __instance.curJob.playerForced;
            __state.targetA = __instance.curJob.targetA;

            if (ModCompatibility.PSE_PS_State_IsAvatar(__instance.pawn))
            {
                // PS 重置时间加速
                JobDefsNeedTimeSpeedUp.ResetTimeSpeed(__state.def);
                // CE 换弹结束报告
                if (ModCompatibility.CombatExpanded && __state.def == ModCompatibility.PSE_CE_GET_CE_JobDefOf_ReloadWeapon())
                {
                    // TODO: 这里会因为中途被打断而导致换弹未完成, 但仍然会提示换弹完成, 暂时先这样
                    AvatarUtils.AvatarNotify("换弹完成!", SoundDefOf.Tick_High);
                }
            }
        }
    }
}