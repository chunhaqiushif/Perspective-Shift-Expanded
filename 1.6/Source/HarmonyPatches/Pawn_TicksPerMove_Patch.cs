using HarmonyLib;
using RimWorld;
using Verse;

// 当化身(Avatar)执行换弹工作时,当前移动速度会被乘以一个减速系数:
// (1+射击能力等级/调整常数)
// ModCompatibility: PS && CE

namespace PerspectiveShiftExpanded
{
    [HarmonyPatch(typeof(Pawn), nameof(Pawn.TicksPerMove))]
    public static class Pawn_TicksPerMove_Patch
    {
        public static void Postfix(Pawn __instance, ref float __result)
        {
            if (!ModCompatibility.PerspectiveShift) return;
            if (!ModCompatibility.CombatExpanded) return;
            if (!__instance.PSE_PS_State_IsAvatar()) return;

            var reloadJobDef = ModCompatibility.PSE_CE_GET_CE_JobDefOf_ReloadWeapon();
            if (reloadJobDef != null && __instance.CurJobDef == reloadJobDef)
            {
                // 调整常数(调整常数越大，高等级收益越小)
                float adjustmentConstant = PerspectiveShiftExpandedMod.settings.reloadTickPerMoveAdjustmentConstant;
                // 射击能力等级
                int level = __instance.skills?.GetSkill(SkillDefOf.Shooting).Level ?? 0;
                // 基础惩罚值
                float maxPenalty = 0.9f;
                // 惩罚值
                float currentPenalty = maxPenalty / (1f + (level / adjustmentConstant));
                // 保底有10%的惩罚
                float finalPenalty = UnityEngine.Mathf.Max(currentPenalty, 0.1f);

                __result *= (1.0f + finalPenalty);
            }
        }
    }
}
