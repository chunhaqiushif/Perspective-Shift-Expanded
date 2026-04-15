using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Verse;
using Verse.AI;
using System.Collections;

namespace PerspectiveShiftExpanded
{
    [StaticConstructorOnStartup]
    [HotSwappable]
    public static class ModCompatibility
    {
        // MOD 兼容性检查
        public static bool CombatExpanded => ModsConfig.IsActive("CETeam.CombatExtended");

        // Perspective Shift -> PSE_PS
        public static readonly Type PSE_PS_AvatarType = AccessTools.TypeByName("PerspectiveShift.Avatar");
        public static readonly Type PSE_PS_StateType = AccessTools.TypeByName("PerspectiveShift.State");

        public static readonly FieldInfo PSE_PS_State_AvatarInstanceField = AccessTools.Field(PSE_PS_StateType, "Avatar");
        private static readonly FieldInfo PSE_PS_Avatar_PawnField = AccessTools.Field(PSE_PS_AvatarType, "pawn");
        private static readonly FieldInfo PSE_PS_Avatar_PassedOutField = AccessTools.Field(PSE_PS_AvatarType, "passedOut");

        public static readonly MethodInfo PSE_PS_IsAvatarMethod = AccessTools.Method(PSE_PS_StateType, "IsAvatar", new[] { typeof(Pawn) });
        public static readonly MethodInfo PSE_PS_Avatar_ProcessMovementMethod = AccessTools.Method(PSE_PS_AvatarType, "ProcessMovement");
        public static readonly MethodInfo PSE_PS_Avatar_HandleAbilityCancellationMethod = AccessTools.Method(PSE_PS_AvatarType, "HandleAbilityCancellation", new[] { typeof(Job) });

