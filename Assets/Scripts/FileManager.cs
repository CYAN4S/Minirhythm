using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using UnityEngine;

public class FileManager : MonoBehaviour
{
    string SongsDirString = System.IO.Path.Combine(Application.streamingAssetsPath, "Songs");

    DirectoryInfo SongsFolder;

    public SelectUIController selectUIController;

    // TESTING
    public List<SerializableSheet> sheets;
    public List<SerializableInfo> infos;

    public List<FileObj> fileObjs = new List<FileObj>();

    private void Start()
    {
        SongsFolder = new DirectoryInfo(SongsDirString);
        if (!SongsFolder.Exists)
        {
            SongsFolder.Create();
            return;
        }

        DirectoryInfo[] Folders = SongsFolder.GetDirectories();
        foreach (DirectoryInfo folder in Folders)
        {
            FileInfo[] infoFile = folder.GetFiles("info.mri");
            if (infoFile.Count() != 1)
                continue;

            FileInfo[] sheetFiles = folder.GetFiles("*.mrs");
            if (sheetFiles.Count() < 1)
                continue;

            SerializableInfo info;
            List<SerializableSheet> sheets = new List<SerializableSheet>();

            using (StreamReader sr = infoFile[0].OpenText())
            {
                string infoJson = sr.ReadToEnd();
                info = JsonUtility.FromJson<SerializableInfo>(infoJson);
                if (!info.IsValid())
                    continue;
            }

            foreach (FileInfo mrs in sheetFiles)
            {
                using (StreamReader sr = mrs.OpenText())
                {
                    string sheetJson = sr.ReadToEnd();
                    SerializableSheet sheet = JsonUtility.FromJson<SerializableSheet>(sheetJson);
                    if (!sheet.IsValid())
                        continue;
                    sheets.Add(sheet);
                }
            }

            if (sheets.Count == 0)
                continue;

            FileObj newObj = new FileObj(folder, info, sheets);
            fileObjs.Add(newObj);
            // Debug.Log(folder.FullName);
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
