using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public GameObject audioSourcePrefab;
    public List<AudioSource> audioSources = new List<AudioSource>();
    public List<AudioClip> audios;
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

    public void MakeAudioSources()
    {
        foreach (AudioClip item in audios)
        {
            AudioSource obj = Instantiate(audioSourcePrefab, transform).GetComponent<AudioSource>();
            obj.clip = item;
            audioSources.Add(obj);
        }
    }

    public void PlayAudioClip(int code)
    {
        audioSources[code].Play();
    }

    public void GetAudioClips()
    {
        
    }
}
