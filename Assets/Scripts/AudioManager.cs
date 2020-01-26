using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource audioSource;
    public List<AudioClip> audios;

    private void Start() 
    {
        audioSource = GetComponent<AudioSource>();
    }
    
    public void PlayAudioClip(int code)
    {
        audioSource.clip = audios[(int)code];
        audioSource.Play();
    }
}
