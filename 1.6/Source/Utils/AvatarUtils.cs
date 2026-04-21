using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace PerspectiveShiftExpanded
{
    public class AvatarUtils
    {
        public static bool isAllowAvatarToReload = false;
        public static bool avatarDraftedStateBefore = false;

        private static readonly int avatarNotifyCoolDownTicks = 30;
        private static int nextavatarNotifyTick = -1;


        public static void AvatarNotify(string speechText, SoundDef sound)
        {
            if (!ModCompatibility.PerspectiveShift) { return; }

            int currentTick = Find.TickManager.TicksGame;
            if (currentTick < nextavatarNotifyTick) { return; }

            Pawn pawn = ModCompatibility.PSE_PS_GET_State_Avatar_Pawn();
            if (pawn == null || speechText == null) { return; }

            MoteMaker.ThrowText(pawn.DrawPos, pawn.Map, speechText);
            sound.PlayOneShotOnCamera();

            nextavatarNotifyTick = currentTick + avatarNotifyCoolDownTicks;
        }

        // 用于在设置更改时手动调用的工具类
        public static void RefreshAvatarNeeds()
        {
            Log.Message("A");
            if (Current.ProgramState != ProgramState.Playing) return;
            Log.Message("B");
            Pawn pawn = ModCompatibility.PSE_PS_GET_State_Avatar_Pawn();
            if (pawn != null && pawn.needs != null)
            {
                Log.Message("C");
                pawn.needs.AddOrRemoveNeedsAsAppropriate();
            }
        }
    }
}
