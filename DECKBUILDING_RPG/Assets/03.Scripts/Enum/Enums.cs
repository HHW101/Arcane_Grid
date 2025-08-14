using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardEnum
{
    
    public enum EffectType
    {
        Attack,
        Armor,
        Heal,
        Stun=10,
        Burn,
        Poison,
        Move=100,
        Draw=1000,

    }
    public enum Rarity {
        Normal,
        Rare,
        Legend
    }

    public enum SortMode
    {
       Cost,
       Value,
       Range
    }
    public enum CardUIMode
    {
        Hand,
        Deck,
        Pack,
        Drop,
        Show
    }
    public enum AttackType
    {
        Melee,
        Range
    }

}

namespace NPCEnum
{
    public enum NPCStateResult
    {
        None,          
        ToIdle,
        ToPatrol,
        ToChase,
        ToAttack,
        ToStun,
        EndTurn
    }
    public enum NPCType
    {
        Melee,
        Ranged
    }
}
namespace JobEnum
{
    public enum Job
    {
        Warrior,
        Wizard,
        CowBoy,
        Developer
    }
}

namespace TurnEnum
{
    public enum TurnPhase
    {
        Player,
        NPC,
        Tile
    }
}

namespace ClearEnum
{
    public enum ClearCondition
    {
        PlayerSurvive,
        KillAllEnemy,
        KillTargetEnemy,
        Escape
    }
}

namespace FailEnum
{
    public enum FailCondition
    {
        None,
        OverTurn
    }
}

namespace TileEnum
{
    public enum TileType
    {
        Empty,
        Grass,
        Escape,
        Forest,
        Mountain,
        Hill,
        Water,
        Road,
        Dirt,

        VolcanoMountain,
        VolcanoHill,
        VolcanoDirt,
        Magma,

        IceMountain,
        IceHill,
        IceDirt,
        Ice,

        Floor,
        DesertDirt,
        DesertHill,
        DarkDesertHill

    }
}

namespace StoryResourceEnum
{
    public enum StoryResourceType
    {
        Announce,
        Dialog,
        Image
    }
}

namespace StageEnum
{
    public enum StageType
    {
        Default,
        Escape,
        Boss,
        FirstCampaignStart,
        TutorialCampaignStart,
        TutorialCampaignEscape,
        TutorialCampaignRest,
        TutorialCampaignBattle
    }
}


namespace CampaignEnum
{
    public enum CampaignTypeEnum
    {
        Tutorial,
        Default
    }
}

[Serializable]
public enum HexDirection
{
    TopRight,
    Right,
    BottomRight,
    BottomLeft,
    Left,
    TopLeft
}

namespace HoverHighlightEnum
{
    public enum HoverHighlight
    {
        Default,
        Move,
        Attack,
        UseCard,
        Interact,
        None
    }
}

namespace FadeStyleEnum
{
    public enum FadeStyle
    {
        Fast,
        Slow,
        Flash
    }
}

namespace UnitEnum
{
    public enum UnitType
    {
        Default,
        Float,
        Swim,
        Human,
        Inanimate
    }
    public enum UnitIdentifierType
    {
        None,
        Empty,
        Player,
        Boss1,
        Boss1Corpse,
        Enemy1,
        Enemy1Corpse,
        Enemy2,
        Enemy2Corpse,
        NPC1,
        NPC1Corpse,
        ShopNPC,
        Chair,
        Desk,
        OakBarrel,
        BigBullRed,
        BigBullBlue,
        BabyBullRed,
        BabyBullBlue,
        BigBullRedCorpse,
        BigBullBlueCorpse,
        BabyBullRedCorpse,
        BabyBullBlueCorpse,
        IndianBoy,
        IndianBoyCorpse,
        IndianGirl,
        IndianGirlCorpse,
        Cactus,
        HumanCactus
    }

    public enum UnitCampType
    {
        None,
        Ally,
        Enemy,
        Neutral
    }
}

namespace StatusEffectEnum
{
    public enum StatusEffectType
    {
        Stun,
        Burn,
        Poison
    }
}