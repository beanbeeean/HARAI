using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [SerializeField] private List<SoundEntry> sfxList;
    [SerializeField] private List<SoundEntry> bgmList;

    private Dictionary<SoundType, SoundEntry> sfxDictionary = new Dictionary<SoundType, SoundEntry>();
    private Dictionary<SoundType, SoundEntry> bgmDictionary = new Dictionary<SoundType, SoundEntry>();

    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private AudioClip[] walkClips;

    [SerializeField] private float customBGMVolume = 0.5f;
    [SerializeField] private float customSFXVolume = 0.5f;

    [SerializeField] private SoundEntry currentBGMData;

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
            PlayBGM(SoundType.TitleBGM);
            
        }
        else
        {
            Destroy(gameObject);
        }

    }

    //Test
    private void Start()
    {
        GameSceneManager.Instance.changeSceneEvent += UpdateBGM;
    }

    void OnDestroy()
    {
        GameSceneManager.Instance.changeSceneEvent -= UpdateBGM;
    }
    private void UpdateBGM(string sceneName)
    {
        bgmSource.Stop();

        switch (sceneName)
        {
            case "Title":
                PlayBGM(SoundType.TitleBGM);
                break;
            case "Game":
                PlayBGM(SoundType.GameBGM);
                break;
            case "Story":
                PlayBGM(SoundType.StoryBGM);
                break;
            default:
                break;
        }
        

    }

    private void InitDictionaries()
    {
        foreach (SoundEntry sfx in sfxList)
        {
            if (!sfxDictionary.ContainsKey(sfx.type))
            {
                sfxDictionary.Add(sfx.type, sfx);
            }
        }

        foreach (SoundEntry bgm in bgmList)
        {
            if (!bgmDictionary.ContainsKey(bgm.type))
            {
                bgmDictionary.Add(bgm.type, bgm);
            }
        }
    }

    public void PlaySFX(SoundType type)
    {
        if (sfxDictionary.TryGetValue(type, out SoundEntry sfxData))
        {
            float finalVolume = sfxData.defaultVolume * customSFXVolume;
            sfxSource.PlayOneShot(sfxData.clip, finalVolume);
        }
    }

    public void PlayBGM(SoundType type)
    {
        if (bgmDictionary.TryGetValue(type, out SoundEntry bgmData))
        {
            if (bgmSource.isPlaying && bgmSource.clip == bgmData.clip) return;

            bgmSource.volume = bgmData.defaultVolume * customBGMVolume;  
            bgmSource.clip = bgmData.clip;
            bgmSource.loop = true;
            bgmSource.Play();

            currentBGMData = bgmData;
        }
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void PlaySFXEnding(SoundType type, float volume)
    {
        if (sfxDictionary.TryGetValue(type, out SoundEntry sfxData))
        {
            float endingVolume = volume;
            sfxSource.PlayOneShot(sfxData.clip, endingVolume);
        }

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
        customBGMVolume = volume;
        
        if (currentBGMData != null)
        {
            bgmSource.volume = currentBGMData.defaultVolume * customBGMVolume;
        }
        //bgmSource.Play();
    }

    public void SetSFXVolume(float volume)
    {
        customSFXVolume = volume;
        sfxSource.volume = customSFXVolume;
        if (footstepSource != null)
        {
            footstepSource.volume = customSFXVolume;
        }

        OnSFXVolumeChanged?.Invoke(customSFXVolume);
    }

    public float GetCustomSFXVolume()
    {
        return customSFXVolume;
    }

    public float GetCustomBGMVolume()
    {
        return customBGMVolume;
    }
}
