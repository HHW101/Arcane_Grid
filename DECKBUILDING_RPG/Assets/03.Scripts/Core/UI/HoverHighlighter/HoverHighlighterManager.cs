using UnityEngine;

public class HoverHighlighterManager : MonoBehaviour
{
    [SerializeField]
    private HoverHighlighterDefault hoverHighlighterDefault;

    [SerializeField]
    private HoverHighlighterMove hoverHighlighterMove;

    [SerializeField]
    private HoverHighlighterAttack hoverHighlighterAttack;

    [SerializeField]
    private HoverHighlighterUseCard hoverHighlighterUseCard;

    [SerializeField]
    private HoverHighlighterInteract hoverHighlighterInteract;

    private HoverHighlightEnum.HoverHighlight currentHighlight = HoverHighlightEnum.HoverHighlight.Default;

    private StageManager stageManager;

    private void Start()
    {
        stageManager = GameManager.Instance.campaignManager.stageManager;
        ChangeState(currentHighlight);
    }

    private void Update()
    {
        HoverHighlightEnum.HoverHighlight nextHighlight = GetHighlightFromStageManager(stageManager);

        if (nextHighlight != currentHighlight)
        {
            currentHighlight = nextHighlight;
            ChangeState(currentHighlight);
        }
    }

    private HoverHighlightEnum.HoverHighlight GetHighlightFromStageManager(StageManager stageManager)
    {
        int count = 0;
        HoverHighlightEnum.HoverHighlight result = HoverHighlightEnum.HoverHighlight.Default;

        if (stageManager.IsProcessingUnitAct())
        {
            return HoverHighlightEnum.HoverHighlight.None;
        }
        if(stageManager.IsStageClear() || stageManager.IsStageFail())
        {
            return HoverHighlightEnum.HoverHighlight.None;
        }
        if (stageManager.IsReadyToMove())
        {
            count++;
            result = HoverHighlightEnum.HoverHighlight.Move;
        }
        if (stageManager.IsReadyToAttack())
        {
            count++;
            result = HoverHighlightEnum.HoverHighlight.Attack;
        }
        if (stageManager.IsReadyToUseCard())
        {
            count++;
            result = HoverHighlightEnum.HoverHighlight.UseCard;
        }
        if (stageManager.IsReadyToInteract())
        {
            count++;
            result = HoverHighlightEnum.HoverHighlight.Interact;
        }
        return count == 1 ? result : HoverHighlightEnum.HoverHighlight.Default;
    }

    public void ChangeState(HoverHighlightEnum.HoverHighlight highlight)
    {
        hoverHighlighterDefault.SetState(highlight);
        hoverHighlighterMove.SetState(highlight);
        hoverHighlighterAttack.SetState(highlight);
        hoverHighlighterUseCard.SetState(highlight);
        hoverHighlighterInteract.SetState(highlight);
    }
}
