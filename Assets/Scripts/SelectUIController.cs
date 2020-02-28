using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectUIController : MonoBehaviour
{
    public GameObject uL;
    public GameObject lIPrefab;

    public TextMeshProUGUI[] Difficulties;


    public GameObject startButton;

    public int currentLine = 4;

    public void SetSelectMusicUI(List<FileObj> fileObjs)
    {
        for (int i = 0; i < fileObjs.Count; i++)
        {
            FileObj item = (FileObj)fileObjs[i];

            var obj = Instantiate(lIPrefab, uL.transform);
            obj.transform.localPosition = new Vector3(0, -200 * i);

            var licomp = obj.GetComponent<LIComponent>();
            licomp.fileObj = item;
            licomp.title.text = item.info.songName;
            licomp.controller = this;
        }
    }

    //private SerializableSheet[,] choosing = new SerializableSheet[4, 4];
    public void SetChoseMusicUI(FileObj fileObj)
    {
        foreach (SerializableSheet item in fileObj.sheets)
        {
            int l, t; double d;
            l = item.modeLine;
            t = item.difficultyType;
            d = item.difficulty;

            Difficulties[t].text = d.ToString("0.0");
        }
    }


}
