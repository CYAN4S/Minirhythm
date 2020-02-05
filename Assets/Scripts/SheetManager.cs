using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheetManager : MonoBehaviour
{
    public string songName, displayBpm;
    public Tuple<string, string> artistList;

    public int modeLine;
    public List<NoteData> noteList;
    public List<BpmData> bpmList;
    //public double cruelty;

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

    public static SheetManager GetSheetManager(SerializableSheet h)
    {
        return null;
    }

    public static SerializableSheet GetSerializableSheet(SheetManager m)
    {
        return null;
    }
}

