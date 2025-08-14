using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

[Serializable]
public class AudioManagerParam
{
    public AudioMixer masterMixer;
    public AudioClip defaultBGM;

    public AudioMixerGroup bgmGroup; 
    public AudioMixerGroup sfxGroup;

    public List<AudioClip> uiSfxClips;
    public List<AudioClip> inGameSfxClips;
    public List<AudioClip> bgmClips;
}

public class AudioManager : MonoBehaviour
{
    //public static AudioManager Instance;

    private AudioSource bgmSource;
    private AudioSource sfxSource;

    private List<AudioClip> uiSfxClips;
    private List<AudioClip> inGameSfxClips;

    private Dictionary<string, AudioClip> sfxDict;
    private AudioMixer masterMixer;
    private List<AudioClip> bgmClips;
    //AudioManager.Instance.PlaySFX("Click"); 이런식으로 효과음을 sfx 클립에다가 가지고 있다면 click이라는 소리를 호출한다
    private Coroutine bgmTransitionCoroutine;

    private int currentBGMIndex = -1; //현재 재생중인 인덱스

    private void InitsfxDict()
    {
        sfxDict = new Dictionary<string, AudioClip>();

        if (uiSfxClips != null)
        {
            foreach (var clip in uiSfxClips)
            {
                if (clip != null)
                {
                    sfxDict[clip.name] = clip;
                }
            }
        }

        if (inGameSfxClips != null)
        {
            foreach (var clip in inGameSfxClips)
            {
                if (clip != null)
                {
                    sfxDict[clip.name] = clip;
                }
            }
        }
    }

    public void PlaySfx(string name)
    {
        if (sfxDict.TryGetValue(name, out var clip))
        {
            sfxSource.PlayOneShot(clip);
            // sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayVolumeSFX(AudioClip clip, float volume = 1f)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.PlayOneShot(clip, volume);
    }

    //private void Awake()
    //{

    //    // Singleton 패턴
    //    if (Instance == null)
    //    {
    //        Instance = this;
    //        InitsfxDict();
    //        DontDestroyOnLoad(gameObject); // 씬 넘어가도 유지

    //        LoadAndApplyVolumes();
    //    }
    //    else
    //    {
    //        Destroy(gameObject);
    //    }

    //}
    public void Init(AudioManagerParam param)
    {
        masterMixer = param.masterMixer;

        uiSfxClips = param.uiSfxClips;
        inGameSfxClips = param.inGameSfxClips;
        bgmClips = param.bgmClips;

        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.outputAudioMixerGroup = param.bgmGroup;
        bgmSource.playOnAwake = false;
        bgmSource.loop = true;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.outputAudioMixerGroup = param.sfxGroup;
        sfxSource.playOnAwake = false;

        InitsfxDict();
        LoadAndApplyVolumes();
    }

    private void PlayBGM(AudioClip clip, float volume = 1f)
    {
        if (bgmSource == null || clip == null) return;

        bgmSource.clip = clip;
        bgmSource.volume = volume;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        if (bgmSource != null && bgmSource.isPlaying)
        {
            bgmSource.Stop();
        }
    }

    public void FadeOutBGM(float duration)
    {
        if (bgmTransitionCoroutine != null) StopCoroutine(bgmTransitionCoroutine);
        bgmTransitionCoroutine = StartCoroutine(FadeOutBGMCoroutine(duration));
    }

    private IEnumerator FadeOutBGMCoroutine(float duration)
    {
        float startVolumeDB;
        masterMixer.GetFloat("BGMVolume", out startVolumeDB);
        float currentTime = 0;
        float targetVolumeDB = -80f;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newVolume = Mathf.Lerp(startVolumeDB, targetVolumeDB, currentTime / duration);
            masterMixer.SetFloat("BGMVolume", newVolume);
            yield return null;
        }

        StopBGM();
        masterMixer.SetFloat("BGMVolume", startVolumeDB);
    }


    public void Update()
    {
        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    PlayBGM(BattleClip);
        //}
        //if (Input.GetKeyDown(KeyCode.S)) 
        //{
        //    PlayBGM(BattleClip1);
        //}
        //if (Input.GetKeyDown(KeyCode.D)) 
        //{
        //    PlayBGM(BattleClip2);
        //}
    }

    public void LoadAndApplyVolumes()
    {
        if (masterMixer == null) return;

        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
        float bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 0.75f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.75f);

        masterMixer.SetFloat("MasterVolume", Mathf.Log10(masterVolume) * 20);
        masterMixer.SetFloat("BGMVolume", Mathf.Log10(bgmVolume) * 20);
        masterMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolume) * 20);
    }
    public void PlayBGMIndex(int index)
    {
        if (bgmClips == null || index < 0 || index >= bgmClips.Count)
        {
            Logger.LogWarning("BGM 리스트가 비어있거나 올바르지 않은 인덱스입니다.");
            return;
        }

        PlayBGMWithFade(index);
    }

    public void PlayBGMWithFade(int index, float transitionDuration = 1.0f)
    {
        if (bgmClips == null || index < 0 || index >= bgmClips.Count)
        {
            Logger.LogWarning("BGM 리스트가 비어있거나 올바르지 않은 인덱스입니다.");
            return;
        }

        AudioClip newClip = bgmClips[index];

        if (bgmSource.clip != newClip || !bgmSource.isPlaying)
        {
            if (bgmTransitionCoroutine != null) StopCoroutine(bgmTransitionCoroutine);

            if (bgmSource.isPlaying)
            {
                bgmTransitionCoroutine = StartCoroutine(TransitionBGMCoroutine(newClip, transitionDuration));
            }
            else
            {
                bgmTransitionCoroutine = StartCoroutine(FadeInBGMCoroutine(newClip, transitionDuration));
            }
        }
    }

    private IEnumerator TransitionBGMCoroutine(AudioClip newClip, float duration)
    {
        float startVolumeDB;
        masterMixer.GetFloat("BGMVolume", out startVolumeDB);
        float currentTime = 0;
        float targetVolumeDB = -80f;

        // 페이드 아웃
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newVolume = Mathf.Lerp(startVolumeDB, targetVolumeDB, currentTime / duration);
            masterMixer.SetFloat("BGMVolume", newVolume);
            yield return null;
        }

        bgmSource.Stop();
        bgmSource.clip = newClip;
        bgmSource.Play(); // 새로운 클립 재생

        // 페이드 인
        currentTime = 0;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newVolume = Mathf.Lerp(targetVolumeDB, startVolumeDB, currentTime / duration);
            masterMixer.SetFloat("BGMVolume", newVolume);
            yield return null;
        }
        masterMixer.SetFloat("BGMVolume", startVolumeDB);
    }

    private IEnumerator FadeInBGMCoroutine(AudioClip clip, float duration)
    {
        bgmSource.clip = clip;
        bgmSource.Play();

        float targetVolumeDB;
        masterMixer.GetFloat("BGMVolume", out targetVolumeDB); 

        float currentTime = 0;
        float startVolumeDB = -80f;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newVolume = Mathf.Lerp(startVolumeDB, targetVolumeDB, currentTime / duration);
            masterMixer.SetFloat("BGMVolume", newVolume);
            yield return null;
        }
        masterMixer.SetFloat("BGMVolume", targetVolumeDB);
    }
    public void SetVolume(string mixerName, float volume)
    {
        if (masterMixer == null) return;

        float db = (volume <= 0f) ? -80f : Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;

        masterMixer.SetFloat(mixerName, db);

        PlayerPrefs.SetFloat(mixerName, volume);
        PlayerPrefs.Save();
    }
}
