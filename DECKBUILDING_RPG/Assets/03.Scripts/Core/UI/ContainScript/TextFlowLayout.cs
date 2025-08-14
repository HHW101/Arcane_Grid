using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextFlowLayout : MonoBehaviour
{
    [SerializeField] private RectTransform layoutRectTransform;
    [SerializeField] private GameObject plainTextPrefab;
    [SerializeField] private GameObject detailInfoConnectedTextPrefab;

    private readonly List<GameObject> spawnedTexts = new();

    private void Awake()
    {
    }

    public void AddText(string rawText)
    {
        GameObject plainText = Instantiate(plainTextPrefab, transform);
        plainText.transform.SetParent(transform, false);

        var text = plainText.GetComponentInChildren<TMP_Text>();
        text.SetText(rawText);
        text.ForceMeshUpdate();
        spawnedTexts.Add(plainText);
    }

    public void AddDetailInfoConnectedText(DetailInfoData data)
    {
        GameObject detailInfoConnectedText = Instantiate(detailInfoConnectedTextPrefab, transform);
        detailInfoConnectedText.transform.SetParent(transform, false);

        var script = detailInfoConnectedText.GetComponent<DetailInfoConnectedText>();
        script.Init(data);

        var tmp = detailInfoConnectedText.GetComponentInChildren<TMP_Text>();
        tmp.ForceMeshUpdate();
        spawnedTexts.Add(detailInfoConnectedText);
    }

    public void ClearAllTexts()
    {
        foreach (var text in spawnedTexts)
        {
            if (text != null)
            {
                Destroy(text);
            }
        }
        spawnedTexts.Clear();
    }
}
