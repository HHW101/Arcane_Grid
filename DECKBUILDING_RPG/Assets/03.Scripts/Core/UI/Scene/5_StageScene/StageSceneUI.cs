using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageSceneUI : MonoBehaviour
{
    [SerializeField]
    private string nextScene;

    [SerializeField]
    private ClearStageUI clearStageUI;
    [SerializeField]
    private GameObject failUI;
    [SerializeField]
    private float waitSecondBeforeActiveUI;
    [SerializeField]
    private float waitSecondAfterActiveUI;

    private int curStageIndex = 0;
    private StageData stageData;
    private UIManager uiManager;
    private StageManager stageManager;
    private AudioManager audioManager;

    private Coroutine waitCanStageClear;
    private Coroutine waitStageFail;
    

    public void Start()
    {
        curStageIndex = GameManager.Instance.gameContext.saveData.campaignData.playerStateInCampaign.curStageIndex;
        uiManager = GameManager.Instance.uiManager;
        stageManager = GameManager.Instance.campaignManager.stageManager;
        audioManager = GameManager.Instance.audioManager;
        clearStageUI?.gameObject.SetActive(false);
        failUI?.SetActive(false);
        StartCoroutine(WaitCanStageClear());
        StartCoroutine(WaitStageFail());


        if (audioManager != null)
        {
            List<int> playableBGMIndices = new List<int> { 1, 2, 3, 4 }; //배경음 인덱스

            if (playableBGMIndices.Count > 0)
            {
                int randomIndex = playableBGMIndices[Random.Range(0, playableBGMIndices.Count)];

                audioManager.PlayBGMWithFade(randomIndex);
            }
        }
    }

    public void Update()
    {

    }

    IEnumerator WaitCanStageClear()
    {
        WaitUntil waitUntilAnnounceFinish = new WaitUntil(() => uiManager.IsAnnounceEnded() == true);
        yield return null;
        yield return null;
        yield return waitUntilAnnounceFinish;
        WaitUntil waitUntil = new WaitUntil(() => stageManager.CanStageClear());
        yield return waitUntil;
        clearStageUI?.gameObject.SetActive(true);
    }

    IEnumerator WaitStageFail()
    {
        WaitUntil waitUntilAnnounceFinish = new WaitUntil(() => uiManager.IsAnnounceEnded() == true);
        yield return null;
        yield return null;
        yield return waitUntilAnnounceFinish;
        WaitUntil waitUntil = new WaitUntil(() => stageManager.IsStageFail());
        yield return waitUntil;
        if(waitCanStageClear != null)
        {
            StopCoroutine(waitCanStageClear);
        }
        clearStageUI?.gameObject.SetActive(false);
        yield return new WaitForSeconds(waitSecondBeforeActiveUI);
        failUI?.SetActive(true);
        yield return new WaitForSeconds(waitSecondAfterActiveUI);
        SceneManager.LoadScene(nextScene);
    }
}