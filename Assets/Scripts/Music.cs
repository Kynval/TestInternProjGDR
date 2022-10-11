using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    [SerializeField] AudioClip[] musicList;

    public AudioSource musicSource;

    void OnEnable()
    {
        PlayMusicList();
    }

    void PlayMusicList()
    {
        musicSource.clip = musicList[Random.Range(0, musicList.Length)];
        musicSource.Play();
        Invoke("PlayMusicList", musicSource.clip.length);
    }
}
