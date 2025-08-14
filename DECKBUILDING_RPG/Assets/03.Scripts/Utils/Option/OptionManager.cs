using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio; // AudioMixer 사용
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class OptionsManager : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [SerializeField] private AudioMixer audioMixer;

    [Header("Graphics Settings")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullScreenToggle;

    [Header("Buttons")]
    [SerializeField] private Button applyButton; 
    [SerializeField] private Button closeButton;

    [SerializeField]
    private GameObject optionPanel;

    private Resolution[] resolutions; // 사용 가능한 해상도 목록

    //private float tempMasterVolume;
    //private float tempBGMVolume;
    //private float tempSFXVolume;
    private int tempResolutionIndex;
    private bool tempFullScreen;

    private void Awake()
    {
        // 해상도 드롭다운 초기화 (Awake에서 미리 초기화)
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);
            // 현재 화면 해상도와 일치하는 해상도 찾기
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        // 드롭다운 초기값 설정 (저장된 값 로드 전에)
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }
    private void OnEnable()
    {
        // 옵션 패널이 활성화될 때마다 저장된 설정 로드 및 임시 변수 초기화
        LoadSettings();

        // 슬라이더 및 드롭다운, 토글 리스너 연결
        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        fullScreenToggle.onValueChanged.AddListener(OnFullScreenChanged);

        // 버튼 리스너 연결
        applyButton.onClick.AddListener(ApplySettings);
        closeButton.onClick.AddListener(OnClickBackOrClose);
    }

    private void OnDisable()
    {
        // 옵션 패널이 비활성화될 때 리스너 제거 (중복 호출 방지)
        masterVolumeSlider.onValueChanged.RemoveAllListeners();
        bgmVolumeSlider.onValueChanged.RemoveAllListeners();
        sfxVolumeSlider.onValueChanged.RemoveAllListeners();
        resolutionDropdown.onValueChanged.RemoveAllListeners();
        fullScreenToggle.onValueChanged.RemoveAllListeners();

        applyButton.onClick.RemoveAllListeners();
        closeButton.onClick.RemoveAllListeners();
    }

    private void LoadSettings()
    {
        float masterVolumeDB, bgmVolumeDB, sfxVolumeDB;

        if (audioMixer.GetFloat("MasterVolume", out masterVolumeDB))
        {
            masterVolumeSlider.value = Mathf.Pow(10, masterVolumeDB / 20);
        }

        if (audioMixer.GetFloat("BGMVolume", out bgmVolumeDB))
        {
            bgmVolumeSlider.value = Mathf.Pow(10, bgmVolumeDB / 20);
        }

        if (audioMixer.GetFloat("SFXVolume", out sfxVolumeDB))
        {
            sfxVolumeSlider.value = Mathf.Pow(10, sfxVolumeDB / 20);
        }

        bool isFullScreen = PlayerPrefs.GetInt("FullScreen", 1) == 1;
        fullScreenToggle.isOn = isFullScreen;

        int resolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", resolutionDropdown.value);
        resolutionDropdown.value = resolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    // 볼륨 설정 메서드

    public void OnMasterVolumeChanged(float volume)
    {
        if (GameManager.Instance != null && GameManager.Instance.audioManager != null)
        {
            GameManager.Instance.audioManager.SetVolume("MasterVolume", volume);
        }
    }
    public void OnBGMVolumeChanged(float volume)
    {
        if (GameManager.Instance != null && GameManager.Instance.audioManager != null)
        {
            GameManager.Instance.audioManager.SetVolume("BGMVolume", volume);
        }
    }
    public void OnSFXVolumeChanged(float volume)
    {
        if (GameManager.Instance != null && GameManager.Instance.audioManager != null)
        {
            GameManager.Instance.audioManager.SetVolume("SFXVolume", volume);
        }
    }


    public void OnResolutionChanged(int resolutionIndex)
    {
        tempResolutionIndex = resolutionIndex;
    }

    public void OnFullScreenChanged(bool isFullScreen)
    {
        tempFullScreen = isFullScreen;
    }

    //적용 버튼 클릭 
    public void ApplySettings()
    {
        GameManager.Instance.audioManager.PlaySfx("Clicks-010");
        // 1. 그래픽 설정 적용 및 저장
        Resolution resolution = resolutions[resolutionDropdown.value];
        Screen.SetResolution(resolution.width, resolution.height, fullScreenToggle.isOn);
        PlayerPrefs.SetInt("ResolutionIndex", resolutionDropdown.value);
        PlayerPrefs.SetInt("FullScreen", fullScreenToggle.isOn ? 1 : 0);

        // 2. PlayerPrefs 변경사항 즉시 저장
        PlayerPrefs.Save();

        optionPanel.gameObject.SetActive(false);
        Logger.Log("설정 적용");
    }


    public void OnClickBackOrClose()
    {
        GameManager.Instance.audioManager.PlaySfx("Clicks-010");
        optionPanel.gameObject.SetActive(false);
    }
}