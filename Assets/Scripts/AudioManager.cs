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

    private AudioSource audioSource;

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
        audioSource = GetComponent<AudioSource>();
    }

    public void SetAudioManager(FileObj fileObj)
    {
        Debug.Log("SET");
        StartCoroutine(GetAudioClips(fileObj));
        //MakeAudioSources();
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
        // if (code < 0 || code >= audioSources.Count)
        // {
        //     Debug.LogError($"Clip code was out of range. {audioSources.Count} / {code}");
        //     return;
        // }

        // if (audioSources[code] == null)
        // {
        //     Debug.LogWarning($"Index {code} AudioClip is missing. No Key Sound.");
        //     return;
        // }

        //audioSources[code].Play();

        audioSource.PlayOneShot(audios[code]);
    }

    IEnumerator GetAudioClips(FileObj fileObj)
    {
        string basePath = fileObj.directory.FullName;
        Debug.Log(basePath);

        for (int i = 0; i < fileObj.info.audios.Count; i++)
        {
            string fileName = fileObj.info.audios[i];
            string path = Path.Combine(basePath, fileName);
            Debug.Log(path);
            audios.Add(null);
            if (File.Exists(path))
            {
                Debug.Log($"{i}: {path}");
            }
            else
            {
                Debug.Log($"{i}: audio file {path} not exists.");
                continue;
            }


            using (UnityWebRequest www =
            UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV))
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
