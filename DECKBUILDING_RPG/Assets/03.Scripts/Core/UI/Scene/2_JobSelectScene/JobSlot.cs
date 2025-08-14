using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class JobSlot : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text slotText;
    [SerializeField] private Image jobIconImage;

    private JobSO jobSO;
    private Action<JobSO> onClickAction;

    public void Init(JobSO jobSO, Action<JobSO> onClickAction)
    {
        this.jobSO = jobSO;
        this.onClickAction = onClickAction;

        if (slotText != null)
        {
            slotText.text = jobSO.name;
        }
        if (jobIconImage != null)
        {
            jobIconImage.sprite = jobSO.jobIcon;
        }

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
        }
    }

    private void OnClick()
    {
        onClickAction?.Invoke(jobSO);
    }

    public void Click()
    {
        OnClick();
    }
}