        public static Pawn PSE_PS_GET_State_Avatar_Pawn()
        {
            if (PSE_PS_State_AvatarInstanceField == null || PSE_PS_Avatar_PawnField == null) return null;
            try
            {
                // 获取 Avatar 类的静态实例：PerspectiveShift.Avatar.Avatar
                object avatarInstance = PSE_PS_State_AvatarInstanceField.GetValue(null);
                if (avatarInstance != null)
                {
                    // 从 Avatar 实例中获取 pawn 字段的值
                    return (Pawn)PSE_PS_Avatar_PawnField.GetValue(avatarInstance);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[PerspectiveShiftExpanded] 获取 PerspectiveShift.State.Avatar.pawn 失败: {ex.Message}");
            }
            return null;
        }

        public static bool PSE_PS_GET_State_Avatar_PassedOut()
        {
            if (PSE_PS_State_AvatarInstanceField == null || PSE_PS_Avatar_PassedOutField == null) return false;
            try
            {
                // 获取 Avatar 类的静态实例：PerspectiveShift.Avatar.Avatar
                object avatarInstance = PSE_PS_State_AvatarInstanceField.GetValue(null);
                if (avatarInstance != null)
                {
                    // 从 Avatar 实例中获取 passedOut 字段的值
                    return (bool)PSE_PS_Avatar_PassedOutField.GetValue(avatarInstance);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[PerspectiveShiftExpanded] 获取 PerspectiveShift.State.Avatar.PassedOut 失败: {ex.Message}");
            }
            return false;
        }

        public static bool PSE_PS_State_IsAvatar(this Pawn pawn)
        {
            if (pawn == null || PSE_PS_IsAvatarMethod == null) return false;

            try
            {
                return (bool)PSE_PS_IsAvatarMethod.Invoke(null, new object[] { pawn });
            }
            catch (Exception ex)
            {
                Log.Error($"[PerspectiveShiftExpanded] 调用 PerspectiveShift.State.IsAvatar 失败: {ex.Message}");
                return false;
            }
        }


        // Combat Expanded -> PSE_CE
        public static readonly Type PSE_CE_CompAmmoUserType = AccessTools.TypeByName("CombatExtended.CompAmmoUser");
        public static readonly Type PSE_CE_VerbShootCEType = AccessTools.TypeByName("CombatExtended.Verb_ShootCE");
        public static readonly Type PSE_CE_JobGiver_CheckReloadType = AccessTools.TypeByName("CombatExtended.JobGiver_CheckReload");
        public static readonly Type PSE_CE_AI_CompReloadType = AccessTools.TypeByName("CombatExtended.AI.CompReload");
        public static readonly Type PSE_CE_CE_JobDefOfType = AccessTools.TypeByName("CombatExtended.CE_JobDefOf");

        private static readonly PropertyInfo PSE_CE_CompAmmoUser_WielderProp = AccessTools.Property(PSE_CE_CompAmmoUserType, "Wielder");
        private static readonly PropertyInfo PSE_CE_CompAmmoUser_HolderProp = AccessTools.Property(PSE_CE_CompAmmoUserType, "Holder");

        private static readonly FieldInfo PSE_CE_CE_JobDefOf_ReloadWeaponField = AccessTools.Field(PSE_CE_CE_JobDefOfType, "ReloadWeapon");

        public static readonly MethodInfo PSE_CE_CompAmmoUser_TryStartReloadMethod = AccessTools.Method(PSE_CE_CompAmmoUserType, "TryStartReload");
        public static readonly MethodInfo PSE_CE_CompAmmoUser_SyncedTryStartReloadMethod = AccessTools.Method(PSE_CE_CompAmmoUserType, "SyncedTryStartReload");
        public static readonly MethodInfo PSE_CE_JobGiver_CheckReload_TryGiveJobMethod = AccessTools.Method(PSE_CE_JobGiver_CheckReloadType, "TryGiveJob");
        public static readonly MethodInfo PSE_CE_VerbShootCE_AvailableMethod = AccessTools.Method(PSE_CE_VerbShootCEType, "Available");
        public static readonly MethodInfo PSE_CE_VerbShootCE_OnCastSuccessfulMethod = AccessTools.Method(PSE_CE_VerbShootCEType, "OnCastSuccessful");
        public static readonly MethodInfo PSE_CE_AI_CompReload_StartCastChecksMethod = AccessTools.Method(PSE_CE_AI_CompReloadType, "StartCastChecks");

        public static Pawn PSE_CE_GET_CompAmmoUser_Wielder(object compInstance)
        {
            if (compInstance == null || PSE_CE_CompAmmoUser_WielderProp == null) return null;
            try
            {
                return (Pawn)PSE_CE_CompAmmoUser_WielderProp.GetValue(compInstance);
            }
            catch (Exception ex)
            {
                Log.Error($"[PerspectiveShiftExpanded] 获取 CombatExtended.CompAmmoUser.Wielder 失败: {ex.Message}");
            }
            return null;
        }
        public static Pawn PSE_CE_GET_CompAmmoUser_Holder(object compInstance)
        {
            if (compInstance == null || PSE_CE_CompAmmoUser_HolderProp == null) return null;
            try
            {
                return (Pawn)PSE_CE_CompAmmoUser_HolderProp.GetValue(compInstance);
            }
            catch (Exception ex)
            {
                Log.Error($"[PerspectiveShiftExpanded] 获取 CombatExtended.CompAmmoUser.Holder 失败: {ex.Message}");
            }
            return null;
        }

        public static JobDef PSE_CE_GET_CE_JobDefOf_ReloadWeapon()
        {
            if (PSE_CE_CE_JobDefOfType != null)
            {
                return AccessTools.Field(PSE_CE_CE_JobDefOfType, "ReloadWeapon")?.GetValue(null) as JobDef;
            }
            return null;
        }

        public static void PSE_CE_CompAmmoUser_TryStartReload(object compInstance)
        {
            if (compInstance == null) return;
            if (PSE_CE_CompAmmoUserType == null || PSE_CE_CompAmmoUser_TryStartReloadMethod == null) return;
            try
            {
                if (compInstance.GetType() == PSE_CE_CompAmmoUserType)
                {
                    PSE_CE_CompAmmoUser_TryStartReloadMethod.Invoke(compInstance, null);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[PerspectiveShiftExpanded] 调用 CombatExtended.CompAmmoUser.TryStartReload 失败: {ex.Message}");
            }
        }

        public static Job PSE_CE_JobGiver_CheckReload_TryGiveJob(object jobGiverInstance, Pawn pawn)
        {
            if (jobGiverInstance == null || pawn == null) return null;
            if (PSE_CE_JobGiver_CheckReloadType == null || PSE_CE_JobGiver_CheckReload_TryGiveJobMethod == null) return null;
            try
            {
                object[] parameters = new object[] { pawn };

                if (jobGiverInstance.GetType() == PSE_CE_JobGiver_CheckReloadType)
                {
                    return (Job)PSE_CE_JobGiver_CheckReload_TryGiveJobMethod.Invoke(jobGiverInstance, parameters);
                }
                return null;
            }
            catch (Exception ex)
            {
                Log.Error($"[PerspectiveShiftExpanded] 调用 CombatExtended.CompAmmoUser.TryStartReload 失败: {ex.Message}");
            }
            return null;
        }

        public static bool PSE_CE_VerbShootCE_Available(object verbInstance)
        {
            if (verbInstance == null) return false;
            if (PSE_CE_VerbShootCEType == null || PSE_CE_VerbShootCE_AvailableMethod == null) return false;
            try
            {
                if (verbInstance.GetType() == PSE_CE_VerbShootCEType)
                {
                    return (bool)PSE_CE_VerbShootCE_AvailableMethod.Invoke(verbInstance, null);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[PerspectiveShiftExpanded] 调用 CombatExtended.Verb_ShootCE.Available 失败: {ex.Message}");
            }
            return false;
        }

        public static bool PSE_CE_VerbShootCE_OnCastSuccessful(object verbInstance)
        {
            if (verbInstance == null) return false;
            if (PSE_CE_VerbShootCEType == null || PSE_CE_VerbShootCE_OnCastSuccessfulMethod == null) return false;
            try
            {
                if (verbInstance.GetType() == PSE_CE_VerbShootCEType)
                {
                    return (bool)PSE_CE_VerbShootCE_OnCastSuccessfulMethod.Invoke(verbInstance, null);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[PerspectiveShiftExpanded] 调用 CombatExtended.Verb_ShootCE.OnCastSuccessful 失败: {ex.Message}");
            }
            return false;
        }
    }
}