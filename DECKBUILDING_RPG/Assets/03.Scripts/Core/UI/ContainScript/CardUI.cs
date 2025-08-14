using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardUI : MonoBehaviour
{
    [SerializeField]
    private GameObject block;
    // Start is called before the first frame update
    private UIManager uiManager;
    void Start()
    {
        uiManager = GameManager.Instance.uiManager;
        block.SetActive(false);
        StartCoroutine(WaitAnnounceEnd());
    }

    IEnumerator WaitAnnounceEnd()
    {
        WaitUntil waitUntilAnnounceFinish = new WaitUntil(() => uiManager.IsAnnounceEnded() == true);
        yield return null;
        yield return null;
        yield return waitUntilAnnounceFinish;
        block.SetActive(true);
    }
}
