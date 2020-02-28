using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SheetManager : MonoBehaviour
{
    public string songName, displayBpm;

    public int modeLine;
    public List<NoteData> noteList;
    public List<BpmData> bpmList;
    public int noteCount;

    //public double cruelty;
    //public List<????> TimeMetadata;

    private void Awake()
    {
        // // USE FOR TESTING
        // modeLine = 4;
        // System.Random random = new System.Random();

        // noteList = new List<NoteData>();
        // for (int i = 0; i < 20; i++)
        // {
        //     noteList.Add(new NoteData(i / 4.0, random.Next(0, 4), i % 2, 0));
        //     noteList.Add(new NoteData(i / 4.0, -1, 2, 0));
        // }

        // //noteList.Add(new LongNoteData(0, 0, 0, 0, 2));

        // bpmList = new List<BpmData>();
        // bpmList.Add(new BpmData(0, 120));

        // noteList.Sort();

        // var keynote = from note in noteList where note.line != -1 select note;
        // noteCount = keynote.Count();
        // //
    }

    public void GetSheetManager(SerializableSheet h, SerializableInfo i)
    {
        modeLine = h.modeLine;

        noteList = h.regNoteList;
        noteList.AddRange(h.longNoteList);

        bpmList = h.bpmList.Count == 0 ? i.bpmList : h.bpmList;

        noteList.Sort();
        noteCount = noteList.Count;
    }

    public static SerializableSheet GetSerializableSheet(SheetManager m)
    {
        return null;
    }
}

