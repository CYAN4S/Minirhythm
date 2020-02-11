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
    //public List<Queue<Tuple<GameObject, NoteComponent>>> notesByLines; // LEGACY
    public List<Queue<QObj>> notesByLines = new List<Queue<QObj>>();
    private KeyCode[] KeyCodes;
    #endregion

    #region SYSTEM SETTINGS
    public const float WAITTIME = 5f;
    public const double WAITTIMING = 2;
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

    private List<bool> isInLongNote;
    private List<Judgement> startJudge;
    private List<QObj> activeLongNote;
    private List<float> endTime;

    private List<Queue<float>> tickTime;

    private List<LongObj> LongObjs = new List<LongObj>();
    #endregion


    private void Start()
    {
        SetKeyCodes();
        uICon.SetPressingEffects(sheetM.modeLine);
        ClassifyNote();
        MoveNotes();
        ApplyHealth();
    }


    private void Update()
    {
        JudgePlayInput();
        ProcessSettingInput();
        RemoveBreakNote();
        RemoveHolding();
    }

    private void LateUpdate()
    {
        MoveNotes();
    }


    private void SetKeyCodes()
    {
        switch (sheetM.modeLine)
        {
            case 4: KeyCodes = inputM.KeyCodes4K; break;
            case 5: KeyCodes = inputM.KeyCodes5K; break;
            case 6: KeyCodes = inputM.KeyCodes6K; break;
            case 8: KeyCodes = inputM.KeyCodes8K; break;
        }

        isInLongNote = new List<bool>();
        startJudge = new List<Judgement>();
        activeLongNote = new List<QObj>();
        endTime = new List<float>();
        tickTime = new List<Queue<float>>();
        for (int i = 0; i < sheetM.modeLine; i++)
        {
            isInLongNote.Add(false);
            startJudge.Add(Judgement.NONE);
            activeLongNote.Add(null);
            endTime.Add(0);
            tickTime.Add(new Queue<float>());

            LongObjs.Add(new LongObj());
        }

    }

    private void ClassifyNote()
    {

        for (int i = 0; i < sheetM.modeLine; i++)
            notesByLines.Add(new Queue<QObj>());

        foreach (var noteData in sheetM.noteList)
        {
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
        noteComponent.time = TimeCalc.GetTime(noteData.timing + WAITTIMING, sheetM) + WAITTIME;
        return new QObj(gameObject, noteComponent);
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

            if (Input.GetKeyDown(KeyCodes[i]))
            {
                QObj peek = notesByLines[i].Peek();

                audioM.PlayAudioClip(peek.noteComponent.noteData.audioCode);

                if (peek.noteComponent.noteData is LongNoteData && isInLongNote[i] == false)
                {
                    activeLongNote[i] = peek;
                    HandleLongNoteDown(i);
                }
                else
                {
                    HandleNote(i, peek);
                }
            }

            else if (Input.GetKey(KeyCodes[i]) && isInLongNote[i])
            {
                HandleLongNote(i);
            }

            else if (Input.GetKeyUp(KeyCodes[i]) && isInLongNote[i])
            {
                HandleLongNoteUp(i);
            }
        }
    }

    private void HandleNote(int i, QObj qObj)
    {
        float time = qObj.noteComponent.time;
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
        float time = activeLongNote[i].noteComponent.time;
        Judgement judgement = JudgeGap(time - Time.time);

        if (judgement == Judgement.NONE)
            return;

        isInLongNote[i] = true;


        LongNoteData longNoteData = activeLongNote[i].noteComponent.noteData as LongNoteData;

        ApplyHealth(healthRecovers[(int)judgement]);
        uICon.JudgeEffect(judgeString[(int)judgement], time - Time.time);

        if (judgement <= Judgement.GREAT)
            startJudge[i] = judgement;
        else
            startJudge[i] = Judgement.NICE;

        endTime[i] = time + TimeCalc.GetTime(longNoteData.lengthTiming, sheetM);
        for (double ti = longNoteData.timing + 0.25; ti < longNoteData.timing + longNoteData.lengthTiming; ti += 0.25)
        {
            tickTime[i].Enqueue(TimeCalc.GetTime(ti + WAITTIMING, sheetM) + WAITTIME);
        }


        if (judgement != Judgement.BAD)
            IncreaseCombo();
        else
            ResetCombo();
    }

    private void HandleLongNote(int i)
    {
        if (tickTime[i].Count == 0)
            return;
        if (tickTime[i].Peek() > Time.time)
            return;

        Debug.Log(tickTime[i].Peek());
        HandleLongNoteTick(i);
        tickTime[i].Dequeue();

    }

    private void HandleLongNoteTick(int i)
    {
        Judgement judgement = startJudge[i];
        ApplyHealth(healthRecovers[(int)judgement] / 10f);
        uICon.JudgeEffect(judgeString[(int)judgement]);
        IncreaseCombo();
    }

    private void HandleLongNoteUp(int i)
    {
        float time = endTime[i];
        Judgement judgement = JudgeGap(time - Time.time);

        if (judgement <= Judgement.BAD)
        {
            IncreaseScore(scoreRate[(int)startJudge[i]]);
            ApplyHealth(healthRecovers[(int)startJudge[i]]);
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
        activeLongNote[i] = null;
        startJudge[i] = Judgement.NONE;
        isInLongNote[i] = false;
        activeLongNote[i] = null;
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
        uICon.ScoreEffect((double)rawScore / (totalNote * scoreRate[0]) * 300000);

    }


    private void ProcessSettingInput()
    {

    }


    private void RemoveBreakNote()
    {
        for (int i = 0; i < notesByLines.Count; i++)
        {
            if (isInLongNote[i])
                continue;

            Queue<QObj> notesQueue = notesByLines[i];
            if (notesQueue.Count == 0)
                continue;

            var noteComponent = notesQueue.Peek().noteComponent;
            float time = noteComponent.time;

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
            if (!isInLongNote[i])
                continue;

            if (Time.time - endTime[i] > judgeTimeDif[3])
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
    public NoteComponent noteComponent;

    public QObj(GameObject gameObject = null, NoteComponent noteComponent = null)
    {
        this.gameObject = gameObject;
        this.noteComponent = noteComponent;
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
