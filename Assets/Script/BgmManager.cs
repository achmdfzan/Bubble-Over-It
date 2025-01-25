using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class BgmManager : Singleton<BgmManager>
{
    [Header("Clips")]
    [SerializeField] private AudioClip menuBgm;
    [SerializeField] private AudioClip gameplayBgm;
    [SerializeField] private AudioClip endingBgm;

    private AudioSource audioSource;

    private void Awake()
    {
        BaseAwake(this);

        audioSource = GetComponent<AudioSource>();
    }

    private void PlayBGM(AudioClip bgm)
    {
        if (audioSource.clip == bgm) return;

        audioSource.Stop();
        audioSource.clip = bgm;
        audioSource.Play();
    }

    public void PlayMenuBGM() { PlayBGM(menuBgm); }
    public void PlayGameplayBGM() { PlayBGM(gameplayBgm); }
    public void PlayEndingBGM() { PlayBGM(endingBgm); }


    public void SetMute(bool value)
    {
        audioSource.mute = value;
    }


    private void OnDestroy()
    {
        BaseOnDestroy();
    }
}