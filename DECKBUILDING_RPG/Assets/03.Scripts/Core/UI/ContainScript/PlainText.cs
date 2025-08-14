using TMPro;
using UnityEngine;

public class PlainText : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private RectTransform rectTransform;

    public readonly Color textColor = new Color(0.386f, 0.255f, 0.225f, 1f);

    public void Init(string rawText)
    {
        text.SetText(rawText);
        text.color = textColor;
    }
}
