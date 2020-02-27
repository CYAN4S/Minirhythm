using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Extension of Serialized file of this class object is ".mrs".
[Serializable]
public class SerializableSheet
{
    public int modeLine;
    public double difficulty;
    public int difficultyType;

    // Serialization does not support polymorphism.
    public List<NoteData> regNoteList;
    public List<LongNoteData> longNoteList;

    // Override when count is more than 0.
    public List<BpmData> bpmList;

    public bool IsValid()
    {
        if (modeLine != 4 && modeLine != 5 && modeLine != 6 && modeLine != 8) { return false; }
        if (0 >= difficulty || difficulty > 15) { return false; }
        if (0 > difficultyType || difficultyType > 3) { return false; }
        return true;
    }
}

// Extension of Serialized file of this class object is ".mri".
[Serializable]
public class SerializableInfo
{
    public string songName, displayBpm;

    // Count of each list should be equal.
    public List<string> artistType;
    public List<string> artistName;

    public List<BpmData> bpmList;

    public List<string> audios;

    public bool IsValid()
    {
        if (bpmList.Count == 0 || audios.Count == 0) { return false; }
        return true;
    }

}