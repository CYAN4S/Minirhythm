using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using UnityEngine;

public class FileManager : MonoBehaviour
{
    string SongsDirString = System.IO.Path.Combine(Application.streamingAssetsPath, "Songs");

    DirectoryInfo SongsDir;

    public SelectUIController selectUIController;

    // TESTING
    public List<SerializableSheet> sheets;
    public List<SerializableInfo> infos;

    public List<FileObj> fileObjs = new List<FileObj>();

    private void Start()
    {
        SongsDir = new DirectoryInfo(SongsDirString);
        if (!SongsDir.Exists)
            SongsDir.Create();

        var SongDirs = SongsDir.GetDirectories();
        foreach (var item in SongDirs)
        {
            FileInfo[] a = item.GetFiles("info.mri");
            if (a.Count() != 1) continue;

            FileInfo[] b = item.GetFiles("*.mrs");
            if (b.Count() < 1) continue;

            SerializableInfo info;
            List<SerializableSheet> sheets = new List<SerializableSheet>();

            using (StreamReader sr = a[0].OpenText())
            {
                string infoString = sr.ReadToEnd();
                info = JsonUtility.FromJson<SerializableInfo>(infoString);
                if (!info.IsValid())
                    continue;
            }

            foreach (var mrs in b)
            {
                using (StreamReader sr = mrs.OpenText())
                {
                    string sheetString = sr.ReadToEnd();
                    var sheet = JsonUtility.FromJson<SerializableSheet>(sheetString);
                    if (!sheet.IsValid())
                        continue;
                    sheets.Add(sheet);
                }
            }

            if (sheets.Count == 0)
                continue;

            FileObj newObj = new FileObj(item, info, sheets);
            fileObjs.Add(newObj);
            Debug.Log(item.FullName);
        }

        selectUIController.SetSelectMusicUI(fileObjs);
    }
}

[Serializable]
public class FileObj
{
    public DirectoryInfo directory;
    public SerializableInfo info;
    public List<SerializableSheet> sheets;

    public FileObj(DirectoryInfo directory, SerializableInfo info, List<SerializableSheet> sheets)
    {
        this.directory = directory;
        this.info = info;
        this.sheets = sheets;
    }
}
