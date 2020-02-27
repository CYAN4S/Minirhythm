using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public GameObject audioSourcePrefab;
    public List<AudioSource> audioSources = new List<AudioSource>();
    public List<AudioClip> audios;

    private void Start() 
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
}
