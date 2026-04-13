using HarmonyLib;
using UnityEngine;
using Verse;
using System;
using System.Collections.Generic;

namespace PerspectiveShiftExpanded
{
    [HotSwappable]
    public class PerspectiveShiftExpandedMod : Mod
    {
        public static PerspectiveShiftExpandedSettings settings;
        private Vector2 scrollPosition;
        public PerspectiveShiftExpandedMod(ModContentPack pack) : base(pack)
        {
            settings = GetSettings<PerspectiveShiftExpandedSettings>();
            new Harmony("PerspectiveShiftExpandedMod").PatchAll();
        }

        public override void DoSettingsWindowContents(Rect rect)
        {
            var listing = new Listing_Standard();
            var outRect = new Rect(rect.x, rect.y, rect.width, rect.height);
            var viewRect = new Rect(0f, 0f, rect.width - 30f, CalculateHeight());
            Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
            listing.Begin(viewRect);
            
            listing.CheckboxLabeled("PS_EnableJobsTimeSpeedUp".Translate(), ref settings.enableJobsTimeSpeedUp);
            if (settings.enableJobsTimeSpeedUp)
            {
                listing.Label("PS_JobsTimeSpeedUpLevel".Translate(settings.jobsTimeSpeedUpLevel.ToString("F0")));
                settings.jobsTimeSpeedUpLevel = Mathf.RoundToInt(listing.Slider(settings.jobsTimeSpeedUpLevel, 1f, 3f));

                listing.CheckboxLabeled("PS_EnableMineTimeSpeedUp".Translate(), ref settings.enableMineTimeSpeedUp);
                listing.CheckboxLabeled("PS_EnablePlantTimeSpeedUp".Translate(), ref settings.enablePlantTimeSpeedUp);
                listing.CheckboxLabeled("PS_EnableResearchTimeSpeedUp".Translate(), ref settings.enableResearchTimeSpeedUp);
                listing.CheckboxLabeled("PS_EnableDoBillTimeSpeedUp".Translate(), ref settings.enableDoBillTimeSpeedUp);
                listing.CheckboxLabeled("PS_EnableDoFrameAndRepairTimeSpeedUp".Translate(), ref settings.enableDoFrameAndRepairTimeSpeedUp);
                listing.CheckboxLabeled("PS_EnableRestTimeSpeedUp".Translate(), ref settings.enableRestTimeSpeedUp);
                listing.CheckboxLabeled("PS_EnableReadBookTimeSpeedUp".Translate(), ref settings.enableReadBookTimeSpeedUp);
                listing.CheckboxLabeled("PS_EnableMeditatePrayTimeSpeedUp".Translate(), ref settings.enableMeditatePrayTimeSpeedUp);
            }

            listing.End();
            Widgets.EndScrollView();
            base.DoSettingsWindowContents(rect);
        }

        private float CalculateHeight()
        {
            var labels = 0;
            var sliders = 0;
            var checkboxes = 1;
            var buttons = 0;
            if (settings.enableJobsTimeSpeedUp)
            {
                labels += 1;
                sliders += 1;
                checkboxes += 8;
            }
            return (labels * 24f) + (sliders * 24f) + (checkboxes * 24f) + (buttons * 32f);
        }

        public override string SettingsCategory()
        {
            return Content.Name;
        }
    }
}
