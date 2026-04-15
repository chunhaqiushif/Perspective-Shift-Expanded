using HarmonyLib;
using NAudio.SoundFont;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using Verse;
using Verse.AI;

// 1. 化身(Avatar)的按键绑定监控: 换弹\阅读\选中单位等功能
// 2. 化身进行ProcessMovement 移动时防止其中断部分操作(比如换弹)
// ModCompatibility: CE && PS

namespace PerspectiveShiftExpanded
{
    [StaticConstructorOnStartup]
    public static class PS_Avatar_OnGUI_HarmonyManualPatches
    {
        static PS_Avatar_OnGUI_HarmonyManualPatches()
        {
            if (!ModCompatibility.CombatExpanded) { return; }
            if (!ModCompatibility.PerspectiveShift) { return; }

            Type avatarType = AccessTools.TypeByName("PerspectiveShift.Avatar");
            if (avatarType == null) return;

            MethodInfo originalOnGUI = AccessTools.Method(avatarType, "OnGUI");
            if (originalOnGUI == null) return;

            MethodInfo myPostfix = AccessTools.Method(typeof(PS_Avatar_OnGUI_Patch), nameof(PS_Avatar_OnGUI_Patch.Postfix));
            MethodInfo myPrefix = AccessTools.Method(typeof(PS_Avatar_OnGUI_Patch), nameof(PS_Avatar_OnGUI_Patch.Prefix));

            Startup.harmony.Patch(originalOnGUI, prefix: new HarmonyMethod(myPrefix));
            Startup.harmony.Patch(originalOnGUI, postfix: new HarmonyMethod(myPostfix));

            Log.Message("[PerspectiveShiftExpanded] 已成功挂载 PerspectiveShift.Avatar.OnGUI 的前后置补丁");
        }
    }

    public static class PS_Avatar_OnGUI_Patch
    {
        public static void Prefix(object __instance)
        {
            HandleReloadWeaponBinding();
        }

        public static void Postfix(object __instance)
        {
            HandleReadingBinding();
            HandleSelectAvatarBindings();
        }

        private static void HandleReloadWeaponBinding()
        {
            if (Find.TickManager.CurTimeSpeed == TimeSpeed.Paused) { return; }

            if (!(Event.current.type == EventType.KeyDown && Event.current.shift
                && DefsOf.PSE_CE_AvatarReload.KeyDownEvent))
            { return; }

            Pawn pawn = ModCompatibility.PSE_PS_GET_State_Avatar_Pawn();

            // Avatar倒下\精神状态\晕倒时跳出
            bool pawn_PassedOut = ModCompatibility.PSE_PS_GET_State_Avatar_PassedOut();
            if (pawn.Downed || pawn.InMentalState || pawn_PassedOut) return;

            var comps = pawn.equipment.Primary.AllComps;
            ThingComp comp_tryStartReload = null;
            foreach (var comp in comps)
            {
                if (comp.GetType() == ModCompatibility.PSE_CE_CompAmmoUserType)
                {
                    comp_tryStartReload = comp;
                    break;
                }
            }

            if (comp_tryStartReload != null)
            {
                AvatarFlag.isAllowAvatarToReload = true;
                ModCompatibility.PSE_CE_CompAmmoUser_TryStartReload(comp_tryStartReload);
                AvatarFlag.isAllowAvatarToReload = false;
                Event.current.Use();    // 消耗掉事件，跳过后续事件(默认按键R为征召)
                return;
            }
            return;
        }

