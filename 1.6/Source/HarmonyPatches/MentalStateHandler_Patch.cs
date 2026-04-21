using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

// 禁用化身(Avatar)心理崩溃(MentalBreak)的情况
// ModCompatibility: PS || CE

namespace PerspectiveShiftExpanded
{
    [HarmonyPatch(typeof(MentalStateHandler), nameof(MentalStateHandler.TryStartMentalState))]
    public static class MentalStateHandler_TryStartMentalState_Patch
    {
        private static readonly HashSet<string> BlockedMentalStates = new HashSet<string>
        {
            // --- 原版 ---
            "Berserk",             // 狂暴
            "BerserkPermanent",    // 永久狂暴
            "BerserkMechanoid",    // 机械族狂暴
            "CocoonDisturbed",     // 茧受扰
            "BerserkWarcall",      // 战争呼唤狂暴 (Biotech)
            "HumanityBreak",       // 人性崩溃 (Anomaly)
            "EntityKiller",        // 实体杀手 (Anomaly)
            "Wander_Psychotic",    // 精神错乱游荡
            "Wander_Sad",          // 悲伤游荡
            "Wander_OwnRoom",      // 躲在房间
            "PanicFlee",           // 惊恐逃跑
            "Manhunter",           // 猎杀人类
            "ManhunterPermanent",  // 永久猎杀人类
            "SocialFighting",      // 社会斗殴
            "Roaming",             // 漫游（离家出走）
            "Rebellion",           // 反抗 (Ideology)
            "ManhunterBloodRain",  // 血雨猎杀 (Anomaly)
            "CubeSculpting",       // 立方体雕刻 (Anomaly)
            "Terror",              // 恐惧 (Odyssey)
            "PanicFleeFire",       // 因火惊恐逃跑 (Biotech)
            "FireStartingSpree",   // 纵火狂欢
            "Binging_DrugExtreme", // 药物狂吃（极重）
            "Jailbreaker",         // 越狱煽动
            "Slaughterer",         // 屠杀动物
            "MurderousRage",       // 谋杀狂暴
            "Binging_DrugMajor",   // 药物狂吃（重度）
            "Tantrum",             // 发脾气
            "TargetedTantrum",     // 针对性发脾气
            "BedroomTantrum",      // 卧室发脾气
            "SadisticRage",        // 施虐狂暴
            "CorpseObsession",     // 尸体痴迷
            "Binging_Food",        // 食物狂吃
            "InsultingSpree",      // 侮辱狂欢
            "TargetedInsultingSpree", // 针对性侮辱狂欢

            // --- CE (Combat Extended) ---
            "CombatFrenzy",        // 战斗狂热
            "ShellShock",          // 战斗休克
            "WanderConfused",      // 困惑游荡
        };

        public static bool Prefix(MentalStateHandler __instance, MentalStateDef stateDef, ref bool __result)
        {
            if (!ModCompatibility.PerspectiveShift) return true;
            if (!PerspectiveShiftExpandedMod.settings.disableMentalBreakStates) { return true; }
            if (stateDef == null) return true;

            Pawn pawn = __instance.pawn;
            if (pawn == null || !pawn.PSE_PS_State_IsAvatar()) { return true; }

            if (BlockedMentalStates.Contains(stateDef.defName))
            {
                __result = false;
                return false;       // 拦截执行
            }
            return true;
        }
    }
}
