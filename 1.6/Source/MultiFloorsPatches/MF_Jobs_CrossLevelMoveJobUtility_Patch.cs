using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using Verse;

// 当角色为化身(Avatar)时，跳过 MultiFloors 的原始检查逻辑，避免玩家角色进入自动换层
// ModCompatibility: MF && PS

namespace PerspectiveShiftExpanded
{
    [StaticConstructorOnStartup]
    public static class MF_Jobs_CrossLevelMoveJobUtility_OnOrderedToSwitchLevel_HarmonyManualPatches
    {
        static MF_Jobs_CrossLevelMoveJobUtility_OnOrderedToSwitchLevel_HarmonyManualPatches()
        {
            if (!ModCompatibility.MultiFloors) { return; }
            if (!ModCompatibility.PerspectiveShift) {  return; }

            MethodInfo myPrefix = AccessTools.Method(
                typeof(MF_Jobs_CrossLevelMoveJobUtility_OnOrderedToSwitchLevel_Patch),
                nameof(MF_Jobs_CrossLevelMoveJobUtility_OnOrderedToSwitchLevel_Patch.Prefix)
                );

            Startup.harmony.Patch(
                ModCompatibility.PSE_MF_Jobs_CrossLevelMoveJobUtility_OnOrderedToSwitchLevelMethod,
                prefix: new HarmonyMethod(myPrefix)
                );

            Log.Message("[PerspectiveShiftExpanded] 已成功挂载 MultiFloors.Jobs.CrossLevelMoveJobUtility.OnOrderedToSwitchLevel 的前置补丁");
        }
    }

    public static class MF_Jobs_CrossLevelMoveJobUtility_OnOrderedToSwitchLevel_Patch
    {
        public static void Prefix(Pawn pawn, ref bool drafted)
        {
            if (pawn == null) { return; }
            if (ModCompatibility.PSE_PS_State_IsAvatar(pawn))
            {
                drafted = pawn.Drafted; // 保持化身的原始状态，不改变
            }
        }
    }
}