        private static void HandleReadingBinding()
        {
            if (!DefsOf.PSE_ReadBook.KeyDownEvent) { return; }
            if (Find.TickManager.CurTimeSpeed == TimeSpeed.Paused) { return; }

            Pawn pawn = ModCompatibility.PSE_PS_GET_State_Avatar_Pawn();
            if (pawn?.inventory == null) { return; }

            // 不止选择1个单位\倒下\精神状态\晕倒时跳出
            bool passedOut = ModCompatibility.PSE_PS_GET_State_Avatar_PassedOut();
            bool onlyAvatarSelected = Find.Selector.NumSelected == 0 || (Find.Selector.NumSelected == 1 && Find.Selector.IsSelected(pawn));
            if (!onlyAvatarSelected || pawn.Downed || pawn.InMentalState || passedOut) return;

            List<Thing> booksInInventory = new List<Thing>();
            foreach (Thing item in pawn.inventory.innerContainer)
            {
                if (item.def.defName.Contains("Tome") | item.def.defName.Contains("TextBook"))
                {
                    booksInInventory.Add(item);
                }
            }
            if (booksInInventory.Count == 0)
            {
                return;
            }
            int randomIndex = Rand.Range(0, booksInInventory.Count);
            Thing book = booksInInventory[randomIndex];

            Job readingJob = JobMaker.MakeJob(DefsOf.PSE_AvatarReading, book);
            pawn.jobs.TryTakeOrderedJob(readingJob, JobTag.Idle);
        }

        private static void HandleSelectAvatarBindings()
        {
            if (!DefsOf.PSE_SelectAvatar.KeyDownEvent)
            {
                return;
            }
            Pawn pawn = ModCompatibility.PSE_PS_GET_State_Avatar_Pawn();
            if (pawn != null && pawn.Spawned)
            {
                Find.Selector.ClearSelection();
                Find.Selector.Select(pawn);
                CameraJumper.TryJump(pawn);
            }
        }
    }

