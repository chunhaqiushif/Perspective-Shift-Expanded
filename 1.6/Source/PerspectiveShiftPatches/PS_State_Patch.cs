using HarmonyLib;
using RimWorld;
using Verse;
using System;
using System.Reflection;

// 当附身角色时, 取消和成为Avatar的Pawn执行一次需求检测
// ModCompatibility: PS

namespace PerspectiveShiftExpanded
{
    [StaticConstructorOnStartup]
    public static class PS_State_SetAvatar_HarmonyManualPatches
    {
        static PS_State_SetAvatar_HarmonyManualPatches()
        {
            if (!ModCompatibility.PerspectiveShift) { return; }
            if (ModCompatibility.PSE_PS_State_SetAvatarMethod == null) { return; }

            MethodInfo myPrefix = AccessTools.Method(typeof(PS_State_SetAvatar_Patch), nameof(PS_State_SetAvatar_Patch.Prefix));
            MethodInfo myPostfix = AccessTools.Method(typeof(PS_State_SetAvatar_Patch), nameof(PS_State_SetAvatar_Patch.Postfix));

            Startup.harmony.Patch(ModCompatibility.PSE_PS_State_SetAvatarMethod, prefix: new HarmonyMethod(myPrefix));
            Startup.harmony.Patch(ModCompatibility.PSE_PS_State_SetAvatarMethod, postfix: new HarmonyMethod(myPostfix));

            Log.Message("[PerspectiveShiftExpanded] 已成功挂载 PerspectiveShift.State.SetAvatar 的前后置补丁");
        }
    }
    public static class PS_State_SetAvatar_Patch
    {
        public static Pawn lastPawn;
        // 因为IsAvatar可能在此时还未更新, 所以需要一个临时变量来存储当前正在设置为Avatar的Pawn
        public static Pawn forcingAvatarPawn;
        public static void Prefix()
        {
            lastPawn = ModCompatibility.PSE_PS_GET_State_Avatar_Pawn();
        }

        public static void Postfix(Pawn pawn)
        {
            try
            {
                forcingAvatarPawn = pawn;
                if (lastPawn != null)
                {
                    lastPawn.needs?.AddOrRemoveNeedsAsAppropriate();
                }
                if (pawn != null)
                {
                    pawn.needs?.AddOrRemoveNeedsAsAppropriate();
                }
            }
            finally
            {
                forcingAvatarPawn = null;
                lastPawn = null;
            }
        }
    }

    [StaticConstructorOnStartup]
    public static class PS_State_ClearAvatar_HarmonyManualPatches
    {
        static PS_State_ClearAvatar_HarmonyManualPatches()
        {
            if (!ModCompatibility.PerspectiveShift) { return; }
            if (ModCompatibility.PSE_PS_State_ClearAvatarMethod == null) { return; }

            MethodInfo myPrefix = AccessTools.Method(typeof(PS_State_ClearAvatar_Patch), nameof(PS_State_ClearAvatar_Patch.Prefix));
            MethodInfo myPostfix = AccessTools.Method(typeof(PS_State_ClearAvatar_Patch), nameof(PS_State_ClearAvatar_Patch.Postfix));

            Startup.harmony.Patch(ModCompatibility.PSE_PS_State_ClearAvatarMethod, prefix: new HarmonyMethod(myPrefix));
            Startup.harmony.Patch(ModCompatibility.PSE_PS_State_ClearAvatarMethod, postfix: new HarmonyMethod(myPostfix));

            Log.Message("[PerspectiveShiftExpanded] 已成功挂载 PerspectiveShift.State.ClearAvatar 的前后置补丁");
        }
    }
    public static class PS_State_ClearAvatar_Patch
    {
        public static Pawn pawnToRestore;
        public static void Prefix()
        {
            pawnToRestore = ModCompatibility.PSE_PS_GET_State_Avatar_Pawn();
        }

        public static void Postfix()
        {
            if (pawnToRestore != null)
            {
                pawnToRestore.needs?.AddOrRemoveNeedsAsAppropriate();
                pawnToRestore = null;
            }
        }
    }
}
