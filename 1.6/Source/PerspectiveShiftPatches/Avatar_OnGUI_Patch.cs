using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using Verse;
using Verse.AI;


namespace PerspectiveShiftExpanded
{
    [StaticConstructorOnStartup]
    public static class HarmonyManualPatches
    {
        static HarmonyManualPatches()
        {
            var harmony = new Harmony("Tupler.PerspectiveShiftExpanded");

            // 1. 获取目标类型
            Type avatarType = AccessTools.TypeByName("PerspectiveShift.Avatar");
            if (avatarType == null) return; // 没装 Mod 则跳过

            // 2. 获取目标 OnGUI 方法
            // 如果 OnGUI 没有参数，传空数组或 null 即可
            MethodInfo originalOnGUI = AccessTools.Method(avatarType, "OnGUI");
            if (originalOnGUI == null) return;

            // 3. 获取你自己的补丁方法
            MethodInfo myPostfix = AccessTools.Method(typeof(Avatar_OnGUI_Patch), nameof(Avatar_OnGUI_Patch.Postfix));

            // 4. 执行手动挂载
            harmony.Patch(originalOnGUI, postfix: new HarmonyMethod(myPostfix));

            Log.Message("[PerspectiveShiftExpanded] 已成功挂载 Avatar.OnGUI 的后置补丁");
        }
    }

    public static class Avatar_OnGUI_Patch
    {

        public static void Postfix(object __instance)
        {
            HandleReadingBinding();
            HandleSelectAvatarBindings();
        }

        private static void HandleReadingBinding()
        {
            if (!DefsOf.PSE_ReadBook.KeyDownEvent)
            {
                return;
            }
            if (Find.TickManager.CurTimeSpeed == TimeSpeed.Paused)
            {
                return;
            }
            Pawn pawn = GetFromPerspectiveShift.GetAvatarPawn();
            if (pawn?.inventory == null)
            {
                return;
            }
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
            bool try_readingJob = pawn.jobs.TryTakeOrderedJob(readingJob, JobTag.Idle);
        }

        private static void HandleSelectAvatarBindings()
        {
            if (!DefsOf.PSE_SelectAvatar.KeyDownEvent)
            {
                return;
            }
            Pawn pawn = GetFromPerspectiveShift.GetAvatarPawn();
            if (pawn != null && pawn.Spawned)
            {
                Find.Selector.ClearSelection();
                Find.Selector.Select(pawn);
                CameraJumper.TryJump(pawn);
            }
        }
    }
}
