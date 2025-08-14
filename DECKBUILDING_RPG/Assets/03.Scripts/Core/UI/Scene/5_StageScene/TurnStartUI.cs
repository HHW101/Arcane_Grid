using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TurnStartUI : MonoBehaviour
{
    [Header("Turn Start")]
    [SerializeField] private Animator turnStartAnimator;
    [SerializeField] private ExternalCallbackConnectedObject turnStartAnimationEndCallbackConnectedObject;
    [SerializeField] private Image turnStartImage;

    [Header("Player Turn Start")]
    [SerializeField] private Animator playerTurnStartAnimator;
    [SerializeField] private ExternalCallbackConnectedObject playerTurnStartAnimationEndCallbackConnectedObject;
    [SerializeField] private Image playerTurnStartImage;
    [SerializeField] private TMP_Text curTurnText;

    [Header("NPC Turn Start")]
    [SerializeField] private Animator npcTurnStartAnimator;
    [SerializeField] private ExternalCallbackConnectedObject npcTurnStartAnimationEndCallbackConnectedObject;
    [SerializeField] private Image npcTurnStartImage;

    [Header("State Names (layer 0)")]
    [SerializeField] private string turnStartStateName = "TurnStart";
    [SerializeField] private string playerTurnStartStateName = "PlayerTurnStart";
    [SerializeField] private string npcTurnStartStateName = "NPCTurnStart";
    [SerializeField] private int layerIndex = 0;

    private Action playerTurnStartAnimationEndCallback;
    private Action npcTurnStartAnimationEndCallback;

    private StageManager stageManager;
    private UIManager uiManager;

    private void Awake()
    {
        if (turnStartAnimationEndCallbackConnectedObject != null)
        {
            turnStartAnimationEndCallbackConnectedObject.Init(() =>
            {
                if (turnStartImage) turnStartImage.enabled = false;
            });
        }

        if (playerTurnStartAnimationEndCallbackConnectedObject != null)
        {
            playerTurnStartAnimationEndCallbackConnectedObject.Init(OnPlayerTurnStartAnimationEnd);
        }

        if (npcTurnStartAnimationEndCallbackConnectedObject != null)
        {
            npcTurnStartAnimationEndCallbackConnectedObject.Init(OnNPCTurnStartAnimationEnd);
        }
    }

    private void Start()
    {
        if (turnStartImage)
        {
            turnStartImage.enabled = false;
        }
        if (playerTurnStartImage)
        {
            playerTurnStartImage.enabled = false;
        }
        if (npcTurnStartImage)
        {
            npcTurnStartImage.enabled = false;
        }
        stageManager = GameManager.Instance.campaignManager.stageManager;
        uiManager = GameManager.Instance.uiManager;
        uiManager?.RegisterTurnStartUI(this);
    }

    [ContextMenu("Player Turn Start (Synced)")]
    public void PlayerTurnStartAnimationStart()
    {
        if (turnStartImage)
        {
            turnStartImage.enabled = true;
        }
        if (playerTurnStartImage)
        {
            playerTurnStartImage.enabled = true;
        }
        curTurnText?.SetText($"{stageManager.GetCurStageData().curTurn} Turn");

        PlayState(turnStartAnimator, turnStartStateName, layerIndex);
        PlayState(playerTurnStartAnimator, playerTurnStartStateName, layerIndex);
    }

    public void OnPlayerTurnStartAnimationEnd()
    {
        if (turnStartImage)
        {
            turnStartImage.enabled = false;
        }
        if (playerTurnStartImage)
        {
            playerTurnStartImage.enabled = false;
        }
        playerTurnStartAnimationEndCallback?.Invoke();
    }

    public void RegisterPlayerTurnStartAnimationEndCallback(Action callback = null)
    {
        playerTurnStartAnimationEndCallback = callback;
    }

    public void RegisterNPCTurnStartAnimationEndCallback(Action callback = null)
    {
        npcTurnStartAnimationEndCallback = callback;
    }

    [ContextMenu("NPC Turn Start (Synced)")]
    public void NPCTurnStartAnimatonStart()
    {
        if (turnStartImage)
        {
            turnStartImage.enabled = true;
        }
        if (npcTurnStartImage)
        {
            npcTurnStartImage.enabled = true;
        }

        PlayState(turnStartAnimator, turnStartStateName, layerIndex);
        PlayState(npcTurnStartAnimator, npcTurnStartStateName, layerIndex);
    }

    public void OnNPCTurnStartAnimationEnd()
    {
        if (turnStartImage)
        {
            turnStartImage.enabled = false;
        }
        if (npcTurnStartImage)
        {
            npcTurnStartImage.enabled = false;
        }
        npcTurnStartAnimationEndCallback?.Invoke();
    }

    private static void PlayState(Animator animator, string stateName, int layer)
    {
        if (!animator || string.IsNullOrEmpty(stateName))
        {
            return;
        }

        animator.CrossFadeInFixedTime(stateName, 0f, layer, 0f);
    }
}
