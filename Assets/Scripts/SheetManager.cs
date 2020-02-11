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
        for (int i = 0; i < 1000; i++)
        {
            //noteList.Add(new NoteData(i / 4.0, i % 4, i % 3, 0));
        }

        noteList.Add(new LongNoteData(0, 0, 0, 0, 2));



        bpmList = new List<BpmData>();
        bpmList.Add(new BpmData(0, 120));
        ///
    }

    public static SheetManager GetSheetManager(SerializableSheet h, SerializableInfo i)
    {
        return null;
    }

    public static SerializableSheet GetSerializableSheet(SheetManager m)
    {
        return null;
    }
}

