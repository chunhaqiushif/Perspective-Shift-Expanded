using System.Collections.Generic;
using Verse;

namespace PerspectiveShiftExpanded
{
    public class PerspectiveShiftExpandedSettings : ModSettings
    {
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
        public TimeSpeed timeSpeedPawnAvatarBeforeWork = TimeSpeed.Normal;

        public override void ExposeData()
        {
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
        }
    }
}
