using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.XR;
using Verse;
using Verse.AI;

namespace PerspectiveShiftExpanded
{
    [StaticConstructorOnStartup]
    [HotSwappable]
    public static partial class ModCompatibility
    {
        // Combat Expanded -> PSE_CE
        public static bool CombatExpanded => ModsConfig.IsActive("CETeam.CombatExtended");
        // 类型
        public static readonly Type PSE_CE_CompAmmoUserType = AccessTools.TypeByName("CombatExtended.CompAmmoUser");
        public static readonly Type PSE_CE_VerbShootCEType = AccessTools.TypeByName("CombatExtended.Verb_ShootCE");
        public static readonly Type PSE_CE_JobGiver_CheckReloadType = AccessTools.TypeByName("CombatExtended.JobGiver_CheckReload");
        public static readonly Type PSE_CE_JobGiver_HunkerDownType = AccessTools.TypeByName("CombatExtended.JobGiver_HunkerDown");
        public static readonly Type PSE_CE_JobGiver_ReactToSuppressionType = AccessTools.TypeByName("CombatExtended.JobGiver_ReactToSuppression");
        public static readonly Type PSE_CE_AI_CompReloadType = AccessTools.TypeByName("CombatExtended.AI.CompReload");
        public static readonly Type PSE_CE_CompSuppressableType = AccessTools.TypeByName("CombatExtended.CompSuppressable");
        public static readonly Type PSE_CE_CE_JobDefOfType = AccessTools.TypeByName("CombatExtended.CE_JobDefOf");
        // 属性
        private static readonly PropertyInfo PSE_CE_CompAmmoUser_WielderProp = AccessTools.Property(PSE_CE_CompAmmoUserType, "Wielder");
        private static readonly PropertyInfo PSE_CE_CompAmmoUser_HolderProp = AccessTools.Property(PSE_CE_CompAmmoUserType, "Holder");
        private static readonly PropertyInfo PSE_CE_CompAmmoUser_CurMagCountProp = AccessTools.Property(PSE_CE_CompAmmoUserType, "CurMagCount");
        private static readonly PropertyInfo PSE_CE_CompAmmoUser_MagSizeProp = AccessTools.Property(PSE_CE_CompAmmoUserType, "MagSize");
        // 字段
        private static readonly FieldInfo PSE_CE_CE_JobDefOf_ReloadWeaponField = AccessTools.Field(PSE_CE_CE_JobDefOfType, "ReloadWeapon");
        private static readonly FieldInfo PSE_CE_CE_JobDefOf_RunForCoverField = AccessTools.Field(PSE_CE_CE_JobDefOfType, "RunForCover");
        private static readonly FieldInfo PSE_CE_CE_JobDefOf_HunkerDownField = AccessTools.Field(PSE_CE_CE_JobDefOfType, "HunkerDown");
        public static readonly FieldInfo PSE_CE_CompSuppressable_IsSuppressedField = AccessTools.Field(PSE_CE_CompSuppressableType, "isSuppressed");
        private static readonly FieldInfo PSE_CE_CompSuppressable_CurrentSuppressionField = AccessTools.Field(PSE_CE_CompSuppressableType, "currentSuppression");
        private static readonly FieldInfo PSE_CE_CompSuppressable_TicksUntilDecayField = AccessTools.Field(PSE_CE_CompSuppressableType, "ticksUntilDecay");
        // 方法
        public static readonly MethodInfo PSE_CE_CompAmmoUser_TryStartReloadMethod = AccessTools.Method(PSE_CE_CompAmmoUserType, "TryStartReload");
        public static readonly MethodInfo PSE_CE_CompAmmoUser_SyncedTryStartReloadMethod = AccessTools.Method(PSE_CE_CompAmmoUserType, "SyncedTryStartReload");
        public static readonly MethodInfo PSE_CE_JobGiver_CheckReload_TryGiveJobMethod = AccessTools.Method(PSE_CE_JobGiver_CheckReloadType, "TryGiveJob");
        public static readonly MethodInfo PSE_CE_JobGiver_HunkerDown_TryGiveJobMethod = AccessTools.Method(PSE_CE_JobGiver_HunkerDownType, "TryGiveJob");
        public static readonly MethodInfo PSE_CE_CompSuppressable_AddSuppressionMethod = AccessTools.Method(PSE_CE_CompSuppressableType, "AddSuppression");
        public static readonly MethodInfo PSE_CE_JobGiver_ReactToSuppression_TryGiveJobMethod = AccessTools.Method(PSE_CE_JobGiver_ReactToSuppressionType, "TryGiveJob");
        public static readonly MethodInfo PSE_CE_VerbShootCE_AvailableMethod = AccessTools.Method(PSE_CE_VerbShootCEType, "Available");
        public static readonly MethodInfo PSE_CE_VerbShootCE_OnCastSuccessfulMethod = AccessTools.Method(PSE_CE_VerbShootCEType, "OnCastSuccessful");
        public static readonly MethodInfo PSE_CE_AI_CompReload_StartCastChecksMethod = AccessTools.Method(PSE_CE_AI_CompReloadType, "StartCastChecks");
        
