using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace PerspectiveShiftExpanded
{
    [HarmonyPatch(typeof(Pawn), nameof(Pawn.TicksPerMove))]
    public static class Pawn_TicksPerMove_Patch
    {

        public static bool Prefix(Pawn __instance, ref float __result, bool diagonal)
        {
            if (!ModCompatibility.CombatExpanded) { return true; }
            
            if (!__instance.PSE_PS_State_IsAvatar()) { return true; }

            if (ModCompatibility.PSE_CE_CE_JobDefOfType == null) { return true; }

            if (ModCompatibility.PSE_CE_GET_CE_JobDefOf_ReloadWeapon() == null) { return true; }
            if (__instance.CurJobDef != ModCompatibility.PSE_CE_GET_CE_JobDefOf_ReloadWeapon()) { return true; }
            float num = ((__instance.Downed && __instance.health.CanCrawl) ? __instance.GetStatValue(StatDefOf.CrawlSpeed) : __instance.GetStatValue(StatDefOf.MoveSpeed));
            float valueOrDefault = (__instance.CurJob?.targetB.Thing?.GetStatValue(StatDefOf.Mass)).GetValueOrDefault();
            float num2 = Math.Max(0.0001f, MassUtility.Capacity(__instance));
            float num3 = Mathf.Clamp(valueOrDefault / (num2 / 2f) * 100f, 0f, 100f);
            num *= (100f - num3) / 100f;
            if (RestraintsUtility.InRestraints(__instance))
            {
                num *= 0.35f;
            }
            Pawn_CarryTracker carryTracker = __instance.carryTracker;
            if (carryTracker != null && carryTracker.CarriedThing?.def?.category == ThingCategory.Pawn)
            {
                num *= 0.6f;
            }
            float num4 = num / 60f;
            if (num4 == 0f)
            {
                __result = 450f;
                return false;
            }
            float num5 = 1f / num4;
            if (__instance.Spawned && !__instance.Map.roofGrid.Roofed(__instance.Position))
            {
                num5 /= __instance.Map.weatherManager.CurMoveSpeedMultiplier;
            }
            if (diagonal)
            {
                num5 *= 1.41421f;
            }
            __result = Mathf.Clamp(num5, 1f, 450f);

            return false;
        }
    }
}
