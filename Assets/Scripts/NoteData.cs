using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NoteData : IComparable
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

    public int CompareTo(object obj)
    {
        NoteData other = obj as NoteData;

        if (other == null)
            throw new NotImplementedException();

        return this.timing.CompareTo(other.timing);
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