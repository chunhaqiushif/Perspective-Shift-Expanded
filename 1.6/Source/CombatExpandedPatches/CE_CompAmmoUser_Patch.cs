using HarmonyLib;
using RimWorld;
using System.Reflection;
using UnityEngine;
using Verse;

// 当持有者为化身(Avatar)时,拦截除例外情况的所有换弹操作
// 例外情况: 
// 1.设置中禁用了拦截(settings.enableAvatarAutoReload == true);
// 2.临时允许换弹(isAvatarAllowToReload == true);
// ModCompatibility: CE && PS

namespace PerspectiveShiftExpanded
{
    [StaticConstructorOnStartup]
    public static class CE_CompAmmoUser_TryStartReload_HarmonyManualPatches
    {
        static CE_CompAmmoUser_TryStartReload_HarmonyManualPatches()
        {
            if (!ModCompatibility.CombatExpanded) { return; }
            if (!ModCompatibility.PerspectiveShift) { return; }

            if (ModCompatibility.PSE_CE_CompAmmoUser_TryStartReloadMethod == null) { return; }

            MethodInfo myPrefix = AccessTools.Method(typeof(CE_CompAmmoUser_TryStartReload_Patch), nameof(CE_CompAmmoUser_TryStartReload_Patch.Prefix));

            Startup.harmony.Patch(ModCompatibility.PSE_CE_CompAmmoUser_TryStartReloadMethod, prefix: new HarmonyMethod(myPrefix));

            Log.Message("[PerspectiveShiftExpanded] 已成功挂载 CombatExtended.CE_CompAmmoUser.TryStartReload 的前置补丁");
        }
    }


    public static class CE_CompAmmoUser_TryStartReload_Patch
    {
        public static bool Prefix(object __instance)
        {
            if (__instance == null) { return true; }

            Pawn holder = ModCompatibility.PSE_CE_GET_CompAmmoUser_Holder(__instance);
            if (holder == null) { return true; }

            if (ModCompatibility.PSE_PS_State_IsAvatar(holder))
            {
                int curMagCount = ModCompatibility.PSE_CE_GET_CompAmmoUser_CurMagCount(__instance);
                //int magSize = ModCompatibility.PSE_CE_GET_CompAmmoUser_MagSize(__instance);

                //// 拦截弹药全满时触发的换弹
                //if (curMagCount == magSize)
                //{
                //    Log.Message("trystartreload: curMagCount == magSize");
                //    string speechText = "弹药全满!不需要换弹!";
                //    AvatarUtils.AvatarNotify(speechText, SoundDefOf.ClickReject);
                //    return false;
                //}

                // 拦截空仓射击时触发的换弹
                if (curMagCount == 0 && holder.Drafted == true && Input.GetMouseButton(0))
                {
                    string speechText = "弹药耗尽!需要更换弹药!";
                    AvatarUtils.AvatarNotify(speechText, SoundDefOf.ClickReject);
                    return false;
                }

                // UI点击换弹和Shift+R换弹的情况
                if (AvatarUtils.isAllowAvatarToReload)
                {
                    if (!holder.Drafted) { holder.drafter.Drafted = true; }
                    return true;
                }
                // 拦截不允许换弹的其他情况(标识未开启的状况)
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
            if (!ModCompatibility.CombatExpanded) { return; }
            if (!ModCompatibility.PerspectiveShift) { return; }

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
            Pawn holder = ModCompatibility.PSE_CE_GET_CompAmmoUser_Holder(__instance);
            if (holder != null && ModCompatibility.PSE_PS_State_IsAvatar(holder))
            {
                AvatarUtils.isAllowAvatarToReload = true;
            }
        }

        public static void Postfix(object __instance)
        {
            AvatarUtils.isAllowAvatarToReload = false;
        }
    }
}
