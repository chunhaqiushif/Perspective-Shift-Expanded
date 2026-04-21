using HarmonyLib;
using System.Reflection;
using Verse;
using RimWorld;
using Verse.AI;

// 当角色为化身(Avatar)时，跳过 CE 的原始检查逻辑，避免玩家角色进入[自动卧倒]
// ModCompatibility: CE

namespace PerspectiveShiftExpanded
{
    [StaticConstructorOnStartup]
    public static class CE_JobGiver_ReactToSuppression_TryGiveJob_HarmonyManualPatches
    {
        static CE_JobGiver_ReactToSuppression_TryGiveJob_HarmonyManualPatches()
        {
            if (!ModCompatibility.CombatExpanded) { return; }
            if (ModCompatibility.PSE_CE_JobGiver_ReactToSuppression_TryGiveJobMethod == null) { return; }

            MethodInfo myPrefix = AccessTools.Method(
                typeof(CE_JobGiver_ReactToSuppression_TryGiveJob_Patch),
                nameof(CE_JobGiver_ReactToSuppression_TryGiveJob_Patch.Prefix)
                );

            Startup.harmony.Patch(
                ModCompatibility.PSE_CE_JobGiver_ReactToSuppression_TryGiveJobMethod,
                prefix: new HarmonyMethod(myPrefix)
                );

            Log.Message("[PerspectiveShiftExpanded] 已成功挂载 CombatExtended.CE_JobGiver_ReactToSuppression.TryGiveJob 的前置补丁");
        }
        
    }

    public static class CE_JobGiver_ReactToSuppression_TryGiveJob_Patch
    {
        public static bool Prefix(object __instance, Pawn pawn, ref Job __result)
        {
            Log.Message("[PerspectiveShiftExpanded] CE_JobGiver_ReactToSuppression.TryGiveJob 前置补丁被触发");
            if (pawn == null) { return true; }
            if (ModCompatibility.PSE_PS_State_IsAvatar(pawn))
            {
                __result = null;
                AvatarUtils.AvatarNotify("我被压制了! 快寻找掩体!", SoundDefOf.Tick_High);
                return false;
            }
            return true;
        }
    }
}
