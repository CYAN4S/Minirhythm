using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectUIController : MonoBehaviour
{
    public GameObject uL;
    public GameObject lIPrefab;

    public Button[] difficultiesButton;
    public TextMeshProUGUI[] DifficultiesTMPro;

    public GameObject RandomPanel;
    public GameObject startButton;

    public int currentLineIndex = 0;
    public FileObj currentObj;
    public SerializableInfo currentInfo;
    public SerializableSheet currentSheet;

    private void Start()
    {
        SetRandomSong();
    }

    public void SetSelectMusicUI(List<FileObj> fileObjs)
    {
        for (int i = 0; i < fileObjs.Count; i++)
        {
            FileObj item = (FileObj)fileObjs[i];

            var obj = Instantiate(lIPrefab, uL.transform);
            obj.transform.localPosition = new Vector3(0, -200 * (i + 1));

            var licomp = obj.GetComponent<LIComponent>();
            licomp.fileObj = item;
            licomp.title.text = item.info.songName;
            licomp.controller = this;
        }
    }

    private SerializableSheet[,] choosing = new SerializableSheet[4, 4];
    public void SetChoseMusicUI(FileObj fileObj)
    {
        currentObj = fileObj;
        currentInfo = fileObj.info;
        RandomPanel.SetActive(false);

        foreach (var item in difficultiesButton)
            item.interactable = false;

        foreach (var item in DifficultiesTMPro)
            item.text = "";


        foreach (SerializableSheet item in fileObj.sheets)
        {
            int l, t; double d;
            l = item.modeLine;
            t = item.difficultyType;
            d = item.difficulty;

            choosing[LineIndex(l), t] = item;

            if (LineIndex(l) == currentLineIndex)
            {
                DifficultiesTMPro[t].text = d.ToString("0.0");
                difficultiesButton[t].interactable = true;
            }
        }
    }

    public void SetRandomSong()
    {
        RandomPanel.SetActive(true);
        foreach (var item in difficultiesButton)
            item.interactable = false;

        foreach (var item in DifficultiesTMPro)
            item.text = "";
    }

    public void SetDiff(int d)
    {
        currentSheet = choosing[currentLineIndex, d];
    }

    public int LineIndex(int line)
    {
        switch (line)
        {
            case 4: return 0;
            case 5: return 1;
            case 6: return 2;
            default: return 3;
        }
    }

    public void OnGameStart()
    {

    }
}
