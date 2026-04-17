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

    [SerializeField] private float customBGMVolume = 0.5f;
    [SerializeField] private float customSFXVolume = 0.5f;

    [SerializeField] private SoundEffect currentBGMData;

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
            PlayBGM("Title");
            
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
        if(sceneName != "Ending")
        {
            PlayBGM(sceneName);
        }

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
            float finalVolume = sfxData.defaultVolume * customSFXVolume;
            sfxSource.PlayOneShot(sfxData.clip, finalVolume);
        }
    }

    public void PlayBGM(string clipName)
    {
        if (bgmDictionary.TryGetValue(clipName, out SoundEffect bgmData))
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

    public void PlaySFXEnding(string clipName, float volume)
    {
        if (sfxDictionary.TryGetValue(clipName, out SoundEffect sfxData))
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