        // 调用或设置参数
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

        public static int PSE_CE_GET_CompAmmoUser_CurMagCount(object compInstance)
        {
            if (compInstance == null || PSE_CE_CompAmmoUser_CurMagCountProp == null) return -1;
            try
            {
                return (int)PSE_CE_CompAmmoUser_CurMagCountProp.GetValue(compInstance);
            }
            catch (Exception ex)
            {
                Log.Error($"[PerspectiveShiftExpanded] 获取 CombatExtended.CompAmmoUser.curMagCountProp 失败: {ex.Message}");
            }
            return -1;
        }

        public static int PSE_CE_GET_CompAmmoUser_MagSize(object compInstance)
        {
            if (compInstance == null || PSE_CE_CompAmmoUser_MagSizeProp == null) return -1;
            try
            {
                return (int)PSE_CE_CompAmmoUser_MagSizeProp.GetValue(compInstance);
            }
            catch (Exception ex)
            {
                Log.Error($"[PerspectiveShiftExpanded] 获取 CombatExtended.CompAmmoUser.magSizeInt 失败: {ex.Message}");
            }
            return -1;
        }

        // 缓存JobDef
        private static JobDef jobDefOf_ReloadWeapon = null;
        private static JobDef jobDefOf_RunForCover = null;
        private static JobDef jobDefOf_HunkerDown = null;
        // 设置记录避免没装CE一直尝试获取
        private static bool jobDefOf_ReloadWeapon_attemptedCEFetch = false;
        private static bool jobDefOf_RunForCover_attemptedCEFetch = false;
        private static bool jobDefOf_HunkerDown_attemptedCEFetch = false;

        public static JobDef PSE_CE_GET_CE_JobDefOf_ReloadWeapon()
        {
            if (jobDefOf_ReloadWeapon != null || jobDefOf_ReloadWeapon_attemptedCEFetch)
            {
                return jobDefOf_ReloadWeapon;
            }

            jobDefOf_ReloadWeapon_attemptedCEFetch = true;
            if (PSE_CE_CE_JobDefOf_ReloadWeaponField == null) return null;
            try
            {
                jobDefOf_ReloadWeapon = (JobDef)PSE_CE_CE_JobDefOf_ReloadWeaponField.GetValue(null);
                return jobDefOf_ReloadWeapon;
            }
            catch (Exception ex)
            {
                Log.Error($"[PerspectiveShiftExpanded] 获取 CombatExtended.CE_JobDefOf.ReloadWeapon 失败: {ex.Message}");
            }
            return null;
        }

