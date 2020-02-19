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
    public int noteCount;
    //public double cruelty;

    //public List<????> TimeMetadata;

    private void Awake()
    {
        /// USE FOR TESTING
        modeLine = 4;
        System.Random random = new System.Random();

        noteList = new List<NoteData>();
        for (int i = 0; i < 10; i++)
        {
            noteList.Add(new NoteData(i / 2.0, random.Next(0, 4), i % 2, 0));
            noteList.Add(new NoteData(i / 2.0, -1, 2, 0));
        }

        //noteList.Add(new LongNoteData(0, 0, 0, 0, 2));

        bpmList = new List<BpmData>();
        bpmList.Add(new BpmData(0, 120));

        noteList.Sort();
        noteCount = 10;
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

