using TMPro;
using UnityEngine;

public class DetailInfoConnectedText : MonoBehaviour, IShowDetailInfoable
{
    [SerializeField]
    private DetailInfoData detailInfoData;
    [SerializeField]
    private TMP_Text text;
    [SerializeField]
    private RectTransform rectTransform;
    public readonly Color textColor = new Color(0.733f, 0.569f, 0.431f, 1f);

    private UIManager uiManager;
    private bool isHovered = false;
    private bool nowHovered = false;
    private bool valid = false;

    public void Init(DetailInfoData detailInfoData)
    {
        uiManager = GameManager.Instance.uiManager;
        this.detailInfoData = detailInfoData;
        text.SetText(detailInfoData.GetHeaderText());
        text.color = textColor;
    }

    private void Update()
    {
        Vector2 localMousePosition;
        valid = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            Input.mousePosition,
            null,
            out localMousePosition
        );

        nowHovered = false;
        if (valid)
        {
            nowHovered = rectTransform.rect.Contains(localMousePosition);
        }
        if (!isHovered && nowHovered)
        {
            uiManager.ShowGetDetailInfo();
        }
        else if (isHovered && !nowHovered)
        {
            uiManager.HideGetDetailInfo();
        }
        isHovered = nowHovered;

        KeyCode keyCode = uiManager.GetShowInfoKeyCode();

        if (Input.GetKeyDown(keyCode))
        {
            if (isHovered)
            {
                uiManager.ShowDetailInfo(this);
            }
        }
    }

    public DetailInfoData GetDataForDetailInfoUI()
    {
        return detailInfoData;
    }
}