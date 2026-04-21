using HarmonyLib;
using System;
using System.Reflection;
using Verse;
using Verse.AI;

namespace PerspectiveShiftExpanded
{
    public static partial class ModCompatibility
    {
        // Perspective Shift -> PSE_PS
        public static bool PerspectiveShift => ModsConfig.IsActive("ferny.PerspectiveShift");
        // 类型
        public static readonly Type PSE_PS_AvatarType = AccessTools.TypeByName("PerspectiveShift.Avatar");
        public static readonly Type PSE_PS_StateType = AccessTools.TypeByName("PerspectiveShift.State");
        // 字段
        public static readonly FieldInfo PSE_PS_State_AvatarField = AccessTools.Field(PSE_PS_StateType, "Avatar");
        private static readonly FieldInfo PSE_PS_Avatar_PawnField = AccessTools.Field(PSE_PS_AvatarType, "pawn");
        private static readonly FieldInfo PSE_PS_Avatar_PassedOutField = AccessTools.Field(PSE_PS_AvatarType, "passedOut");
        // 方法
        public static readonly MethodInfo PSE_PS_State_IsAvatarMethod = AccessTools.Method(PSE_PS_StateType, "IsAvatar", new[] { typeof(Pawn) });
        public static readonly MethodInfo PSE_PS_State_SetAvatarMethod = AccessTools.Method(PSE_PS_StateType, "SetAvatar", new[] { typeof(Pawn), typeof(bool) });
        public static readonly MethodInfo PSE_PS_State_ClearAvatarMethod = AccessTools.Method(PSE_PS_StateType, "ClearAvatar");
        public static readonly MethodInfo PSE_PS_Avatar_UpdateCameraMethod = AccessTools.Method(PSE_PS_AvatarType, "UpdateCamera");
        public static readonly MethodInfo PSE_PS_Avatar_ProcessMovementMethod = AccessTools.Method(PSE_PS_AvatarType, "ProcessMovement");
        public static readonly MethodInfo PSE_PS_Avatar_HandleAbilityCancellationMethod = AccessTools.Method(PSE_PS_AvatarType, "HandleAbilityCancellation", new[] { typeof(Job) });
        // 调用参数
        public static Pawn PSE_PS_GET_State_Avatar_Pawn()
        {
            if (PSE_PS_State_AvatarField == null || PSE_PS_Avatar_PawnField == null) return null;
            try
            {
                // 获取 Avatar 类的静态实例：PerspectiveShift.Avatar.Avatar
                object avatarInstance = PSE_PS_State_AvatarField.GetValue(null);
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
            if (PSE_PS_State_AvatarField == null || PSE_PS_Avatar_PassedOutField == null) return false;
            try
            {
                // 获取 Avatar 类的静态实例：PerspectiveShift.Avatar.Avatar
                object avatarInstance = PSE_PS_State_AvatarField.GetValue(null);
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

        //调用函数
        public static void PSE_PS_State_Avatar_UpdateCamera()
        {
            if (PSE_PS_Avatar_UpdateCameraMethod == null) { return; }
            try
            {
                object instance = PSE_PS_State_AvatarField?.GetValue(null);
                if (instance != null)
                {
                    PSE_PS_Avatar_UpdateCameraMethod.Invoke(instance, null);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[PerspectiveShiftExpanded] 调用 PerspectiveShift.State.UpdateCamera 失败: {ex.Message}");
                return;
            }
        }

        public static bool PSE_PS_State_IsAvatar(this Pawn pawn)
        {
            if (pawn == null || PSE_PS_State_IsAvatarMethod == null) return false;

            try
            {
                return (bool)PSE_PS_State_IsAvatarMethod.Invoke(null, new object[] { pawn });
            }
            catch (Exception ex)
            {
                Log.Error($"[PerspectiveShiftExpanded] 调用 PerspectiveShift.State.IsAvatar 失败: {ex.Message}");
                return false;
            }
        }

    }
}