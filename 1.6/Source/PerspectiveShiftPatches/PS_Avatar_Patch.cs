using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using Verse;
using Verse.AI;


namespace PerspectiveShiftExpanded
{
    [StaticConstructorOnStartup]
    public static class PS_Avatar_OnGUI_HarmonyManualPatches
    {
        static PS_Avatar_OnGUI_HarmonyManualPatches()
        {
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
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return PS_Avatar_ProcessMovement_Transpiler.DoTranspile(instructions);
        }
    }

    public static class PS_Avatar_ProcessMovement_Transpiler
    {
        /// <summary>
        /// 不会被移动中断的Job列表
        /// </summary>
        private static List<JobDef> PreventInterruptJobs = new List<JobDef>
        {
            //DefsOf.PSE_AvatarReading,
            ModCompatibility.PSE_CE_GET_CE_JobDefOf_ReloadWeapon()

        };
        private static void Injected_SelectivePreventInterruptJobs(object avatarInstance, Job currentJob, Pawn pawn)
        {
            if (avatarInstance == null || currentJob== null) return;

            if (PreventInterruptJobs.Contains(currentJob.def))
            {
                // 化身正在执行某些特殊Job时中断原版操作
                return;
            }
            ModCompatibility.PSE_PS_Avatar_HandleAbilityCancellationMethod.Invoke(avatarInstance, new object[] { currentJob });
            pawn?.jobs?.EndCurrentJob(JobCondition.InterruptForced);
        }

        /// <summary>
        /// 注入代码, 增加Avatar在移动时的中断Job条件, 以实现移动中不打断某些特殊Job(如换弹\读书)的效果
        /// </summary>
        public static IEnumerable<CodeInstruction> DoTranspile(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            var targetMethod = ModCompatibility.PSE_PS_Avatar_HandleAbilityCancellationMethod;
            var replacement = AccessTools.Method(typeof(PS_Avatar_ProcessMovement_Transpiler), nameof(Injected_SelectivePreventInterruptJobs));
            var endJobMethod = AccessTools.Method(typeof(Pawn_JobTracker), nameof(Pawn_JobTracker.EndCurrentJob));

            if (targetMethod == null) return instructions;

            bool found = false;
            for (int i = 0; i < codes.Count; i++)
            {
                // 找到 HandleAbilityCancellation 的位置
                if ((codes[i].opcode == OpCodes.Callvirt || codes[i].opcode == OpCodes.Call) &&
                    codes[i].operand is MethodInfo mi && mi == targetMethod)
                {
                    // --- 修改 3: 调整栈数据，压入 pawn 参数 ---
                    // 在调用补丁前，我们需要在栈上准备好 [Avatar, Job, Pawn]
                    // 原本栈上已有 [Avatar, Job]，我们只需要在最后塞入一个 Pawn
                    codes.Insert(i, new CodeInstruction(OpCodes.Ldarg_0));
                    codes.Insert(i + 1, new CodeInstruction(OpCodes.Ldfld, ModCompatibility.PSE_PS_State_AvatarInstanceField));

                    // --- 修改 4: 替换原始调用 ---
                    // 注意：由于 Insert 了两条指令，原本的 i 现在变成了 i+2
                    codes[i + 2].opcode = OpCodes.Call;
                    codes[i + 2].operand = replacement;

                    // --- 修改 5: 大面积抹除 (Nop) ---
                    // 从补丁调用之后，一直向后搜索，直到把 EndCurrentJob 也抹除掉
                    for (int j = i + 3; j < codes.Count; j++)
                    {
                        var op = codes[j].opcode;
                        var operand = codes[j].operand;

                        // 记录当前指令，准备抹除
                        codes[j].opcode = OpCodes.Nop;
                        codes[j].operand = null;

                        // 如果碰到了 EndCurrentJob 的调用，说明清理到头了
                        if ((op == OpCodes.Callvirt || op == OpCodes.Call) && operand is MethodInfo m && m == endJobMethod)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (found) break;
                }
            }
            if (!found)
            {
                Log.Message("[PerspectiveShiftExpanded] PS_Avatar_ProcessMovement_Transpiler_DoTranspile 未找到挂载点");
            }
            return codes;
        }
    }
}