    [StaticConstructorOnStartup]
    public static class PS_Avatar_ProcessMovement_HarmonyManualPatches
    {
        static PS_Avatar_ProcessMovement_HarmonyManualPatches()
        {
            if (ModCompatibility.PSE_PS_Avatar_ProcessMovementMethod == null) { return; }
            // 需要兼容CE
            if (!ModCompatibility.CombatExpanded) { return; }

            MethodInfo myTranspiler = AccessTools.Method(
                typeof(PS_Avatar_ProcessMovement_Patch),
                nameof(PS_Avatar_ProcessMovement_Patch.Transpiler)
                );

            Startup.harmony.Patch(
                ModCompatibility.PSE_PS_Avatar_ProcessMovementMethod,
                transpiler: new HarmonyMethod(myTranspiler)
                );

            Log.Message("[PerspectiveShiftExpanded] 已成功挂载 PerspectiveShift.Avatar.ProcessMovement 的转译补丁");
        }
    }
    public static class PS_Avatar_ProcessMovement_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var codes = PS_Avatar_ProcessMovement_Transpiler.DoTranspile_PreventInterruptJobs(instructions, generator);
            return PS_Avatar_ProcessMovement_Transpiler.DoTranspile_RemoveInterruptDelay(codes);

        }
    }
    public static class PS_Avatar_ProcessMovement_Transpiler
    {
        private static List<JobDef> PreventInterruptJobs = new List<JobDef>
        {
            ModCompatibility.PSE_CE_GET_CE_JobDefOf_ReloadWeapon(),
        };

        // 逻辑：如果 RunAndGun 激活，或者是我们列表中的 Job，则返回 true
        public static bool Wrap_HandleAbilityCancellation(object avatarInstance, Job curJob)
        {
            // 1. 获取 pawn 实例
            // 假设通过其一中的结构，avatarInstance 内部有 pawn 字段
            var pawnField = AccessTools.Field(avatarInstance.GetType(), "pawn");
            Pawn pawn = pawnField?.GetValue(avatarInstance) as Pawn;
            // 2. 判断是否在免中断列表中
            if (pawn != null && curJob != null && PreventInterruptJobs.Contains(curJob.def))
            {
                return true; // 触发拦截
            }

            // 3. 否则执行原有的 HandleAbilityCancellation
            // 使用反射调用其三中的私有方法
            var originalMethod = AccessTools.Method(avatarInstance.GetType(), "HandleAbilityCancellation", new[] { typeof(Job) });
            originalMethod?.Invoke(avatarInstance, new object[] { curJob });

            return false; // 继续执行后续逻辑（即 EndCurrentJob）
        }

        public static IEnumerable<CodeInstruction> DoTranspile_PreventInterruptJobs(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var codes = instructions.ToList();

            // 获取原方法引用 (其三)
            var avatarType = ModCompatibility.PSE_PS_AvatarType;
            var originalCancelMethod = ModCompatibility.PSE_PS_Avatar_HandleAbilityCancellationMethod;

            // 获取 EndCurrentJob 引用
            var endJobMethod = AccessTools.Method(typeof(Pawn_JobTracker), nameof(Pawn_JobTracker.EndCurrentJob));

            for (int i = 0; i < codes.Count; i++)
            {
                // 步骤 A: 找到 HandleAbilityCancellation 的调用
                if (codes[i].Calls(originalCancelMethod))
                {
                    // 此时栈上是 [avatarInstance, curJob]
                    // 我们将其替换为调用我们的包装方法
                    codes[i] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PS_Avatar_ProcessMovement_Transpiler), nameof(Wrap_HandleAbilityCancellation)));

                    // 此时栈上剩下一个 bool (WrapHandleAbilityCancellation 的返回值)
                    // 步骤 B: 寻找紧随其后的 EndCurrentJob 调用，并用 if(bool) 跳过它
                    for (int j = i + 1; j < codes.Count; j++)
                    {
                        if (codes[j].Calls(endJobMethod))
                        {
                            // 创建一个跳转标签，指向 EndCurrentJob 之后的那条指令
                            Label labelAfterEndJob = generator.DefineLabel();
                            codes[j + 1].labels.Add(labelAfterEndJob);

                            // 在 i+1 处（即 Call 包装方法之后）插入跳转指令
                            // 如果返回 true (在列表中)，则跳转到 EndCurrentJob 之后
                            codes.Insert(i + 1, new CodeInstruction(OpCodes.Brtrue, labelAfterEndJob));
                            break;
                        }
                    }
                }
            }
            return codes;
        }

        public static IEnumerable<CodeInstruction> DoTranspile_RemoveInterruptDelay(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            var avatarType = ModCompatibility.PSE_PS_AvatarType;
            var durationField = AccessTools.Field(avatarType, "moveInputDuration");

            if (durationField == null) return codes;

            for (int i = 0; i < codes.Count; i++)
            {
                // 定位 moveInputDuration 的加载
                if (codes[i].LoadsField(durationField))
                {
                    // 此时栈结构通常是：... && (!isOurWaitJob) [可能已经存入局部变量或还在栈上]
                    // 然后开始判断 moveInputDuration > 0.35f

                    // 我们寻找后续最近的一个比较跳转指令 (brfalse, ble, blt 等)
                    // 这个跳转指令负责在 "moveInputDuration <= 0.35f" 时跳过 if 块内部
                    for (int j = i + 1; j < i + 10 && j < codes.Count; j++)
                    {
                        // 如果找到了比较跳转指令
                        if (codes[j].opcode == OpCodes.Ble || codes[j].opcode == OpCodes.Ble_S ||
                            codes[j].opcode == OpCodes.Ble_Un || codes[j].opcode == OpCodes.Ble_Un_S ||
                            codes[j].opcode == OpCodes.Brfalse || codes[j].opcode == OpCodes.Brfalse_S)
                        {
                            // 我们把从加载 moveInputDuration 到这个跳转指令之前的所有内容全部 Nop 掉
                            // 并且把跳转指令本身也 Nop 掉
                            // 这样 logic 就会“流向” if 块内部，不再受 0.35f 的约束

                            int startIdx = i;
                            // 如果前面有 ldarg.0 也一并处理
                            if (i > 0 && codes[i - 1].opcode == OpCodes.Ldarg_0) startIdx = i - 1;

                            for (int k = startIdx; k <= j; k++)
                            {
                                // 保留可能存在的标签，防止破坏其他地方跳过来的逻辑
                                codes[k].opcode = OpCodes.Nop;
                                codes[k].operand = null;
                            }

                            Log.Message("[PerspectiveShiftExpanded] 已移除 PerspectiveShift.Avatar.ProcessMovement的moveInputDuration 比较代码");
                            return codes;
                        }
                    }
                }
            }
            return codes;
        }
    }
}