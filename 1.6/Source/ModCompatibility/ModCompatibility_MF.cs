using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.XR;
using Verse;
using Verse.AI;

namespace PerspectiveShiftExpanded
{
    public static partial class ModCompatibility
    {
        // MultiFloors -> PSE_MF
        public static bool MultiFloors => ModsConfig.IsActive("telardo.MultiFloors");
        //类型
        public static readonly Type PSE_MF_Jobs_CrossLevelMoveJobUtilityType = AccessTools.TypeByName("MultiFloors.Jobs.CrossLevelMoveJobUtility");
        //方法
        public static readonly MethodInfo PSE_MF_Jobs_CrossLevelMoveJobUtility_OnOrderedToSwitchLevelMethod = AccessTools.Method(PSE_MF_Jobs_CrossLevelMoveJobUtilityType, "OnOrderedToSwitchLevel", new Type[] { typeof(Pawn), typeof(bool) });
    }
}