        public static JobDef PSE_CE_GET_CE_JobDefOf_RunForCover()
        {
            if (jobDefOf_RunForCover != null || jobDefOf_RunForCover_attemptedCEFetch)
            {
                return jobDefOf_RunForCover;
            }

            jobDefOf_RunForCover_attemptedCEFetch = true;
            if (PSE_CE_CE_JobDefOf_RunForCoverField == null) return null;
            try
            {
                jobDefOf_RunForCover = (JobDef)PSE_CE_CE_JobDefOf_RunForCoverField.GetValue(null);
                return jobDefOf_RunForCover;
            }
            catch (Exception ex)
            {
                Log.Error($"[PerspectiveShiftExpanded] 获取 CombatExtended.CE_JobDefOf.RunForCover 失败: {ex.Message}");
            }
            return null;
        }

        public static JobDef PSE_CE_GET_CE_JobDefOf_HunkerDown()
        {
            if (jobDefOf_HunkerDown != null || jobDefOf_HunkerDown_attemptedCEFetch)
            {
                return jobDefOf_HunkerDown;
            }
            jobDefOf_HunkerDown_attemptedCEFetch = true;
            if (PSE_CE_CE_JobDefOf_HunkerDownField == null) return null;
            try
            {
                jobDefOf_HunkerDown = (JobDef)PSE_CE_CE_JobDefOf_HunkerDownField.GetValue(null);
                return jobDefOf_HunkerDown;
            }
            catch (Exception ex)
            {
                Log.Error($"[PerspectiveShiftExpanded] 获取 CombatExtended.CE_JobDefOf.HunkerDown 失败: {ex.Message}");
            }
            return null;
        }

        public static bool PSE_CE_GET_CompSuppressable_IsSuppressed(object compInstance)
        {
            if (compInstance == null || PSE_CE_CompSuppressable_IsSuppressedField == null) return false;
            try
            {
                return (bool)PSE_CE_CompSuppressable_IsSuppressedField.GetValue(compInstance);
            }
            catch (Exception ex)
            {
                Log.Error($"[PerspectiveShiftExpanded] 获取 CombatExtended.CompSuppressable.isSuppressed 失败: {ex.Message}");
            }
            return false;
        }

        public static void PSE_CE_SET_CompSuppressable_IsSuppressed(object compInstance, bool value)
        {
            if (compInstance == null || PSE_CE_CompSuppressable_IsSuppressedField == null) return;
            PSE_CE_CompSuppressable_IsSuppressedField.SetValue(compInstance, value);
        }

        public static float PSE_CE_GET_CompSuppressable_CurrentSuppression(object compInstance)
        {
            if (compInstance == null || PSE_CE_CompSuppressable_CurrentSuppressionField == null) return 0f;
            try
            {
                return (float)PSE_CE_CompSuppressable_CurrentSuppressionField.GetValue(compInstance);
            }
            catch (Exception ex)
            {
                Log.Error($"[PerspectiveShiftExpanded] 获取 CombatExtended.CompSuppressable.currentSuppression 失败: {ex.Message}");
            }
            return 0f;
        }

        public static void PSE_CE_SET_CompSuppressable_CurrentSuppression(object compInstance, float value)
        {
            if (compInstance == null || PSE_CE_CompSuppressable_CurrentSuppressionField == null) return;
            PSE_CE_CompSuppressable_CurrentSuppressionField.SetValue(compInstance, value);
        }

        public static int PSE_CE_GET_CompSuppressable_TicksUntilDecay(object compInstance)
        {
            if (compInstance == null || PSE_CE_CompSuppressable_TicksUntilDecayField == null) return 0;
            try
            {
                return (int)PSE_CE_CompSuppressable_TicksUntilDecayField.GetValue(compInstance);
            }
            catch (Exception ex)
            {
                Log.Error($"[PerspectiveShiftExpanded] 获取 CombatExtended.CompSuppressable.ticksUntilDecay 失败: {ex.Message}");
            }
            return 0;
        }
                public static void PSE_CE_SET_CompSuppressable_TicksUntilDecay(object compInstance, int value)
        {
            if (compInstance == null || PSE_CE_CompSuppressable_TicksUntilDecayField == null) return;
            PSE_CE_CompSuppressable_TicksUntilDecayField.SetValue(compInstance, value);
        }

        // 调用方法
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