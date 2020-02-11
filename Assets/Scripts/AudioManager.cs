using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //private AudioSource audioSource;
    public GameObject audioSourcePrefab;
    public List<AudioSource> audioSources = new List<AudioSource>();
    public List<AudioClip> audios;

    private void Start() 
    {
        //audioSource = GetComponent<AudioSource>();

        foreach (AudioClip item in audios)
        {
            AudioSource obj = Instantiate(audioSourcePrefab, transform).GetComponent<AudioSource>();
            obj.clip = item;
            audioSources.Add(obj);
        }
    }
    
    public void PlayAudioClip(int code)
    {
        //audioSource.clip = audios[code];
        //audioSource.Play();

        audioSources[code].Play();
    }
}
