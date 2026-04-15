using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PerspectiveShiftExpanded
{
    //[StaticConstructorOnStartup]
    //public static class CE_Verb_ShootCE_Available_HarmonyManualPatches
    //{ 
    //    static CE_Verb_ShootCE_Available_HarmonyManualPatches()
    //    {
    //        if (ModCompatibility.PSE_CE_VerbShootCE_AvailableMethod == null) { return; }

    //        MethodInfo myTranspiler = AccessTools.Method(
    //            typeof(CE_Verb_ShootCE_Available_Patch), 
    //            nameof(CE_Verb_ShootCE_Available_Patch.Transpiler)
    //            );

    //        Startup.harmony.Patch(
    //            ModCompatibility.PSE_CE_VerbShootCE_AvailableMethod, 
    //            transpiler: new HarmonyMethod(myTranspiler)
    //            );

    //        Log.Message("[PerspectiveShiftExpanded] 已成功挂载 CombatExtended.Verb_ShootCE.Available 的转译补丁");
    //    }
    //}

    //[StaticConstructorOnStartup]
    //public static class CE_Verb_ShootCE_OnCastSuccessful_HarmonyManualPatches
    //{
    //    static CE_Verb_ShootCE_OnCastSuccessful_HarmonyManualPatches()
    //    {
    //        if (ModCompatibility.PSE_CE_VerbShootCE_OnCastSuccessfulMethod == null) { return; }

    //        MethodInfo myTranspiler = AccessTools.Method(
    //            typeof(CE_Verb_ShootCE_OnCastSuccessful_Patch),
    //            nameof(CE_Verb_ShootCE_OnCastSuccessful_Patch.Transpiler)
    //            );

    //        Startup.harmony.Patch(
    //            ModCompatibility.PSE_CE_VerbShootCE_OnCastSuccessfulMethod,
    //            transpiler: new HarmonyMethod(myTranspiler)
    //            );

    //        Log.Message("[PerspectiveShiftExpanded] 已成功挂载 CombatExtended.Verb_ShootCE.OnCastSuccessful 的转译补丁");
    //    }
    //}


    public static class CE_Verb_ShootCE_Available_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // 调用通用的替换逻辑
            return CE_Verb_ShootCE_Transpiler.DoTranspile(instructions);
        }   
    }

    public static class CE_Verb_ShootCE_OnCastSuccessful_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // 调用通用的替换逻辑
            return CE_Verb_ShootCE_Transpiler.DoTranspile(instructions);
        }
    }

    public static class CE_Verb_ShootCE_Transpiler
    {
        // 注入逻辑：这是被 IL 调用的“分流器”
        private static void Injected_SelectiveReload(object compInstance)
        {
            if (compInstance == null) return;

            Pawn wielder = ModCompatibility.PSE_CE_GET_CompAmmoUser_Wielder(compInstance);

            if (wielder != null && wielder.PSE_PS_State_IsAvatar())
            {
                // 为化身则吞掉原版调用
                return;
            }

            // 执行原版逻辑
            ModCompatibility.PSE_CE_CompAmmoUser_TryStartReload(compInstance);
        }

        public static IEnumerable<CodeInstruction> DoTranspile(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            var target = ModCompatibility.PSE_CE_CompAmmoUser_TryStartReloadMethod;
            var replacement = AccessTools.Method(typeof(CE_Verb_ShootCE_Transpiler), nameof(Injected_SelectiveReload));

            if (target == null) return instructions;

            bool found = false;
            for (int i = 0; i < codes.Count; i++)
            {
                // 查找所有对 CE 原版 TryStartReload 的调用
                if ((codes[i].opcode == OpCodes.Callvirt || codes[i].opcode == OpCodes.Call) && codes[i].operand is MethodInfo mi && mi == target)
                {
                    // 改为静态调用我们的拦截器
                    codes[i].opcode = OpCodes.Call;
                    codes[i].operand = replacement;
                    found = true;
                }
            }
            if (!found)
            {
                Log.Message("[PerspectiveShiftExpanded] CE_Verb_ShootCE_Transpiler_DoTranspile 未找到挂载点");
            }
            return codes;
        }
    }
}