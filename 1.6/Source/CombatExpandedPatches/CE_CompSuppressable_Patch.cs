using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Verse;
using Verse.AI;

namespace PerspectiveShiftExpanded
{
    [StaticConstructorOnStartup]
    public static class CE_CompSuppressable_AddSuppression_HarmonyManualPatches
    {
        static CE_CompSuppressable_AddSuppression_HarmonyManualPatches()
        {
            if (ModCompatibility.PSE_PS_Avatar_ProcessMovementMethod == null) { return; }
            if (!ModCompatibility.CombatExpanded) { return; }

            MethodInfo myTranspiler = AccessTools.Method(
                typeof(CE_CompSuppressable_AddSuppression_Patch),
                nameof(CE_CompSuppressable_AddSuppression_Patch.Transpiler)
                );

            Startup.harmony.Patch(
                ModCompatibility.PSE_CE_CompSuppressable_AddSuppressionMethod,
                transpiler: new HarmonyMethod(myTranspiler)
                );

            Log.Message("[PerspectiveShiftExpanded] 已成功挂载 CombatExtended.CompSuppressable.AddSuppression 的转译补丁");
        }
    }
    public static class CE_CompSuppressable_AddSuppression_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            return CE_CompSuppressable_AddSuppression_Transpiler.DoTranspile(instructions, generator);
        }
    }

    public static class CE_CompSuppressable_AddSuppression_Transpiler
    {
        public static bool Wrap_HandleAvatarSuppression(object comp)
        {
            // 转换为 ThingComp 以获取 parent (Pawn)
            if (comp is ThingComp thingComp)
            {
                Pawn pawn = thingComp.parent as Pawn;
                if (pawn != null && pawn.PSE_PS_State_IsAvatar())
                {
                    float temp = ModCompatibility.PSE_CE_GET_CompSuppressable_CurrentSuppression(comp);
                    AvatarUtils.AvatarNotify($"我被压制了! {temp}", SoundDefOf.Tick_High);
                    Log.Message($"{temp}");
                    return true; // 返回 true 表示是 Avatar，触发拦截跳转
                }
            }
            return false;
        }

        public static IEnumerable<CodeInstruction> DoTranspile(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var codes = instructions.ToList();
            // 获取 isSuppressed 字段
            FieldInfo isSuppressedField = ModCompatibility.PSE_CE_CompSuppressable_IsSuppressedField;

            // 获取 Wrap_HandleAvatarSuppression 方法
            MethodInfo wrapMethod = AccessTools.Method(
                typeof(CE_CompSuppressable_AddSuppression_Transpiler),
                nameof(Wrap_HandleAvatarSuppression)
            );

            // 创建跳转标签
            Label skipOriginalLogicLabel = generator.DefineLabel();

            bool found = false;
            int stfldIndex = -1;

            // 第一遍：找到 stfld isSuppressed 的位置
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Stfld && (FieldInfo)codes[i].operand == isSuppressedField)
                {
                    // 确认前面是 ldc.i4.1
                    if (i > 0 && codes[i - 1].opcode == OpCodes.Ldc_I4_1)
                    {
                        stfldIndex = i;
                        found = true;
                        break;
                    }
                }
            }

            if (!found)
            {
                Log.Error("[PerspectiveShiftExpanded] 无法找到 isSuppressed = true 的指令位置");
                return codes;
            }

            // 构建要插入的指令
            var insertInstructions = new List<CodeInstruction>
    {
        // ldarg.0 (加载 this)
        new CodeInstruction(OpCodes.Ldarg_0),
        // call Wrap_HandleAvatarSuppression
        new CodeInstruction(OpCodes.Call, wrapMethod),
        // brtrue.s skipLabel (如果返回 true，跳转)
        new CodeInstruction(OpCodes.Brtrue_S, skipOriginalLogicLabel)
    };

            // 在 stfld 指令之后插入
            codes.InsertRange(stfldIndex + 1, insertInstructions);

            // 找到整个 if 块的结束位置来放置标签
            // 方法：从插入位置向后查找，找到第一个 ret 指令或者离开当前作用域的 br 指令
            int labelPlaceIndex = -1;
            bool inIfBlock = true;

            for (int i = stfldIndex + 1 + insertInstructions.Count; i < codes.Count; i++)
            {
                // 简单的块深度检测
                if (codes[i].opcode == OpCodes.Brfalse || codes[i].opcode == OpCodes.Brfalse_S ||
                    codes[i].opcode == OpCodes.Brtrue || codes[i].opcode == OpCodes.Brtrue_S)
                {
                    if (inIfBlock)
                    {
                        // 这是 if 块内的跳转，继续
                        continue;
                    }
                }

                // 找到 ret 指令（方法结束）
                if (codes[i].opcode == OpCodes.Ret)
                {
                    labelPlaceIndex = i;
                    break;
                }

                // 找到离开 if 块的无条件跳转
                if (codes[i].opcode == OpCodes.Br || codes[i].opcode == OpCodes.Br_S)
                {
                    labelPlaceIndex = i;
                    break;
                }
            }

            // 如果找到了合适的位置，添加标签
            if (labelPlaceIndex != -1)
            {
                if (codes[labelPlaceIndex].labels == null)
                    codes[labelPlaceIndex].labels = new List<Label>();
                codes[labelPlaceIndex].labels.Add(skipOriginalLogicLabel);
            }
            else
            {
                // 备选：在方法末尾添加标签
                if (codes[codes.Count - 1].labels == null)
                    codes[codes.Count - 1].labels = new List<Label>();
                codes[codes.Count - 1].labels.Add(skipOriginalLogicLabel);
            }

            return codes;

        }
    }
}
