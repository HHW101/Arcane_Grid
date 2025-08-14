using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData : UnitData
{
    [Header("Prefab Info")]
    public string prefabPath;

    [Header("Player Job")]
    public JobEnum.Job jobEnum;

    [Header("Player Stats")]
    //public float currentHealth;
    public float currentMana;
    public int level = 1;
    public float experience;
    public int coin;
    public int actionPoint;
    public int technicalPoint;
    public int meleeAttackRange;
    public int gunRange;
    public int interactRange;
    public int radius = 0;
    public int defaultAttack;
    public int pushForce = 0;

    public int actionPointPerMove;
    public int actionPointPerAttack;
    public int actionPointPerInteract;

    [Header("Max Stats")]
    //public int maxHealth;
    public int maxMana;
    public int maxLevel;
    public int maxExperience;
    public int maxCoin;
    public int maxActionPoint;
    public int maxTechnicalPoint;

    public bool isDead;
    public bool isStunned;
    [Header("Cards")]
    public CardSetData cards;

    public PlayerData ClonePlayerData()
    {
        var baseData = base.CloneUnitData();
      
        return new PlayerData
        {
            unitTypeList = baseData.unitTypeList,
            portraitPath = this.portraitPath,
            unitIdentifierType = this.unitIdentifierType,

            prefabPath = this.prefabPath,
            jobEnum = this.jobEnum,

            currentHealth = this.currentHealth,
            currentMana = this.currentMana,
            level = this.level,
            experience = this.experience,
            coin = this.coin,
            actionPoint = this.actionPoint,
            technicalPoint = this.technicalPoint,
            meleeAttackRange = this.meleeAttackRange,
            gunRange = this.gunRange,
            interactRange = this.interactRange,
            actionPointPerMove = this.actionPointPerMove,
            actionPointPerAttack = this.actionPointPerAttack,
            actionPointPerInteract = this.actionPointPerInteract,
            radius= this.radius,
            defaultAttack = this.defaultAttack,
            pushForce = this.pushForce,
            
            maxHealth = this.maxHealth,
            maxMana = this.maxMana,
            maxLevel = this.maxLevel,
            maxExperience = this.maxExperience,
            maxCoin = this.maxCoin,
            maxActionPoint = this.maxActionPoint,
            maxTechnicalPoint = this.maxTechnicalPoint,

            isDead = this.isDead,
            isStunned = this.isStunned,

            cards = this.cards.CloneCardSetData()
        };
    }


    public void RecoverBeforeStartStage()
    {
        Logger.Log($"[PlayerData] Stage 시작 전 플레이어 상태 회복(Action Point)");
        actionPoint = maxActionPoint;
    }

    public bool SpendCoin(int amount)
    {
        if (amount < 0)
        {
            Logger.LogWarning($"[PlayerData] SpendCoin: - 금액 ({amount})은 지불할 수 없습니다.");
            return false;
        }

        if (this.coin >= amount) 
        {
            this.coin -= amount;
            this.coin = Mathf.Max(this.coin, 0);

            Logger.Log($"[PlayerData] 코인 지불 성공: {amount} 코인 사용. 남은 코인: {this.coin}");
            return true;
        }
        else
        {
            Logger.Log($"[PlayerData] 코인 부족! 필요한 코인: {amount}, 현재 코인: {this.coin}");
            return false;
        }
    }

    public void AddCoin(int amount)
    {
        if (amount < 0)
        {
            Logger.LogWarning($"[PlayerData] AddCoin: - 금액 ({amount})은 추가할 수 없습니다.");
            return;
        }

        this.coin += amount; 
        this.coin = Mathf.Min(this.coin, maxCoin);

        Logger.Log($"[PlayerData] 코인 획득: {amount} 코인 추가. 총 코인: {this.coin}");
    }
}

[Serializable]
public class PlayerStateInStage
{
    public HexCoord hexCoord;
    public bool turnStarted = false;
    public bool turnEnded = false;

    public List<StatusEffectInstance> statusEffectList = new List<StatusEffectInstance>();
    public bool statusEffectProcessedThisTurn = false;

    public bool isFlipped = false;
}