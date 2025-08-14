using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // TextMeshPro를 사용하는 경우, 이 using 문이 필요합니다.

public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager Instance { get; private set; }

    [Header("UI 참조")]
    [SerializeField] private CanvasGroup loadingPanelCanvasGroup;
    [SerializeField] private Image loadingProgressBar;
    [SerializeField] private TextMeshProUGUI percentageTxt;
    [SerializeField] private Image loadingBackgroundImage;
    [SerializeField] private TextMeshProUGUI DescriptionText;

    [Header("페이드 설정")]
    [SerializeField] private float fadeDuration = 0.5f; // 로딩 화면이 나타나고 사라지는 시간

    [Header("로딩 화면 콘텐츠")]
    [SerializeField] private Sprite[] backgroundImages; // 여러 배경 이미지를 담을 배열
    [SerializeField] private string[] loadingDescriptions; // 로딩 중 설명을 담을 배열

    private AsyncOperation currentLoadingOperation; // 비동기 씬 로드 작업
    private string targetSceneName; // 로드할 씬

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
         
        if (loadingPanelCanvasGroup != null)
        {
            loadingPanelCanvasGroup.alpha = 0f;
            loadingPanelCanvasGroup.blocksRaycasts = false;
        }

    }

    public void ShowLoadingScreen(string sceneName, bool stopBGM)
    {
        targetSceneName = sceneName;
        StopAllCoroutines();
        StartCoroutine(ShowAndLoadRoutine(stopBGM));
    }

    private IEnumerator ShowAndLoadRoutine(bool stopBGM)
    {
        if (stopBGM)
        {
            if (GameManager.Instance != null && GameManager.Instance.audioManager != null)
            {
                GameManager.Instance.audioManager.FadeOutBGM(1.5f);
            }
        }

        ApplyRandomLoadingContent();
        // 1. 로딩 화면 페이드 인
        loadingPanelCanvasGroup.blocksRaycasts = true; // 로딩 중 사용자 입력 차단
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            loadingPanelCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }
        loadingPanelCanvasGroup.alpha = 1f; 

        currentLoadingOperation = SceneManager.LoadSceneAsync(targetSceneName);
        // 중요: 씬이 90% 로드되어도 즉시 활성화X
        currentLoadingOperation.allowSceneActivation = false;
        float startTime = Time.time;

        // 3. 로딩 진행률 UI 업데이트
        while (!currentLoadingOperation.isDone)
        {

            float progress = Mathf.Clamp01(currentLoadingOperation.progress / 0.9f);

            float elapsedTime = Time.time - startTime;
            float normalizedElapsedTime = Mathf.Clamp01(elapsedTime / 1.5f);

            float displayProgress = Mathf.Max(progress, normalizedElapsedTime);
            displayProgress = Mathf.Max(displayProgress, 0.99f);

            if (loadingProgressBar != null)
            {
                loadingProgressBar.fillAmount = displayProgress;
            }
            if (percentageTxt != null)
            {
                percentageTxt.text = $"{Mathf.RoundToInt(displayProgress * 100)}%";
            }

            // 씬이 거의 로드되었을 때
            if (currentLoadingOperation.progress >= 0.9f)
            {
                // 여기에 씬 활성화 전에 완료해야 할 추가 작업
                yield return new WaitForSeconds(1.5f); // 로딩 화면을 더 오래 보여주기 위해

                // 모든 조건이 충족되면 씬 활성화를 허용
                currentLoadingOperation.allowSceneActivation = true;
            }
            yield return null;
        }

        StartCoroutine(HideRoutine());
    }

    // 로딩 화면을 페이드 아웃
    private IEnumerator HideRoutine()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            loadingPanelCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }
        loadingPanelCanvasGroup.alpha = 0f;
        loadingPanelCanvasGroup.blocksRaycasts = false;


        currentLoadingOperation = null;
        targetSceneName = null;

        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.CompleteLoading();
        }
    }

    private void ApplyRandomLoadingContent()
    {
        if (loadingBackgroundImage != null && backgroundImages != null && backgroundImages.Length > 0)
        {
            int randomIndex = Random.Range(0, backgroundImages.Length);
            loadingBackgroundImage.sprite = backgroundImages[randomIndex];
        }

        if (DescriptionText != null && loadingDescriptions != null && loadingDescriptions.Length > 0)
        {
            int randomIndex = Random.Range(0, loadingDescriptions.Length);
            DescriptionText.text = loadingDescriptions[randomIndex];
        }
 
    }
}