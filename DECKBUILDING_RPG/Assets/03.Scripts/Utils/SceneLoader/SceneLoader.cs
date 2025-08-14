using FadeStyleEnum;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }
    public bool IsSceneLoading { get; private set; } = false;

    //BGM이 바뀌어야 하는 씬 추가
    private HashSet<string> scenesToStopBGM = new HashSet<string>
    {
        "BattleScene",
        "StageReadyScenes" 
        //더있으면 추가
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void LoadSceneWithFade(string sceneName, float? fadeOutDuration = null, AnimationCurve fadeOutCurve = null, bool skipFade = false)
    {
        if (IsSceneLoading)
        {
            Logger.Log($"SceneLoader: 이미 씬 로드 중입니다.");
            return;
        }

        IsSceneLoading = true;

        if (SceneFader.Instance != null)
        {
            SceneFader.Instance.FadeOut(fadeOutDuration, fadeOutCurve, skipFade, () => {
                SceneManager.LoadScene(sceneName);
                IsSceneLoading = false;
            });
        }
    }
    public void LoadSceneWithFade(string sceneName, FadeStyle fadeStyle, bool skipFade = false) 
    {
        if (IsSceneLoading)
        {
            Debug.LogWarning($"SceneLoader: 이미 씬 로드 중입니다.");
            return;
        }

        IsSceneLoading = true;

        if (SceneFader.Instance == null)
        {
            Debug.LogWarning("SceneLoader: SceneFader.Instance가 없습니다 페이드 없이 씬을 로드");
            SceneManager.LoadScene(sceneName);
            return;
        }

        SceneFader.Instance.shouldSkipNextFadeIn = skipFade;

        SceneFader.Instance.GetFadeStyle(fadeStyle, out SceneFader.Instance.nextFadeInDuration, out SceneFader.Instance.nextFadeInCurve);

        SceneFader.Instance.FadeOut(fadeStyle, skipFade, () => {
            SceneManager.LoadScene(sceneName);
            IsSceneLoading = false;
        });
    }

    //로딩창추가
    public void LoadSceneWithLoadingScreen(string sceneName)
    {
        if (IsSceneLoading)
        {
            Logger.LogWarning($"SceneLoader: 이미 씬 로드 중입니다.");
            return;
        }

        IsSceneLoading = true;

        bool shouldStopBGM = scenesToStopBGM.Contains(sceneName);
        if (shouldStopBGM)
        {
            // BGM을 멈춰야 하는 씬이라면 페이드 아웃을 시작
            if (GameManager.Instance != null && GameManager.Instance.audioManager != null)
            {
                GameManager.Instance.audioManager.FadeOutBGM(1.5f);
            }
        }

        if (LoadingScreenManager.Instance != null)
        {
            LoadingScreenManager.Instance.ShowLoadingScreen(sceneName,shouldStopBGM);
        }
        else
        {
            // 로딩 화면이 없을 경우를 대비
            SceneManager.LoadScene(sceneName);
            CompleteLoading();
        }
    }

    public void CompleteLoading()
    {
        if (IsSceneLoading) // 로딩 중인 상태일 때만 리셋
        {
            IsSceneLoading = false;
        }

    }
}
