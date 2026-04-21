using HarmonyLib;
using System.Reflection;
using Verse;
using Verse.AI;

// 当角色为化身(Avatar)时，跳过 CE 的原始检查逻辑，避免玩家角色进入[自动换弹]
// ModCompatibility: CE

namespace PerspectiveShiftExpanded
{
    [StaticConstructorOnStartup]
    public static class CE_JobGiver_CheckReload_TryGiveJob_HarmonyManualPatches
    {
        static CE_JobGiver_CheckReload_TryGiveJob_HarmonyManualPatches()
        {
            if (!ModCompatibility.CombatExpanded) { return; }
            if (ModCompatibility.PSE_CE_JobGiver_CheckReload_TryGiveJobMethod == null) { return; }

            MethodInfo myPrefix = AccessTools.Method(
                typeof(CE_JobGiver_CheckReload_TryGiveJob_Patch),
                nameof(CE_JobGiver_CheckReload_TryGiveJob_Patch.Prefix)
                );

            Startup.harmony.Patch(
                ModCompatibility.PSE_CE_JobGiver_CheckReload_TryGiveJobMethod,
                prefix: new HarmonyMethod(myPrefix)
                );

            Log.Message("[PerspectiveShiftExpanded] 已成功挂载 CombatExtended.CE_JobGiver_CheckReload.TryGiveJob 的前置补丁");
        }
    }

    public static class CE_JobGiver_CheckReload_TryGiveJob_Patch
    {
        public static bool Prefix(object __instance, Pawn pawn, ref Job __result)
        {
            if (pawn == null) { return true; }
            if (ModCompatibility.PSE_PS_State_IsAvatar(pawn))
            {
                __result = null;
                return false;
            }
            return true;
        }
    }
}
