using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class SfxManager : Singleton<SfxManager>
{
    [Tooltip("Delay between sofaFallSFX and malePainSFX on PlayFallSFX()")]
    [SerializeField] private float delay = 0.1f;

    [Header("Clips")]
    [SerializeField] private AudioClip clickSfx;
    [SerializeField] private AudioClip bubbleBlowSfx;
    [SerializeField] private AudioClip bubblePopSfx;
    [SerializeField] private AudioClip rocketExhaustSfx;

    [SerializeField] private AudioClip sofaFallSfx;
    [SerializeField] private List<AudioClip> malePainSfxs;



    private AudioSource audioSource;

    private void Awake()
    {
        BaseAwake(this);

        audioSource = GetComponent<AudioSource>();
    }

    public void PlayClickSFX()
    {
        audioSource.PlayOneShot(clickSfx);
    }

    public void PlayBubbleBlowSFX()
    {
        audioSource.PlayOneShot(bubbleBlowSfx);
    }

    public void PlayBubblePopSFX()
    {
        audioSource.PlayOneShot(bubblePopSfx);
    }

    public void PlayRocketExhaustSFX()
    {
        audioSource.PlayOneShot(rocketExhaustSfx);
    }





    public void PlayFallSFX()
    {
        audioSource.PlayOneShot(sofaFallSfx);
        Invoke("PlayMalePainSFX", delay);
    }

    private void PlayMalePainSFX()
    {
        var chosenSfx = malePainSfxs[Random.Range(0, malePainSfxs.Count)];
        audioSource.PlayOneShot(chosenSfx);
    }

    


    public void SetMute(bool value)
    {
        audioSource.mute = value;
    }


    private void OnDestroy()
    {
        BaseOnDestroy();
    }
}