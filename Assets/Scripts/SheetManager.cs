using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
    채보 데이터 관리 전용 클래스로 다음 역할을 수행합니다.
    1. 채보 파일을 읽어 리스트로 저장합니다.

    주의사항
    1. 노트 배치 기능을 이 곳에 구현하지 마시오.
        노트 배치 기능은 GameManager에서 구현해야 합니다.
*/


public class SheetManager : MonoBehaviour
{
    public string songName, displayBpm;
    public Tuple<string, string> artistList;

    public int modeLine;
    public List<NoteData> noteList;
    public List<BpmData> bpmList;
    public double cruelty;

    //public List<????> TimeMetadata;

    private void Awake()
    {
        /// USE FOR TESTING
        modeLine = 4;

        noteList = new List<NoteData>();
        for (int i = 0; i < 20; i++)
        {
            noteList.Add(new NoteData(i / 2.0, i % 4, i % 3, 0));
        }
        noteList.Add(new LongNoteData(11.0, 0, 0, 0, 1));

        bpmList = new List<BpmData>();
        bpmList.Add(new BpmData(0, 120));
        ///
    }

    public static SheetManager GetSheetManager(SerializableSheet h, SerializableSong s)
    {
        return null;
    }

    public static SerializableSheet GetSerializableSheet(SheetManager m)
    {
        return null;
    }

    public static SerializableSong GetSerializableSong(SheetManager m)
    {
        return null;
    }
}

[Serializable]
public class NoteData
{
    public double timing;
    public int line;
    public int audioCode;
    public byte playType;

    public NoteData(double timing, int line, int audioCode, byte playType)
    {
        this.timing = timing;
        this.line = line;
        this.audioCode = audioCode;
        this.playType = playType;
    }
}

[Serializable]
public class LongNoteData : NoteData
{
    public double lengthTiming;

    public LongNoteData(double timing, int line, int audioCode, byte playType, double lengthTiming) : base(timing, line, audioCode, playType)
    {
        this.lengthTiming = lengthTiming;
    }
}

[Serializable]
public class BpmData
{
    public double timing, bpm;

    public BpmData(double timing, double bpm)
    {
        this.timing = timing;
        this.bpm = bpm;
    }
}

// Extension of Serialized file of this class object is ".mrs".
public class SerializableSheet
{
    public int modeLine;
    public List<NoteData> noteList;
    public List<BpmData> bpmList;
    public double cruelty;
}

public class SerializableSong
{
    public string songName, displayBpm;
    public Tuple<string, string> artistList;
    public List<AudioClip> audios;
}

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