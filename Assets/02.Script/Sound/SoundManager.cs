using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [SerializeField] private List<SoundEffect> sfxList;
    [SerializeField] private List<SoundEffect> bgmList;

    private Dictionary<string, SoundEffect> sfxDictionary = new Dictionary<string, SoundEffect>();
    private Dictionary<string, SoundEffect> bgmDictionary = new Dictionary<string, SoundEffect>();

    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private AudioClip[] walkClips;

    public event Action<float> OnSFXVolumeChanged;

    private int currentStepIndex = 0;
    private bool isIncreasing = true;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitDictionaries();

        }
        else
        {
            Destroy(gameObject);
        }

    }

    //Test
    private void Start()
    {
        PlayBGM("GameBGM");
    }

    private void InitDictionaries()
    {
        foreach (SoundEffect sfx in sfxList)
        {
            if (!string.IsNullOrEmpty(sfx.name) && !sfxDictionary.ContainsKey(sfx.name))
            {
                sfxDictionary.Add(sfx.name, sfx);
            }
        }

        foreach (SoundEffect bgm in bgmList)
        {
            if (!string.IsNullOrEmpty(bgm.name) && !bgmDictionary.ContainsKey(bgm.name))
            {
                bgmDictionary.Add(bgm.name, bgm);
            }
        }
    }

    public void PlaySFX(string clipName)
    {
        if (sfxDictionary.TryGetValue(clipName, out SoundEffect sfxData))
        {
            float finalVolume = sfxData.defaultVolume * sfxSource.volume;
            sfxSource.PlayOneShot(sfxData.clip, finalVolume);
        }
    }

    public void PlayBGM(string clipName)
    {
        if (bgmDictionary.TryGetValue(clipName, out SoundEffect bgmData))
        {
            if (bgmSource.isPlaying && bgmSource.clip == bgmData.clip) return;

            bgmSource.volume = bgmData.defaultVolume * bgmSource.volume;  
            bgmSource.clip = bgmData.clip;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    public void StopBGM() { 
        bgmSource.Stop();
    }

    public void PlayWalkSound()
    {
        if (walkClips == null || walkClips.Length <= 0) return;

        footstepSource.clip = walkClips[currentStepIndex];
        footstepSource.Play();

        if (isIncreasing)
        {
            currentStepIndex++;
            if(currentStepIndex >= walkClips.Length - 1)
            {
                isIncreasing = false;
            }
        }
        else
        {
            currentStepIndex--;
            if (currentStepIndex <= 0) isIncreasing = true;
            
        }
    }

    public void StopWalkSound()
    {
        footstepSource.Stop();
    }

    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = volume;
        //bgmSource.Play();
    }

    public void SetSFXVolume(float volume)
    {

        sfxSource.volume = volume;
        if (footstepSource != null)
        {
            footstepSource.volume = volume;
        }

        OnSFXVolumeChanged?.Invoke(volume);
    }

    public float GetSFXVolume()
    {
        return sfxSource.volume;
    }
}
