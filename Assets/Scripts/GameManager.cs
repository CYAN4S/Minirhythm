using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region UNITYEDITOR SETUPS
    public InputManager inputM;
    public SheetManager sheetM;
    public AudioManager audioM;
    public UIController uICon;
    public GameObject notePrefab, notesParent;
    #endregion

    #region USER SETTINGS
    public double scrollSpeed;
    #endregion

    #region INGAME INFO
    private int combo = 0, rawScore = 0;
    private float health = 0.8f;
    private bool isAlive = true;
    #endregion

    #region PERFORMANCE VARIABLES
    private int totalNote;
    public List<Queue<QObj>> notesByLines = new List<Queue<QObj>>();
    public Queue<NoteData> bgmNotes = new Queue<NoteData>();
    #endregion

    #region SYSTEM SETTINGS
    public static readonly float WAITTIME = 5f;
    public static readonly double WAITTIMING = 2;
    private static readonly double[] judgeTimeDif = { 0.05, 0.1, 0.3, 0.5 };
    private static readonly int[] scoreRate = { 100, 80, 50, 10 };
    private static readonly float[] healthRecovers = { 0.05f, 0.02f, 0.01f, -0.05f, -0.1f };
    private readonly float[][] xPoses =
    {
            new float[] { -300, -100, 100, 300}, // 4K
            new float[] { -300, -100, 100, 300}, // 5K
            new float[] { -300, -100, 100, 300}, // 6K
            new float[] { },                     // Since we don't have 7K...
            new float[] { -300, -100, 100, 300}  // 8K
        };
    private readonly string[] judgeString = { "PRECISE", "GREAT", "NICE", "BAD", "BREAK", "FAULT" };
    private List<LongObj> LongObjs = new List<LongObj>();
    #endregion


    private void Start()
    {
        inputM = InputManager.instance;
        audioM = AudioManager.instance;
        ClassifyNote();
        MoveNotes();
        ApplyHealth();
    }


    private void Update()
    {
        PlayBgm();
        JudgePlayInput();
        ProcessSettingInput();
        RemoveBreakNote();
        RemoveHolding();
    }

    private void LateUpdate()
    {
        MoveNotes();
    }


    private void ClassifyNote()
    {
        for (int i = 0; i < sheetM.modeLine; i++)
        {
            notesByLines.Add(new Queue<QObj>());
            LongObjs.Add(new LongObj());
        }

        foreach (var noteData in sheetM.noteList)
        {
            noteData.SetTime(sheetM);
            

            if (noteData.line < 0 || noteData.line >= sheetM.modeLine)
            {
                bgmNotes.Enqueue(noteData);
                continue;
            }

            var noteObject = MakeNoteObject(noteData);
            notesByLines[noteData.line].Enqueue(noteObject);
        }

        totalNote = sheetM.noteList.Count;
    }


    private QObj MakeNoteObject(NoteData noteData)
    {
        var gameObject = Instantiate(notePrefab, notesParent.transform);
        var noteComponent = gameObject.GetComponent<NoteComponent>();

        if (noteData is LongNoteData longNoteData)
        {
            gameObject.GetComponent<RectTransform>().sizeDelta += new Vector2(0, GetNoteYLength(longNoteData.lengthTiming));
        }

        noteComponent.noteData = noteData;
        //noteComponent.time = TimeCalc.GetTime(noteData.timing + WAITTIMING, sheetM) + WAITTIME;
        return new QObj(gameObject, noteData);
    }


    private void MoveNotes()
    {
        foreach (var notesQueue in notesByLines)
        {
            foreach (var note in notesQueue)
                MoveNote(note.gameObject);
        }
    }


    private void MoveNote(GameObject note)
    {
        NoteData noteData = note.GetComponent<NoteComponent>().noteData;
        note.transform.localPosition = new Vector3
        (
            GetNoteXPos(noteData.line),
            GetNoteYPos(noteData.timing - TimeCalc.GetTiming(Time.time - WAITTIME, sheetM) + WAITTIMING),
            0
        );
    }


    private void JudgePlayInput()
    {
        for (int i = 0; i < sheetM.modeLine; i++)
        {
            if (notesByLines[i].Count == 0)
                continue;

            if (Input.GetKeyDown(inputM.KeyCodes[i]))
            {
                QObj peek = notesByLines[i].Peek();

                audioM.PlayAudioClip(peek.noteData.audioCode);

                if (peek.noteData is LongNoteData && LongObjs[i].isInLongNote == false)
                {
                    LongObjs[i].qObj = peek;
                    HandleLongNoteDown(i);
                }
                else
                {
                    HandleNote(i, peek);
                }
            }

            else if (Input.GetKey(inputM.KeyCodes[i]) && LongObjs[i].isInLongNote)
            {
                HandleLongNote(i);
            }

            else if (Input.GetKeyUp(inputM.KeyCodes[i]) && LongObjs[i].isInLongNote)
            {
                HandleLongNoteUp(i);
            }
        }
    }

    private void HandleNote(int i, QObj qObj)
    {
        float time = qObj.noteData.Time;
        Judgement judgement = JudgeGap(time - Time.time);

        if (judgement == Judgement.NONE)
            return;

        IncreaseScore(scoreRate[(int)judgement]);
        ApplyHealth(healthRecovers[(int)judgement]);
        uICon.JudgeEffect(judgeString[(int)judgement], time - Time.time);

        if (judgement != Judgement.BAD)
            IncreaseCombo();
        else
            ResetCombo();

        notesByLines[i].Dequeue().gameObject.SetActive(false);
    }

    private void HandleLongNoteDown(int i)
    {
        float time = LongObjs[i].qObj.noteData.Time;
        Judgement judgement = JudgeGap(time - Time.time);

        if (judgement == Judgement.NONE)
            return;

        LongObjs[i].isInLongNote = true;


        LongNoteData longNoteData = LongObjs[i].qObj.noteData as LongNoteData;

        ApplyHealth(healthRecovers[(int)judgement]);
        uICon.JudgeEffect(judgeString[(int)judgement], time - Time.time);

        if (judgement <= Judgement.GREAT)
            LongObjs[i].judgement = judgement;
        else
            LongObjs[i].judgement = Judgement.NICE;

        LongObjs[i].endTime = time + TimeCalc.GetTime(longNoteData.lengthTiming, sheetM);
        for (double ti = longNoteData.timing + 0.25; ti < longNoteData.timing + longNoteData.lengthTiming; ti += 0.25)
        {
            LongObjs[i].tick.Enqueue(TimeCalc.GetTime(ti + WAITTIMING, sheetM) + WAITTIME);
        }


        if (judgement != Judgement.BAD)
            IncreaseCombo();
        else
            ResetCombo();
    }

    private void PlayBgm()
    {
        while (bgmNotes.Count != 0)
        {
            if (bgmNotes.Peek().Time > Time.time)
                return;
            
            audioM.PlayAudioClip(bgmNotes.Dequeue().audioCode);
        }
    }

    private void HandleLongNote(int i)
    {
        if (LongObjs[i].tick.Count == 0)
            return;
        if (LongObjs[i].tick.Peek() > Time.time)
            return;

        HandleLongNoteTick(i);
        LongObjs[i].tick.Dequeue();

    }

    private void HandleLongNoteTick(int i)
    {
        Judgement judgement = LongObjs[i].judgement;
        ApplyHealth(healthRecovers[(int)judgement] / 10f);
        uICon.JudgeEffect(judgeString[(int)judgement]);
        IncreaseCombo();
    }

    private void HandleLongNoteUp(int i)
    {
        float time = LongObjs[i].endTime;
        Judgement judgement = JudgeGap(time - Time.time);

        if (judgement <= Judgement.BAD)
        {
            IncreaseScore(scoreRate[(int)LongObjs[i].judgement]);
            ApplyHealth(healthRecovers[(int)LongObjs[i].judgement]);
            uICon.JudgeEffect(judgeString[(int)judgement], time - Time.time);
            IncreaseCombo();
        }
        else
        {
            ApplyHealth(healthRecovers[4]);
            uICon.JudgeEffect(judgeString[4]);
            ResetCombo();
        }
        ResetLong(i);
    }

    private void ResetLong(int i)
    {
        notesByLines[i].Dequeue().gameObject.SetActive(false);
        LongObjs[i].qObj = null;
        LongObjs[i].judgement = Judgement.NONE;
        LongObjs[i].isInLongNote = false;
        LongObjs[i].qObj = null;
    }



    private Judgement JudgeGap(float gap)
    {
        float absGap = Mathf.Abs(gap);
        if (absGap <= judgeTimeDif[0]) return Judgement.PRECISE;
        else if (absGap <= judgeTimeDif[1]) return Judgement.GREAT;
        else if (absGap <= judgeTimeDif[2]) return Judgement.NICE;
        else if (absGap <= judgeTimeDif[3]) return Judgement.BAD;
        else return Judgement.NONE;
    }


    private void IncreaseCombo()
    {
        combo++;
        uICon.ComboEffect(combo);
    }


    private void ResetCombo()
    {
        combo = 0;
        uICon.ComboResetEffect(combo);
    }


    private void IncreaseScore(int ratio)
    {
        rawScore += ratio;
        uICon.ScoreEffect((double)rawScore / (sheetM.noteCount * scoreRate[0]) * 300000);

    }


    private void ProcessSettingInput()
    {

    }


    private void RemoveBreakNote()
    {
        for (int i = 0; i < notesByLines.Count; i++)
        {
            if (LongObjs[i].isInLongNote)
                continue;

            Queue<QObj> notesQueue = notesByLines[i];
            if (notesQueue.Count == 0)
                continue;

            float time = notesQueue.Peek().noteData.Time;

            if (Time.time - time > judgeTimeDif[3])
            {
                notesQueue.Dequeue().gameObject.SetActive(false);
                ResetCombo();
                uICon.JudgeEffect("BREAK", 0);
                ApplyHealth(healthRecovers[4]);
            }
        }
    }

    private void RemoveHolding()
    {
        for (int i = 0; i < notesByLines.Count; i++)
        {
            if (!LongObjs[i].isInLongNote)
                continue;

            if (Time.time - LongObjs[i].endTime > judgeTimeDif[3])
            {
                ResetLong(i);
                ApplyHealth(healthRecovers[2]);
                IncreaseScore(scoreRate[2]);
                ApplyHealth(healthRecovers[2]);
                uICon.JudgeEffect(judgeString[2]);
                IncreaseCombo();
            }
        }
    }


    private float GetNoteYPos(double timing) => (float)(timing * scrollSpeed * 405);
    private float GetNoteYLength(double lengthTiming) => (float)(lengthTiming * scrollSpeed * 405);
    private float GetNoteXPos(int line) => xPoses[sheetM.modeLine - 4][line];


    private void ApplyHealth(float delta = 0)
    {
        if (!isAlive) return;

        health = (health + delta > 1) ? 1 : health + delta;

        if (health < 0)
        {
            isAlive = false;
            health = 0;
        }

        uICon.HealthEffect(health);
    }
}

public enum Judgement { PRECISE, GREAT, NICE, BAD, NONE }

// 
public class QObj
{
    public GameObject gameObject;
    public NoteData noteData;

    public QObj(GameObject gameObject = null, NoteData noteData = null)
    {
        this.gameObject = gameObject;
        this.noteData = noteData;
    }
}

public class LongObj
{
    public bool isInLongNote;
    public Judgement judgement;
    public QObj qObj;
    public float endTime;
    public Queue<float> tick;

    public LongObj()
    {
        isInLongNote = false;
        judgement = Judgement.NONE;
        qObj = null;
        endTime = -1;
        tick = new Queue<float>();
    }
}
