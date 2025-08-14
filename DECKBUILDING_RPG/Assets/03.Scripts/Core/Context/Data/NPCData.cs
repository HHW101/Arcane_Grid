using NPCEnum;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NPCData : UnitData
{
    public string prefabPath;
    public string corpsePrefabPath;
    public HexCoord hexCoord;
    public NPCStateResult currentState;
    //public float currentHealth;
    //public float maxHealth;
    public bool turnEnded = false;
    public bool turnStarted = false;
    public bool isEnemy = true;
    public bool isAggroToPlayer = true;      
    public bool isAttackEnemy = false;       
    public bool isAttackableByEnemies = true;

    public int detectRange = 2;
    public int attackRange = 1;

    public int actionPoint = 3;
    public int currentActionPoint = 3;

    public int actionPointPerMove = 1;
    public int actionPointPerAttack = 2;
    
    public int maxAttackPerTurn = 3; 
    public int currentAttackCount = 0;
    
    public int attackDamage = 10; 
    public float attackDelay = 0.5f; 
    
    public List<int> droppedCardIDList = new List<int>();

    public NPCDropCardTable npcDropCardTable;

    public List<StatusEffectInstance> statusEffectList;
    public bool statusEffectProcessedThisTurn = false;
    public bool isFlipped = false;
    public bool isStunned = false;

    public bool canBehaviour = true;
    
    public NPCData CloneNPCData()
    {
        var unitClone = this.CloneUnitData();
        return new NPCData
        {
            prefabPath = this.prefabPath,
            corpsePrefabPath = this.corpsePrefabPath,
            hexCoord = new HexCoord(this.hexCoord.q, this.hexCoord.r),
            currentState = this.currentState,
            currentHealth = this.currentHealth,
            maxHealth = this.maxHealth,
            turnEnded = this.turnEnded,
            turnStarted = this.turnStarted,
            isEnemy = this.isEnemy,
            isAggroToPlayer = this.isAggroToPlayer,
            isAttackEnemy = this.isAttackEnemy,
            isAttackableByEnemies = this.isAttackableByEnemies,
            
            statusEffectList = this.statusEffectList,
            statusEffectProcessedThisTurn = this.statusEffectProcessedThisTurn,

            detectRange = this.detectRange,
            attackRange = this.attackRange,

            actionPoint = this.actionPoint,
            currentActionPoint = this.currentActionPoint,

            actionPointPerMove = this.actionPointPerMove,
            actionPointPerAttack = this.actionPointPerAttack,

            maxAttackPerTurn = this.maxAttackPerTurn,
            currentAttackCount = this.currentAttackCount,

            attackDamage = this.attackDamage,
            attackDelay = this.attackDelay,

            droppedCardIDList = this.droppedCardIDList,
            npcDropCardTable = this.npcDropCardTable,

            unitTypeList = unitClone.unitTypeList,
            portraitPath = this.portraitPath,
            unitIdentifierType = this.unitIdentifierType,
            isFlipped =  this.isFlipped,
            isStunned = this.isStunned,
            canBehaviour = this.canBehaviour
        };
    }
}