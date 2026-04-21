using RimWorld;
using System.Collections.Generic;
using Verse;

// 当化身(Avatar)开始特定工作时,根据工作定义设置时间加速状态
// ModCompatibility: PS

namespace PerspectiveShiftExpanded
{
    public static class JobDefsNeedTimeSpeedUp
    {
        private static List<JobDef> JobDefs = new List<JobDef>
        {
            JobDefOf.Mine,
            JobDefOf.SmoothFloor,
            JobDefOf.SmoothWall,

            JobDefOf.CutPlantDesignated,
            JobDefOf.HarvestDesignated,
            JobDefOf.CutPlant,
            JobDefOf.Harvest,
            JobDefOf.Sow,

            JobDefOf.Research,
            JobDefOf.InvestigateMonolith,   // 调查巨石
            JobDefOf.ApplyTechprint,        // 应用科技蓝图;
            JobDefOf.AnalyzeItem,           // 分析物品
            JobDefOf.Hack,
            JobDefOf.UseNeurotrainer,       // 使用神经训练器
            JobDefOf.HateChanting,          // 仇恨咏唱

            JobDefOf.DoBill,
            JobDefOf.OperateDeepDrill,      // 操作深钻井

            JobDefOf.FinishFrame,
            JobDefOf.Repair,
            JobDefOf.Deconstruct,

            JobDefOf.LayDown,

            DefsOf.PSE_AvatarReading,
            JobDefOf.Reading,

            JobDefOf.MeditatePray,
            JobDefOf.Meditate,

            JobDefOf.RepairMech,

            JobDefOf.GiveSpeech,
            JobDefOf.Dance,
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

            if ((jobDef == JobDefOf.Mine ||
                jobDef == JobDefOf.SmoothFloor ||
                jobDef == JobDefOf.SmoothWall)
                && !settings.enableMineTimeSpeedUp)
            { return; }
            if ((jobDef == JobDefOf.CutPlantDesignated ||
                jobDef == JobDefOf.HarvestDesignated ||
                jobDef == JobDefOf.CutPlant ||
                jobDef == JobDefOf.Harvest ||
                jobDef == JobDefOf.Sow)
                && !settings.enablePlantTimeSpeedUp)
            { return; }
            if ((jobDef == JobDefOf.Research ||
                jobDef == JobDefOf.InvestigateMonolith ||
                jobDef == JobDefOf.ApplyTechprint ||
                jobDef == JobDefOf.AnalyzeItem ||
                jobDef == JobDefOf.Hack ||
                jobDef == JobDefOf.UseNeurotrainer ||
                jobDef == JobDefOf.HateChanting)
                && !settings.enableResearchTimeSpeedUp)
            { return; }
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
            if (jobDef == JobDefOf.RepairMech && !settings.enableRepairMechTimeSpeedUp) { return; }
            if ((jobDef == JobDefOf.GiveSpeech ||
                jobDef == JobDefOf.Dance)
                && !settings.enableGroupActivitiesTimeSpeedUp)
            { return; }
            if ((jobDef == JobDefOf.Reading ||
                jobDef == DefsOf.PSE_AvatarReading)
                && !settings.enableReadBookTimeSpeedUp)
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
