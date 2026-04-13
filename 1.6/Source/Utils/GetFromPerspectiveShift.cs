using System;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace PerspectiveShiftExpanded
{
    [StaticConstructorOnStartup]
    public static class GetFromPerspectiveShift
    {
        // 1. 定义类型
        private static readonly Type AvatarType = AccessTools.TypeByName("PerspectiveShift.Avatar");
        private static readonly Type StateType = AccessTools.TypeByName("PerspectiveShift.State");

        // 2. 定义字段和方法
        // 注意：根据源码，Avatar 类通常有一个静态属性或字段叫 "Avatar" (或 Instance)
        private static readonly FieldInfo AvatarInstanceField = AccessTools.Field(StateType, "Avatar");
        private static readonly FieldInfo PawnField = AccessTools.Field(AvatarType, "pawn");

        // IsAvatar(this Pawn pawn) 在反射中就是一个接收 Pawn 参数的静态方法
        private static readonly MethodInfo IsAvatarMethod = AccessTools.Method(StateType, "IsAvatar", new[] { typeof(Pawn) });

        /// <summary>
        /// 获取 PerspectiveShift.Avatar.pawn
        /// </summary>
        public static Pawn GetAvatarPawn()
        {
            if (AvatarInstanceField == null || PawnField == null) return null;
            try
            {
                // 获取 Avatar 类的静态实例：PerspectiveShift.Avatar.Avatar
                object avatarInstance = AvatarInstanceField.GetValue(null);
                if (avatarInstance != null)
                {
                    return (Pawn)PawnField.GetValue(avatarInstance);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[PerspectiveShiftExpanded] 获取 Pawn 失败: {ex.Message}");
            }
            return null;
        }

        /// <summary>
        /// 调用 PerspectiveShift.State.IsAvatar(pawn)
        /// 检查指定的 pawn 是否为当前的化身
        /// </summary>
        public static bool IsAvatar(Pawn pawn)
        {
            if (pawn == null || IsAvatarMethod == null) return false;

            try
            {
                // 扩展方法在反射调用时，第一个参数就是 target pawn
                return (bool)IsAvatarMethod.Invoke(null, new object[] { pawn });
            }
            catch (Exception ex)
            {
                Log.Error($"[PerspectiveShiftExpanded] 调用 IsAvatar 失败: {ex.Message}");
                return false;
            }
        }

        // 快捷逻辑：直接判断当前是否有化身正在运行
        public static bool IsModLoaded() => IsAvatarMethod != null && PawnField != null;
    }
}
