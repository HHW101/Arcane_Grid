using System.Collections;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using FadeStyleEnum;

public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance { get; private set; } // 싱글톤 인스턴스

    [HideInInspector]
    public bool shouldSkipNextFadeIn = false; // 씬 로드 후 FadeIn을 건너뛸지 결정
    [HideInInspector]
    public float nextFadeInDuration;         // 다음 FadeIn의 지속 시간
    [HideInInspector]
    public AnimationCurve nextFadeInCurve;   // 다음 FadeIn의 커브

    [SerializeField] private CanvasGroup canvasGroup; // 페이드 효과를 줄 UI Panel의 CanvasGroup
    [SerializeField] private float defaultfadeDuration = 1.0f; // 페이드 지속 시간
    [SerializeField] private AnimationCurve defaltfadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // 페이드 속도 곡선


    [Header("Custom Fade Styles")]
    [SerializeField] private float fastFadeDuration = 0.5f;
    [SerializeField] private AnimationCurve fastFadeCurve = AnimationCurve.Linear(0, 0, 1, 1); // 선형

    [SerializeField] private float slowFadeDuration = 2.0f;
    [SerializeField] private AnimationCurve slowFadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [SerializeField] private float flashFadeDuration = 0.2f;
    [SerializeField] private AnimationCurve flashFadeCurve = AnimationCurve.EaseInOut(0, 0, 0.5f, 1);

    private bool isFirstSceneLoaded = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (Instance != null)
            {
                // 다음 씬 전환을 위한 상태를 초기화
                Instance.shouldSkipNextFadeIn = false;
                Instance.nextFadeInDuration = defaultfadeDuration;
                Instance.nextFadeInCurve = defaltfadeCurve;
            }
            Destroy(gameObject); 
            return; 
        }

        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                Logger.Log("SceneFader: CanvasGroup를 찾을 수 없습니다");
                return;
            }
        }
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        nextFadeInDuration = fastFadeDuration;
        nextFadeInCurve = fastFadeCurve;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!isFirstSceneLoaded)
        {
            // 게임 시작 시 첫 씬에서는 페이드 인 X
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f; 
                canvasGroup.blocksRaycasts = false; 
            }
            isFirstSceneLoaded = true; // 첫 씬 로드 완료 
            Debug.Log($"SceneFader: 게임 시작 - 첫 씬 '{scene.name}'");
        }
        else
        {
            if (shouldSkipNextFadeIn)
            {
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 0f;
                    canvasGroup.blocksRaycasts = false;
                }
                Debug.Log($"SceneFader: 씬 '{scene.name}' 로드 완료. 페이드 인 건너뜀.");
            }
            else
            {
                if (canvasGroup != null)
                {
                    FadeIn(nextFadeInDuration, nextFadeInCurve);
                }
                Debug.Log($"SceneFader: 씬 '{scene.name}' 로드 완료.");
            }

            shouldSkipNextFadeIn = false;
        }
    }
    //두가지 방식: 스타일대로 적용, 시간과 커브를 직접 지정해서 적용
    public void FadeIn(FadeStyle style, Action onComplete = null)
    {
        GetFadeStyle(style, out float duration, out AnimationCurve curve);
        FadeIn(duration, curve, onComplete); 
    }
    public void FadeIn(float? duration = null, AnimationCurve curve = null, Action onComplete = null) 
    {
        if (canvasGroup == null)
        {
            onComplete?.Invoke();
            return;
        }

        StopAllCoroutines();

        float actualDuration = duration ?? defaultfadeDuration;
        AnimationCurve actualCurve = curve ?? defaltfadeCurve;

        if (!gameObject.activeInHierarchy) { onComplete?.Invoke(); return; } // 오브젝트가 비활성화된 경우 처리
        StartCoroutine(FadeRoutine(1, 0, actualDuration, actualCurve, onComplete));
    }

    //두가지 방식: 스타일대로 적용, 시간과 커브를 직접 지정해서 적용
    public void FadeOut(FadeStyle style, bool skipFade = false, Action onComplete = null)
    {
        GetFadeStyle(style, out float duration, out AnimationCurve curve);
        FadeOut(duration, curve, skipFade, onComplete); 
    }
    // 화면이 점차 어두워지는 효과 (알파 0 -> 1)
    public void FadeOut(float? duration = null, AnimationCurve curve = null, bool skipFade = false, Action onComplete = null)
    {
        if (canvasGroup == null) return;

        if (skipFade)
        {
            // 페이드 건너뛰기
            canvasGroup.alpha = 0f; 
            canvasGroup.blocksRaycasts = false; 
            onComplete?.Invoke();
            return;
        }
        float actualDuration = duration ?? defaultfadeDuration;
        AnimationCurve actualCurve = curve ?? defaltfadeCurve;

        StopAllCoroutines();
        if (canvasGroup == null || !gameObject.activeInHierarchy) { onComplete?.Invoke(); return; }
        StartCoroutine(FadeRoutine(0, 1, actualDuration, actualCurve, onComplete));
    }

    private IEnumerator FadeRoutine(float startAlpha, float targetAlpha, float duration, AnimationCurve curve, Action onComplete = null)
    {
        canvasGroup.blocksRaycasts = true;

        float timer = 0;
        // 현재 알파 값을 시작 알파 값으로 설정
        canvasGroup.alpha = startAlpha;

        while (timer < defaultfadeDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, curve.Evaluate(progress));
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
        canvasGroup.blocksRaycasts = (targetAlpha == 1);

        onComplete?.Invoke();
    }

    public void GetFadeStyle(FadeStyle style, out float duration, out AnimationCurve curve)
    {
        switch (style)
        {
            case FadeStyle.Fast:
                duration = fastFadeDuration;
                curve = fastFadeCurve;
                break;
            case FadeStyle.Slow:
                duration = slowFadeDuration;
                curve = slowFadeCurve;
                break;
            case FadeStyle.Flash:
                duration = flashFadeDuration;
                curve = flashFadeCurve;
                break;
            default:
                duration = defaultfadeDuration;
                curve = defaltfadeCurve;
                break;
        }
    }
}

