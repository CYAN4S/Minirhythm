using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectUIController : MonoBehaviour
{
    public GameObject uL;
    public GameObject lIPrefab;

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


        }
    }

    public void Add(FileObj fileObj)
    {
        
    }


}
