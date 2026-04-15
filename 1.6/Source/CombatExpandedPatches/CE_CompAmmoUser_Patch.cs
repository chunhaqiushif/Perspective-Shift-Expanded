using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace PerspectiveShiftExpanded
{
    [StaticConstructorOnStartup]
    public static class CE_CompAmmoUser_TryStartReload_HarmonyManualPatches
    {
        static CE_CompAmmoUser_TryStartReload_HarmonyManualPatches()
        {
            if (ModCompatibility.PSE_CE_CompAmmoUser_TryStartReloadMethod == null) { return; }

            MethodInfo myPrefix = AccessTools.Method(
                typeof(CE_CompAmmoUser_TryStartReload_Patch),
                nameof(CE_CompAmmoUser_TryStartReload_Patch.Prefix)
                );

            Startup.harmony.Patch(
                ModCompatibility.PSE_CE_CompAmmoUser_TryStartReloadMethod,
                prefix: new HarmonyMethod(myPrefix)
                );

            Log.Message("[PerspectiveShiftExpanded] 已成功挂载 CombatExtended.CE_CompAmmoUser.TryStartReload 的前置补丁");
        }
    }

    /// <summary>
    /// 当持有者为化身时,拦截除例外情况的所有换弹操作
    /// <br/>例外情况: 
    /// <br/>1.设置中禁用了拦截(settings.enableAvatarAutoReload == true);
    /// <br/>2.临时允许换弹(isAvatarAllowToReload == true);
    /// </summary>
    public static class CE_CompAmmoUser_TryStartReload_Patch
    {

        public static bool Prefix(object __instance)
        {
            if (__instance == null) { return true; }

            Pawn holder = ModCompatibility.PSE_CE_GET_CompAmmoUser_Holder(__instance);
            if (holder == null) { return true; }

            if (ModCompatibility.PSE_PS_State_IsAvatar(holder))
            {
                // 启用自动换弹意味着始终允许换弹,所以不进行拦截
                if (PerspectiveShiftExpandedMod.settings.enableAvatarAutoReload)
                {
                    return true;
                }

                // 允许换弹的临时开关,默认为false,当为true时允许换弹,并且在换弹操作后会被重置为false
                if (AvatarFlag.isAllowAvatarToReload)
                {
                    return true;
                }
                return false;
            }
            return true;
        }
    }

    [StaticConstructorOnStartup]
    public static class CE_CompAmmoUser_SyncedTryStartReload_HarmonyManualPatches
    {
        static CE_CompAmmoUser_SyncedTryStartReload_HarmonyManualPatches()
        {
            if (ModCompatibility.PSE_CE_CompAmmoUser_SyncedTryStartReloadMethod == null) { return; }
            MethodInfo myPrefix = AccessTools.Method(
                typeof(CE_CompAmmoUser_SyncedTryStartReload_Patch),
                nameof(CE_CompAmmoUser_SyncedTryStartReload_Patch.Prefix)
                );
            MethodInfo myPostfix = AccessTools.Method(
                typeof(CE_CompAmmoUser_SyncedTryStartReload_Patch),
                nameof(CE_CompAmmoUser_SyncedTryStartReload_Patch.Postfix)
                );
            Startup.harmony.Patch(
                ModCompatibility.PSE_CE_CompAmmoUser_SyncedTryStartReloadMethod,
                prefix: new HarmonyMethod(myPrefix),
                postfix: new HarmonyMethod(myPostfix)
                );
            Log.Message("[PerspectiveShiftExpanded] 已成功挂载 CombatExtended.CE_CompAmmoUser.SyncedTryStartReload 的前后置补丁");
        }


    }

    /// <summary>
    /// 阻止Avatar换弹行为(除非按下按钮或Shift+R换弹)
    /// <br/>当角色为Avatar时,临时开启允许换弹标志, 执行后关闭换弹标志
    /// </summary>
    public static class CE_CompAmmoUser_SyncedTryStartReload_Patch
    {
        public static void Prefix(object __instance)
        {
            // 如果点击按钮的是 Avatar，开启临时允许标志
            Pawn holder = ModCompatibility.PSE_CE_GET_CompAmmoUser_Holder(__instance);
            if (holder != null && ModCompatibility.PSE_PS_State_IsAvatar(holder))
            {
                AvatarFlag.isAllowAvatarToReload = true;
            }
        }

        public static void Postfix(object __instance)
        {
            // 执行完后立刻关掉，恢复拦截状态
            AvatarFlag.isAllowAvatarToReload = false;
        }
    }
}
