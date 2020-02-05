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
    public List<Queue<QObj>> notesByLines;
    private KeyCode[] KeyCodes;
    public List<double> judgeTimeDif;
    #endregion

    #region SYSTEM SETTINGS
    public const float WAITTIME = 5f;
    public const double WAITTIMING = 2;
    private static readonly double[] judgeCruel = { 0.05, 0.1, 0.3, 0.5 };
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
    private List<bool> isInLongNote;
    private List<Judgement> startJudge;
    private List<Tuple<GameObject, NoteComponent>> activeLongNote;
    #endregion


    private void Start()
    {
        SetKeyCodes();
        SetJudgeTimeDif();
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
        activeLongNote = new List<Tuple<GameObject, NoteComponent>>();
        for (int i = 0; i < sheetM.modeLine; i++)
        {
            isInLongNote.Add(false);
            startJudge.Add(Judgement.NONE);
            activeLongNote.Add(null);
        }

    }


    private void SetJudgeTimeDif()
    {
        judgeTimeDif = new List<double>();
        for (int i = 0; i < 4; i++)
            judgeTimeDif.Add(judgeCruel[i]);
    }


    private void ClassifyNote()
    {
        notesByLines = new List<Queue<QObj>>();
        for (int i = 0; i < sheetM.modeLine; i++)
            notesByLines.Add(new Queue<QObj>());

        foreach (var noteData in sheetM.noteList)
        {
            var noteObject = MakeNoteObject(noteData);
            notesByLines[noteData.line].Enqueue(noteObject);
        }

        totalNote = sheetM.noteList.Count + sheetM.noteList.Count;
    }


    private QObj MakeNoteObject(NoteData noteData)
    {
        var gameObject = Instantiate(notePrefab, notesParent.transform);
        var noteComponent = gameObject.GetComponent<NoteComponent>();

        if (noteData is LongNoteData)
        {
            gameObject.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 1000);
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
                var temp = notesByLines[i].Peek();
                var noteObject = temp.gameObject;
                var noteComponent = temp.noteComponent;
                var noteData = noteComponent.noteData;
                float time = noteComponent.time;

                audioM.PlayAudioClip(noteData.audioCode);

                if (noteData is LongNoteData && isInLongNote[i] == false)
                {
                    // JudgeLongDown(); and continue.
                }

                // else { JudgeNote(); }


                // I neeeeed to fix it...
                switch (JudgeGap(time - Time.time))
                {
                    case Judgement.PRECISE:
                        IncreaseCombo();
                        IncreaseScore(scoreRate[0]);
                        ApplyHealth(healthRecovers[0]);
                        notesByLines[i].Dequeue().gameObject.SetActive(false);
                        uICon.JudgeEffect("PRECISE", time - Time.time);

                        break;

                    case Judgement.GREAT:
                        IncreaseCombo();
                        IncreaseScore(scoreRate[1]);
                        ApplyHealth(healthRecovers[1]);
                        notesByLines[i].Dequeue().gameObject.SetActive(false);
                        uICon.JudgeEffect("GREAT", time - Time.time);
                        break;

                    case Judgement.NICE:
                        IncreaseCombo();
                        IncreaseScore(scoreRate[2]);
                        ApplyHealth(healthRecovers[2]);
                        notesByLines[i].Dequeue().gameObject.SetActive(false);
                        uICon.JudgeEffect("NICE", time - Time.time);
                        break;

                    case Judgement.BAD:
                        ResetCombo();
                        IncreaseScore(scoreRate[3]);
                        ApplyHealth(healthRecovers[3]);
                        notesByLines[i].Dequeue().gameObject.SetActive(false);
                        uICon.JudgeEffect("BAD", time - Time.time);
                        break;
                }
            }

            else if (Input.GetKey(KeyCodes[i]))
            {
                // JudgeLongGet();

            }

            else if (Input.GetKeyUp(KeyCodes[i]))
            {
                // JudgeLongUp();
            }
        }
    }

    private void JudgeNote()
    {

    }

    private void JudgeLongDown()
    {

    }

    private void JudgeLongGet()
    {

    }

    private void JudgeLongUp()
    {

    }


    private Judgement JudgeGap(float gap)
    {
        float absGap = Mathf.Abs(gap);
        if (absGap <= judgeTimeDif[0])
            return Judgement.PRECISE;

        else if (absGap <= judgeTimeDif[1])
            return Judgement.GREAT;

        else if (absGap <= judgeTimeDif[2])
            return Judgement.NICE;

        else if (absGap <= judgeTimeDif[3])
            return Judgement.BAD;

        else return Judgement.NONE;
    }


    enum Judgement
    {
        PRECISE, GREAT, NICE, BAD, NONE
    }


    private void IncreaseCombo()
    {
        combo++;
        uICon.ComboEffect(combo);
    }


    private void ResetCombo()
    {
        combo = 0;
    }


    private void IncreaseScore(int ratio)
    {
        rawScore += ratio;
        uICon.ScoreEffect(((double)rawScore / (totalNote * scoreRate[0])) * 300000);

    }


    private void ProcessSettingInput()
    {

    }


    private void RemoveBreakNote()
    {
        foreach (var notesQueue in notesByLines)
        {
            if (notesQueue.Count == 0)
                continue;

            var noteComponent = notesQueue.Peek().noteComponent;
            float time = noteComponent.time;

            if (Time.time - time > judgeTimeDif[3])
            {
                notesQueue.Dequeue().gameObject.SetActive(false);
                ResetCombo();
                Debug.Log("BREAK");
                uICon.JudgeEffect("BREAK", 0);
                ApplyHealth(healthRecovers[4]);
            }
        }
    }


    private float GetNoteYPos(double timing)
    {
        return (float)((timing) * scrollSpeed * 405);
    }


    private float GetNoteYLength(double lengthTiming)
    {
        return (float)(lengthTiming * scrollSpeed * 405);
    }


    private float GetNoteXPos(int line)
    {
        return xPoses[sheetM.modeLine - 4][line];
    }


    private void ApplyHealth(float delta = 0)
    {
        if (!isAlive) return;

        health += delta;

        if (health < 0)
        {
            isAlive = false;
            health = 0;
        }

        else if (health > 1)
        {
            health = 1;
        }
        uICon.HealthEffect(health);
    }


}

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