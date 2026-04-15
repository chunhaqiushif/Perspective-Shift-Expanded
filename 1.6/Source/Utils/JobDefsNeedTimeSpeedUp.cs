using RimWorld;
using System.Collections.Generic;
using Verse;

namespace PerspectiveShiftExpanded
{
    public static class JobDefsNeedTimeSpeedUp
    {
        private static List<JobDef> JobDefs = new List<JobDef>
        {
            JobDefOf.Mine,

            JobDefOf.CutPlantDesignated,
            JobDefOf.HarvestDesignated,
            JobDefOf.CutPlant,
            JobDefOf.Harvest,
            JobDefOf.Sow,

            JobDefOf.Research,

            JobDefOf.DoBill,
            JobDefOf.OperateDeepDrill,

            JobDefOf.FinishFrame,
            JobDefOf.Repair,
            JobDefOf.Deconstruct,

            JobDefOf.LayDown,
            
            DefsOf.PSE_AvatarReading,

            JobDefOf.MeditatePray,
            JobDefOf.Meditate,
        };

        public static bool isNeedTimeSpeedUp(JobDef jobDef)
        {
            return JobDefs.Contains(jobDef);
        }

        public static void SetTimeSpeedByJobDef(JobDef jobDef)
        {
            var settings = PerspectiveShiftExpandedMod.settings;
            if (!settings.enableJobsTimeSpeedUp) { return; }
            if (!isNeedTimeSpeedUp(jobDef)) { return; }
            if ((float)Find.TickManager.curTimeSpeed >= settings.jobsTimeSpeedUpLevel) { return; }

            if (jobDef == JobDefOf.Mine && !settings.enableMineTimeSpeedUp) { return; }
            if ((jobDef == JobDefOf.CutPlantDesignated ||
                jobDef == JobDefOf.HarvestDesignated ||
                jobDef == JobDefOf.CutPlant ||
                jobDef == JobDefOf.Harvest ||
                jobDef == JobDefOf.Sow) 
                && !settings.enablePlantTimeSpeedUp)
            { return; }
            if (jobDef == JobDefOf.Research && !settings.enableResearchTimeSpeedUp) { return; }
            if ((jobDef == JobDefOf.DoBill ||
                jobDef == JobDefOf.OperateDeepDrill) 
                && !settings.enableDoBillTimeSpeedUp) 
            { return; }
            if ((jobDef == JobDefOf.FinishFrame ||
                jobDef == JobDefOf.Repair ||
                jobDef == JobDefOf.Deconstruct) 
                && !settings.enableDoFrameAndRepairTimeSpeedUp) 
            { return; }
            if (jobDef == JobDefOf.LayDown && !settings.enableRestTimeSpeedUp) { return; }
            if ((jobDef == JobDefOf.MeditatePray ||
                jobDef == JobDefOf.Meditate) 
                && !settings.enableMeditatePrayTimeSpeedUp)
            { return; }

            settings.timeSpeedPawnAvatarBeforeWork = Find.TickManager.curTimeSpeed;
            Find.TickManager.curTimeSpeed = (TimeSpeed)settings.jobsTimeSpeedUpLevel;
        }

        public static void ResetTimeSpeed(JobDef jobDef)
        {
            var settings = PerspectiveShiftExpandedMod.settings;
            if (!settings.enableJobsTimeSpeedUp) { return; }
            if (!isNeedTimeSpeedUp(jobDef)) { return; }
            if ((float)Find.TickManager.curTimeSpeed != settings.jobsTimeSpeedUpLevel) { return; }
            Find.TickManager.CurTimeSpeed = settings.timeSpeedPawnAvatarBeforeWork;
        }
    }
}
