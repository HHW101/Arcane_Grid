using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DetailInfoConnectedObject : MonoBehaviour, IShowDetailInfoable
{
    private DetailInfoData detailInfoData = new DetailInfoData();
    [SerializeField]
    private RectTransform rectTransform;
    [SerializeField]
    [TextArea] private string infoName;
    [SerializeField]
    [TextArea] private string infoDescription;

    private UIManager uiManager;
    private bool isHovered = false;
    private bool nowHovered = false;
    private bool valid = false;

    private void Start()
    {
        detailInfoData.name = infoName;
        detailInfoData.description = infoDescription;
        uiManager = GameManager.Instance.uiManager;
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

        nowHovered = valid && rectTransform.rect.Contains(localMousePosition);

        if (!isHovered && nowHovered)
        {
            uiManager.ShowGetDetailInfo();
        }
        else if (isHovered && !nowHovered)
        {
            uiManager.HideGetDetailInfo();
        }
        if (nowHovered && uiManager.IsHideGetDetailInfo())
        {
            uiManager.ShowGetDetailInfo();
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