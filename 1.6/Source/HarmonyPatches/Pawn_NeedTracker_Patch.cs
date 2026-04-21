using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

// 禁用化身(Avatar)的特定需求
// ModCompatibility: PS

namespace PerspectiveShiftExpanded
{
    [HarmonyPatch(typeof(Pawn_NeedsTracker), nameof(Pawn_NeedsTracker.ShouldHaveNeed))]
    public static class Pawn_NeedTracker_ShouldHaveNeed_Patch
    {
        public static void Postfix(Pawn_NeedsTracker __instance, NeedDef nd, ref bool __result)
        {
            if (!__result) return;
            if (!ModCompatibility.PerspectiveShift) { return; }

            if (__instance.pawn == PS_State_SetAvatar_Patch.lastPawn ||
                __instance.pawn == PS_State_ClearAvatar_Patch.pawnToRestore)
            { return; }

            bool isAvatar = ModCompatibility.PSE_PS_State_IsAvatar(__instance.pawn)
                        || __instance.pawn == PS_State_SetAvatar_Patch.forcingAvatarPawn;

            if (isAvatar && PerspectiveShiftExpandedMod.settings.disableNeeds)
            {
                if (nd != null && NeedToggles.TryGetValue(nd.defName, out var shouldDisable))
                {
                    if (shouldDisable())
                    {
                        __result = false;
                    }
                }
            }
        }

        private static readonly Dictionary<string, Func<bool>> NeedToggles = new Dictionary<string, Func<bool>>
        {
            { "Mood", () => PerspectiveShiftExpandedMod.settings.disableNeedMood },
            { "Food", () => PerspectiveShiftExpandedMod.settings.disableNeedFood },
            { "Rest", () => PerspectiveShiftExpandedMod.settings.disableNeedRest },
            { "Joy", () => PerspectiveShiftExpandedMod.settings.disableNeedJoy },
            { "Outdoors", () => PerspectiveShiftExpandedMod.settings.disableNeedOutdoor },
            { "Indoors", () => PerspectiveShiftExpandedMod.settings.disableNeedIndoor },
            { "Beauty", () => PerspectiveShiftExpandedMod.settings.disableNeedBeauty },
            { "Comfort", () => PerspectiveShiftExpandedMod.settings.disableNeedComfort },
            { "RoomSize", () => PerspectiveShiftExpandedMod.settings.disableNeedRoomSize },
            { "DrugDesire", () => PerspectiveShiftExpandedMod.settings.disableNeedDrugDesire },
        };
    }
}
