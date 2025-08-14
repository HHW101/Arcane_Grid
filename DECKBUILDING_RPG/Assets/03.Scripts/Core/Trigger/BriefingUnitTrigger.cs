using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BriefingUnitTrigger : MonoBehaviour
{
    [SerializeField]
    private bool autoDestroy = false;
    // Start is called before the first frame update
    [SerializeField]
    private float unitFollowSeconds = 0.5f;
    [SerializeField]
    private float tileFollowSeconds = 0.1f;
    void Start()
    {
        StartCoroutine(BrefingUnitRoutine());
    }

    private IEnumerator BrefingUnitRoutine()
    {
        yield return null;
        yield return null;
        UIManager uiManager = GameManager.Instance.uiManager;
        StageManager stageManager = GameManager.Instance.campaignManager.stageManager;
        yield return new WaitUntil(() => uiManager.IsAnnounceEnded() == true);
        stageManager.StartBriefingUnit();
        WaitForSeconds waitForSeconds = new WaitForSeconds(unitFollowSeconds);
        GameContext gameContext = GameManager.Instance.gameContext;
        APlayer player = gameContext.player;
        List<ANPC> aNPCList = gameContext.npcList;
        List<ATile> aTileList = gameContext.tileList;
        player.CameraFollow();
        yield return waitForSeconds;
        for (int i = 0; i < aNPCList.Count; i++)
        {
            aNPCList[i].CameraFollow();
            yield return waitForSeconds;
        }
        player.CameraFollow();
        yield return waitForSeconds;
        stageManager.EndBriefingUnit();
        if (autoDestroy)
        {
            Destroy(gameObject);
        }
    }
}
