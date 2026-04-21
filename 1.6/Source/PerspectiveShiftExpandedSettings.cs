using System.Collections.Generic;
using Verse;
using static RimWorld.CompCableConnection;

namespace PerspectiveShiftExpanded
{
    public class PerspectiveShiftExpandedSettings : ModSettings
    {
        public TimeSpeed timeSpeedPawnAvatarBeforeWork = TimeSpeed.Normal;
        public bool enableJobsTimeSpeedUp = true;
        public float jobsTimeSpeedUpLevel = 3f;
        public bool enableMineTimeSpeedUp = true;
        public bool enablePlantTimeSpeedUp = true;
        public bool enableResearchTimeSpeedUp = true;
        public bool enableDoBillTimeSpeedUp = true;
        public bool enableDoFrameAndRepairTimeSpeedUp = true;
        public bool enableRestTimeSpeedUp = true;
        public bool enableReadBookTimeSpeedUp = true;
        public bool enableMeditatePrayTimeSpeedUp = true;
        public bool enableRepairMechTimeSpeedUp = true;
        public bool enableGroupActivitiesTimeSpeedUp = true;

        public float reloadTickPerMoveAdjustmentConstant = 5f;

        public bool disableNeeds = true;
        public bool disableNeedMood = false;
        public bool disableNeedFood = true;
        public bool disableNeedRest = true;
        public bool disableNeedJoy = false;
        public bool disableNeedOutdoor = false;
        public bool disableNeedIndoor = false;
        public bool disableNeedBeauty = false;
        public bool disableNeedComfort = false;
        public bool disableNeedDrugDesire = false;
        public bool disableNeedRoomSize = false;

        public bool disableMentalBreakStates = false;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref disableNeeds, "disableNeeds", true);
            Scribe_Values.Look(ref disableNeedMood, "disableNeedMood", false);
            Scribe_Values.Look(ref disableNeedFood, "disableNeedFood", true);
            Scribe_Values.Look(ref disableNeedRest, "disableNeedRest", true);
            Scribe_Values.Look(ref disableNeedJoy, "disableNeedJoy", false);
            Scribe_Values.Look(ref disableNeedOutdoor, "disableNeedOutdoor", false);
            Scribe_Values.Look(ref disableNeedIndoor, "disableNeedIndoor", false);
            Scribe_Values.Look(ref disableNeedBeauty, "disableNeedBeauty", false);
            Scribe_Values.Look(ref disableNeedComfort, "disableNeedComfort", false);
            Scribe_Values.Look(ref disableNeedDrugDesire, "disableNeedDrugDesire", false);
            Scribe_Values.Look(ref disableNeedRoomSize, "disableNeedRoomSize", false);

            Scribe_Values.Look(ref disableMentalBreakStates, "disableMentalBreakStates", false);

            Scribe_Values.Look(ref enableJobsTimeSpeedUp, "enableWorkSpeedUp", true);
            Scribe_Values.Look(ref jobsTimeSpeedUpLevel, "jobsTimeSpeedUpLevel", 3);
            Scribe_Values.Look(ref enableMineTimeSpeedUp, "enableMineTimeSpeedUp", true);
            Scribe_Values.Look(ref enablePlantTimeSpeedUp, "enablePlantTimeSpeedUp", true);
            Scribe_Values.Look(ref enableResearchTimeSpeedUp, "enableResearchTimeSpeedUp", true);
            Scribe_Values.Look(ref enableDoBillTimeSpeedUp, "enableDoBillTimeSpeedUp", true);
            Scribe_Values.Look(ref enableDoFrameAndRepairTimeSpeedUp, "enableDoFrameAndRepairTimeSpeedUp", true);
            Scribe_Values.Look(ref enableRestTimeSpeedUp, "enableRestTimeSpeedUp", true);
            Scribe_Values.Look(ref enableReadBookTimeSpeedUp, "enableReadBookTimeSpeedUp", true);
            Scribe_Values.Look(ref enableMeditatePrayTimeSpeedUp, "enableMeditatePrayTimeSpeedUp", true);
            Scribe_Values.Look(ref enableRepairMechTimeSpeedUp, "enableRepairMechTimeSpeedUp", true);
            Scribe_Values.Look(ref enableGroupActivitiesTimeSpeedUp, "enableGroupActivitiesTimeSpeedUp", true);

            Scribe_Values.Look(ref reloadTickPerMoveAdjustmentConstant, "reloadTickPerMoveAdjustmentConstant", 5f);
        }
    }
}
