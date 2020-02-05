using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimeCalc
{

    public static void GetTimeMetadata(List<BpmData> bpmData)
    {

    }


    #region LEGACY
    // SheetManager Parameter should be fixed to TimeMetadata for performance.
    public static double GetTiming(float time, SheetManager sheetManager)
    {
        return time * sheetManager.bpmList[0].bpm * (1.0 / 60.0);
    }

    public static float GetTime(double timing, SheetManager sheetManager)
    {
        return (float)(timing * (1.0 / sheetManager.bpmList[0].bpm)) * 60.0f;
    }
    #endregion
}