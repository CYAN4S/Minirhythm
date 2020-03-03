using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

public class AudioManager : MonoBehaviour
{
    public GameObject audioSourcePrefab;
    public List<AudioSource> audioSources = new List<AudioSource>();
    public List<AudioClip> audios = new List<AudioClip>();
    public static AudioManager instance;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        //MakeAudioSources();
    }

    public void SetAudioManager(FileObj fileObj)
    {
        GetAudioClips(fileObj);
        MakeAudioSources();
    }

    public void MakeAudioSources()
    {
        foreach (AudioClip item in audios)
        {
            AudioSource obj = null;

            if (item != null)
            {
                obj = Instantiate(audioSourcePrefab, transform).GetComponent<AudioSource>();
                obj.clip = item;
            }
            audioSources.Add(obj);
        }
    }

    public void PlayAudioClip(int code)
    {
        if (code < 0 || code >= audioSources.Count) return;
        if (audioSources[code] == null) return;

        audioSources[code].Play();
    }

    IEnumerator GetAudioClips(FileObj fileObj)
    {
        string basePath = "file://" + fileObj.directory.FullName;

        foreach (string fileName in fileObj.info.audios)
        {
            string path;
            audios.Add(null);

            using (UnityWebRequest www =
            UnityWebRequestMultimedia.GetAudioClip(path = Path.Combine(basePath, fileName), AudioType.WAV))
            {
                yield return www.SendWebRequest();
                if (www.isNetworkError)
                {
                    Debug.Log(path + " : ERROR");
                    break;
                }

                audios[audios.Count - 1] = DownloadHandlerAudioClip.GetContent(www);
            }
        }
    }
}
