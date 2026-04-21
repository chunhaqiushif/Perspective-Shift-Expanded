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

            listing.Label("PSE_ReloadTickPerMoveAdjustmentConstant".Translate(settings.reloadTickPerMoveAdjustmentConstant.ToString("F0")));
            settings.reloadTickPerMoveAdjustmentConstant = Mathf.RoundToInt(listing.Slider(settings.reloadTickPerMoveAdjustmentConstant, 1f, 10f));

            listing.GapLine();

            listing.CheckboxLabeled("PSE_DisableMentalBreakStates".Translate(), ref settings.disableMentalBreakStates);

            listing.CheckboxLabeled("PSE_DisableNeeds".Translate(), ref settings.disableNeeds);
            if (settings.disableNeeds)
            {
                listing.CheckboxLabeled("PSE_DisableNeedMood".Translate(), ref settings.disableNeedMood);
                listing.CheckboxLabeled("PSE_DisableNeedFood".Translate(), ref settings.disableNeedFood);
                listing.CheckboxLabeled("PSE_DisableNeedRest".Translate(), ref settings.disableNeedRest);
                listing.CheckboxLabeled("PSE_DisableNeedJoy".Translate(), ref settings.disableNeedJoy);
                listing.CheckboxLabeled("PSE_DisableNeedOutdoor".Translate(), ref settings.disableNeedOutdoor);
                listing.CheckboxLabeled("PSE_DisableNeedIndoor".Translate(), ref settings.disableNeedIndoor);
                listing.CheckboxLabeled("PSE_DisableNeedBeauty".Translate(), ref settings.disableNeedBeauty);
                listing.CheckboxLabeled("PSE_DisableNeedComfort".Translate(), ref settings.disableNeedComfort);
                listing.CheckboxLabeled("PSE_DisableNeedDrugDesire".Translate(), ref settings.disableNeedDrugDesire);
                listing.CheckboxLabeled("PSE_DisableNeedRoomSize".Translate(), ref settings.disableNeedRoomSize);
            }

            listing.GapLine();

            listing.CheckboxLabeled("PSE_EnableJobsTimeSpeedUp".Translate(), ref settings.enableJobsTimeSpeedUp);
            if (settings.enableJobsTimeSpeedUp)
            {
                listing.Label("PSE_JobsTimeSpeedUpLevel".Translate(settings.jobsTimeSpeedUpLevel.ToString("F0")));
                settings.jobsTimeSpeedUpLevel = Mathf.RoundToInt(listing.Slider(settings.jobsTimeSpeedUpLevel, 1f, 3f));

                listing.CheckboxLabeled("PSE_EnableMineTimeSpeedUp".Translate(), ref settings.enableMineTimeSpeedUp);
                listing.CheckboxLabeled("PSE_EnablePlantTimeSpeedUp".Translate(), ref settings.enablePlantTimeSpeedUp);
                listing.CheckboxLabeled("PSE_EnableResearchTimeSpeedUp".Translate(), ref settings.enableResearchTimeSpeedUp);
                listing.CheckboxLabeled("PSE_EnableDoBillTimeSpeedUp".Translate(), ref settings.enableDoBillTimeSpeedUp);
                listing.CheckboxLabeled("PSE_EnableDoFrameAndRepairTimeSpeedUp".Translate(), ref settings.enableDoFrameAndRepairTimeSpeedUp);
                listing.CheckboxLabeled("PSE_EnableRestTimeSpeedUp".Translate(), ref settings.enableRestTimeSpeedUp);
                listing.CheckboxLabeled("PSE_EnableReadBookTimeSpeedUp".Translate(), ref settings.enableReadBookTimeSpeedUp);
                listing.CheckboxLabeled("PSE_EnableMeditatePrayTimeSpeedUp".Translate(), ref settings.enableMeditatePrayTimeSpeedUp);
                listing.CheckboxLabeled("PSE_EnableRepairMechTimeSpeedUp".Translate(), ref settings.enableRepairMechTimeSpeedUp);
                listing.CheckboxLabeled("PSE_EnableGroupActivitiesTimeSpeedUp".Translate(), ref settings.enableGroupActivitiesTimeSpeedUp);
            }
            listing.End();
            Widgets.EndScrollView();
            base.DoSettingsWindowContents(rect);
        }

        private float CalculateHeight()
        {
            var labels = 1;
            var sliders = 1;
            var checkboxes = 3;
            var buttons = 0;
            var gap_line = 2;
            if (settings.enableJobsTimeSpeedUp)
            {
                labels += 1;
                sliders += 1;
                checkboxes += 10;
            }
            if(settings.disableNeeds)
            {
                checkboxes += 10;
            }
            return (labels * 24f) + (sliders * 24f) + (checkboxes * 24f) + (buttons * 32f) + (gap_line * 12f);
        }

        public override string SettingsCategory()
        {
            return Content.Name;
        }

        public override void WriteSettings()
        {
            base.WriteSettings();
            AvatarUtils.RefreshAvatarNeeds();
        }
    }
}
